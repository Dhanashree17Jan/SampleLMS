using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schoolnet.Domain;
namespace Schoolnet.WEBUI.Controllers;
public sealed class BatchesController(ITrainingRepository repo) : AppController(repo)
{
    public async Task<IActionResult> Index(string search="",long project=0,long school=0,string status="") => View(new Screen { Title="Batches",Choices=await Repo.Choices(Actor),Filters=new() { ["search"]=search,["project"]=project.ToString(),["school"]=school.ToString(),["status"]=status },Rows=await Repo.Query("SELECT * FROM schoolnet.batch_directory WHERE schoolnet.can_batch(@actor,id) AND (name ILIKE @search OR code ILIKE @search) AND (@project=0 OR project_id=@project) AND (@school=0 OR school_id=@school) AND (@status='' OR display_status=@status) ORDER BY start_date DESC,id DESC",("actor",Actor),("search","%"+search+"%"),("project",project),("school",school),("status",status)) });
    public async Task<IActionResult> Detail(long id)
    {
        var batch=await Batch(id); if(batch is null) return NotFound();
        var roster=await Repo.Query("SELECT a.id,t.name,t.username,count(at.id) total,count(at.id) FILTER(WHERE at.status='Present') present,count(at.id) FILTER(WHERE at.status='Absent') absent,count(at.id) FILTER(WHERE at.status='Not marked') unmarked,(SELECT count(*) FROM schoolnet.feedback_submission f JOIN schoolnet.trainingplandetails d ON d.training_plan_detail_id=f.training_plan_detail_id WHERE f.assignment_id=a.id AND d.day_number=1) initial_feedback,(SELECT count(*) FROM schoolnet.feedback_submission f JOIN schoolnet.trainingplandetails d ON d.training_plan_detail_id=f.training_plan_detail_id WHERE f.assignment_id=a.id AND d.day_number=@last) final_feedback FROM schoolnet.batch_assignment a JOIN schoolnet.trainee t ON t.id=a.trainee_id LEFT JOIN schoolnet.attendance at ON at.assignment_id=a.id WHERE a.batch_id=@id AND (@staff OR t.user_id=@actor) GROUP BY a.id,t.name,t.username ORDER BY t.name",("last",(int)batch.Id("number_of_days")),("id",id),("staff",Role!="Trainee"),("actor",Actor));
        var eligible=Staff && batch.Text("status")=="Draft"?await Repo.Query("SELECT t.* FROM schoolnet.trainee t WHERE t.active AND t.project_id=@project AND t.school_id=@school AND NOT EXISTS(SELECT 1 FROM schoolnet.batch_assignment a WHERE a.batch_id=@id AND a.trainee_id=t.id) ORDER BY t.name",("project",batch.Id("project_id")),("school",batch.Id("school_id")),("id",id)):[];
        return View(new Screen {Title=batch.Text("name"),Item=batch,Roster=roster,Eligible=eligible,Days=await Repo.Query("SELECT d.*,(@start::date+day_number-1) day_date FROM schoolnet.trainingplandetails d WHERE training_plan_id=@plan ORDER BY day_number",("start",batch.Text("start_date")),("plan",batch.Id("training_plan_id"))) });
    }
    [Authorize(Roles="Administrator,Coordinator")] public async Task<IActionResult> Edit(long id=0)
    {
        var batch=id==0?new Record { ["start_date"]=DateTime.Today.ToString("yyyy-MM-dd") }:await Batch(id);
        if(batch is null || id>0 && batch.Text("status")!="Draft") return NotFound();
        return View(new Screen {Title=id==0?"Create batch":"Edit batch",Item=batch,Choices=await Repo.Choices(Actor)});
    }
    [HttpPost,Authorize(Roles="Administrator,Coordinator")] public async Task<IActionResult> Edit(long id,IFormCollection form)
    {
        try { var data=Data(form); RequireFields(data,"code","name","project_id","school_id","training_plan_id","number_of_days","start_date","end_date"); var saved=await Repo.Save("save_batch",id,data.ToJsonString(),Actor); TempData["Success"]="Batch saved. Add trainees before scheduling."; return RedirectToAction(nameof(Detail),new{id=saved}); }
        catch(Exception ex) { var item=FormRecord(form); item["id"]=id; return View(new Screen {Title="Edit batch",Item=item,Choices=await Repo.Choices(Actor),Error=Friendly(ex)}); }
    }
    [HttpPost,Authorize(Roles="Administrator,Coordinator")] public async Task<IActionResult> Assign(long id,long[] trainees)
    {
        try { if(trainees.Length==0) throw new ArgumentException("Select trainees"); await Repo.Execute("CALL schoolnet.assign_trainees(@id,@trainees,@actor)",("id",id),("trainees",trainees),("actor",Actor)); TempData["Success"]="Trainees assigned; daily attendance initialized."; } catch(Exception ex) {TempData["Error"]=Friendly(ex);} return RedirectToAction(nameof(Detail),new{id});
    }
    [HttpPost,Authorize(Roles="Administrator,Coordinator")] public async Task<IActionResult> Remove(long id,long assignment)
    {
        var row=await Repo.One("SELECT id FROM schoolnet.batch_assignment WHERE id=@assignment AND batch_id=@id",("assignment",assignment),("id",id)); if(row is null) return NotFound();
        try {await Repo.Execute("CALL schoolnet.remove_assignment(@assignment,@actor)",("assignment",assignment),("actor",Actor));TempData["Success"]="Assignment removed.";} catch(Exception ex){TempData["Error"]=Friendly(ex);} return RedirectToAction(nameof(Detail),new{id});
    }
    [HttpPost,Authorize(Roles="Administrator,Coordinator")] public async Task<IActionResult> Transition(long id,string status,string reason="")
    {try {await Repo.Execute("CALL schoolnet.transition_batch(@id,@status,@reason,@actor)",("id",id),("status",status),("reason",reason),("actor",Actor));TempData["Success"]="Batch status updated.";} catch(Exception ex){TempData["Error"]=Friendly(ex);} return RedirectToAction(nameof(Detail),new{id});}
}
