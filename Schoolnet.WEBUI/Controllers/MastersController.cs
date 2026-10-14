using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Schoolnet.Domain;
namespace Schoolnet.WEBUI.Controllers;
[Authorize(Roles="Administrator,Coordinator")]
public sealed class MastersController(ITrainingRepository repo,PasswordHasher<string> hasher) : AppController(repo)
{
    private static readonly Dictionary<string,string> Titles=new() { ["project"]="Projects",["school"]="Schools",["trainee"]="Trainees",["location"]="Locations",["user"]="Users & permissions",["template"]="Feedback templates" };
    private bool Allowed(string kind,bool edit=false) => Titles.ContainsKey(kind) && (Role=="Administrator" || kind=="trainee" || (!edit && kind is "project" or "school"));
    private string Select(string kind) => kind switch
    {
        "project"=>"SELECT * FROM schoolnet.project WHERE schoolnet.can_project(@actor,id)",
        "school"=>"SELECT s.*, (SELECT jsonb_agg(project_id) FROM schoolnet.project_school WHERE school_id=s.id) projects FROM schoolnet.school_directory s WHERE EXISTS(SELECT 1 FROM schoolnet.project_school ps WHERE ps.school_id=s.id AND schoolnet.can_project(@actor,ps.project_id))",
        "trainee"=>"SELECT t.*,p.name project,s.name school,s.state,s.district,s.block,s.cluster,s.state_id,s.district_id,s.block_id,s.cluster_id FROM schoolnet.trainee t JOIN schoolnet.project p ON p.id=t.project_id JOIN schoolnet.school_directory s ON s.id=t.school_id WHERE schoolnet.can_project(@actor,t.project_id)",
        "location"=>"SELECT l.*,p.name parent FROM schoolnet.location l LEFT JOIN schoolnet.location p ON p.id=l.parent_id WHERE schoolnet.actor_role(@actor)='Administrator'",
        "user"=>"SELECT u.id,u.username,u.name,u.role,u.active,(SELECT jsonb_agg(project_id) FROM schoolnet.user_project WHERE user_id=u.id) projects FROM schoolnet.app_user u WHERE schoolnet.actor_role(@actor)='Administrator'",
        "template"=>"SELECT * FROM schoolnet.feedback_template WHERE schoolnet.actor_role(@actor)='Administrator'",
        _=>throw new ArgumentException("kind")
    };
    public async Task<IActionResult> Index(string kind="project",string search="",long project=0,long school=0,string status="",long state=0,long district=0,long block=0,long cluster=0)
    {
        if(!Allowed(kind)) return Forbid();
        var rows=await Repo.Query(Select(kind),("actor",Actor));
        rows=rows.Where(r=>string.IsNullOrWhiteSpace(search) || (r.Text("name")+" "+r.Text("username")+" "+r.Text("code")).Contains(search,StringComparison.OrdinalIgnoreCase)).Where(r=>kind!="trainee" || (project==0 || r.Id("project_id")==project) && (school==0 || r.Id("school_id")==school)).Where(r=>status=="" || r.Flag()==(status=="Active")).OrderBy(r=>r.Text("name")).ToList();
        if(kind=="trainee") rows=rows.Where(r=>(state==0||r.Id("state_id")==state)&&(district==0||r.Id("district_id")==district)&&(block==0||r.Id("block_id")==block)&&(cluster==0||r.Id("cluster_id")==cluster)).ToList();
        return View(new Screen { Title=Titles[kind],Kind=kind,Rows=rows,Choices=await Repo.Choices(Actor),Filters=new() { ["search"]=search,["project"]=project.ToString(),["school"]=school.ToString(),["status"]=status,["state"]=state.ToString(),["district"]=district.ToString(),["block"]=block.ToString(),["cluster"]=cluster.ToString() } });
    }
    public async Task<IActionResult> Edit(string kind,long id=0)
    {
        if(!Allowed(kind,true)) return Forbid();
        var item=id==0?new Record { ["active"]=true }: (await Repo.Query(Select(kind),("actor",Actor))).FirstOrDefault(r=>r.Id()==id);
        if(item is null) return NotFound();
        return View(new Screen { Title=(id==0?"Add ":"Edit ")+Titles[kind],Kind=kind,Item=item,Choices=await Repo.Choices(Actor) });
    }
    public async Task<IActionResult> ModifyTrainee(long id)
    {
        var item=await Trainee(id);
        if(item is null) return NotFound();
        return View(new Screen { Title="Modify Trainee",Kind="trainee",Item=item });
    }
    [HttpPost] public async Task<IActionResult> ModifyTrainee(long id,IFormCollection form)
    {
        var item=await Trainee(id);
        if(item is null) return NotFound();
        // Only these contact fields are accepted. Association and login values come from the stored record.
        var data=new JsonObject();
        foreach(var field in new[]{"name","mobile","email","reason"})
        {
            var value=form[field].ToString().Trim();
            data[field]=value; item[field]=value;
        }
        var error=ValidateTraineeContact(data);
        if(error=="")
        {
            try
            {
                await Repo.Save("save_trainee_contact",id,data.ToJsonString(),Actor);
                TempData["Success"]="Trainee contact details updated successfully.";
                return RedirectToAction(nameof(Index),new {kind="trainee"});
            }
            catch(Exception ex) { error=Friendly(ex); }
        }
        return View(new Screen { Title="Modify Trainee",Kind="trainee",Item=item,Error=error });
    }
    private Task<Record?> Trainee(long id) => Repo.One(Select("trainee")+" AND t.id=@id",("actor",Actor),("id",id));
    private static string ValidateTraineeContact(JsonObject data)
    {
        var name=data["name"]!.ToString(); var mobile=data["mobile"]!.ToString();
        var email=data["email"]!.ToString(); var reason=data["reason"]!.ToString();
        if(name.Length is <1 or >200) return "Enter a name between 1 and 200 characters.";
        if(!System.Text.RegularExpressions.Regex.IsMatch(mobile,@"\A\+?[0-9]{10,15}\z")) return "Enter a mobile number with 10 to 15 digits and an optional +.";
        if(email.Length>254 || !System.Text.RegularExpressions.Regex.IsMatch(email,@"\A[^\s@]+@[^\s@]+\.[^\s@]+\z")) return "Enter a valid email address of up to 254 characters.";
        if(reason.Length is <1 or >1000) return "Enter a correction reason between 1 and 1000 characters.";
        return "";
    }
    [HttpPost] public async Task<IActionResult> Edit(string kind,long id,IFormCollection form)
    {
        if(!Allowed(kind,true)) return Forbid();
        try
        {
            var data=Data(form); RequireFields(data,"name");
            switch(kind)
            {
                case "project": RequireFields(data,"code"); break;
                case "school": RequireFields(data,"code","cluster_id"); break;
                case "location": data["kind"]=form["location_kind"].ToString(); RequireFields(data,"kind"); break;
                case "trainee": RequireFields(data,"username","mobile","email","project_id","school_id"); break;
                case "user":
                    RequireFields(data,"username","role"); var password=form["password"].ToString();
                    if(id==0 && password.Length<10 || password.Length>0 && password.Length<10) throw new ArgumentException("password");
                    data["password_hash"]=password.Length>0?hasher.HashPassword(data["username"]!.ToString(),password):""; data.Remove("password"); break;
                case "template":
                    var questions=new JsonArray(); var labels=form["question_label"]; var types=form["question_type"]; var choices=form["question_choices"];
                    for(var i=0;i<labels.Count;i++) if(!string.IsNullOrWhiteSpace(labels[i])) questions.Add(new JsonObject { ["id"]="q"+(i+1),["label"]=labels[i],["type"]=types.ElementAtOrDefault(i)??"Text",["required"]=form["question_required"].Contains(i.ToString()),["choices"]=new JsonArray((choices.ElementAtOrDefault(i)??"").Split('|',StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries).Select(c=>(JsonNode?)JsonValue.Create(c)).ToArray()) });
                    data["questions"]=questions; break;
            }
            await Repo.Save("master:"+kind,id,data.ToJsonString(),Actor); TempData["Success"]="Saved successfully.";
            if(kind=="trainee")
            {
                var duplicates=await Repo.One("SELECT count(*) count FROM schoolnet.trainee WHERE (mobile=@mobile OR lower(email)=lower(@email)) AND schoolnet.can_project(@actor,project_id)",("mobile",form["mobile"].ToString()),("email",form["email"].ToString()),("actor",Actor));
                if(duplicates!.Id("count")>1) TempData["Success"]="Saved. Another trainee uses this mobile number or email address; please review.";
            }
            return RedirectToAction(nameof(Index),new {kind});
        }
        catch(Exception ex) { var item=FormRecord(form); item["id"]=id; return View(new Screen {Title="Edit "+Titles[kind],Kind=kind,Item=item,Error=Friendly(ex),Choices=await Repo.Choices(Actor)}); }
    }
}
