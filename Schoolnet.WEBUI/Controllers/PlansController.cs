using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schoolnet.Domain;
namespace Schoolnet.WEBUI.Controllers;
[Authorize(Roles="Administrator,Coordinator,Trainer")]
public sealed class PlansController(ITrainingRepository repo) : AppController(repo)
{
    public async Task<IActionResult> Index(string search="") => View(new Screen { Title="Training plans",Rows=await Repo.Query("SELECT p.*,p.training_plan_id id,(SELECT count(*) FROM schoolnet.batch b WHERE b.training_plan_id=p.training_plan_id AND schoolnet.can_batch(@actor,b.id)) batches FROM schoolnet.trainingplan p WHERE (@admin OR status<>'Draft') AND (plan_name ILIKE @search OR plan_code ILIKE @search) ORDER BY plan_code,version_number DESC",("actor",Actor),("admin",Role=="Administrator"),("search","%"+search+"%")) });
    public async Task<IActionResult> Detail(long id)
    {
        var p=await Repo.One("SELECT *,training_plan_id id FROM schoolnet.trainingplan WHERE training_plan_id=@id AND (@admin OR status<>'Draft')",("id",id),("admin",Role=="Administrator"));
        if(p is null) return NotFound();
        return View(new Screen { Title=p.Text("plan_name"),Item=p,Days=await Repo.Query("SELECT * FROM schoolnet.trainingplandetails WHERE training_plan_id=@id ORDER BY day_number",("id",id)),Rows=await Repo.Query("SELECT * FROM schoolnet.batch_directory WHERE training_plan_id=@id AND schoolnet.can_batch(@actor,id)",("id",id),("actor",Actor)) });
    }
    [Authorize(Roles="Administrator")] public async Task<IActionResult> Edit(long id=0)
    {
        var p=id==0?new Record { ["number_of_days"]=4 }:await Repo.One("SELECT *,training_plan_id id FROM schoolnet.trainingplan WHERE training_plan_id=@id AND status='Draft'",("id",id));
        if(p is null) return NotFound();
        return View(new Screen { Title=id==0?"Create training plan":"Edit training plan",Item=p,Choices=await Repo.Choices(Actor),Days=id==0?[]:await Repo.Query("SELECT * FROM schoolnet.trainingplandetails WHERE training_plan_id=@id ORDER BY day_number",("id",id)) });
    }
    [HttpPost,Authorize(Roles="Administrator")] public async Task<IActionResult> Edit(long id,IFormCollection form)
    {
        try
        {
            var data=Data(form); RequireFields(data,"plan_code","plan_name","number_of_days");
            if(!int.TryParse(form["number_of_days"],out var duration) || duration is <2 or >365)
                throw new ArgumentException("Number of days must be between 2 and 365.");
            if(new[]{"day_title","day_topics","day_instructions","day_template","day_duration"}.Any(key=>form[key].Count!=duration))
                throw new ArgumentException("Provide exactly one activity for each training day.");
            var days=new JsonArray();
            for(var i=0;i<form["day_title"].Count;i++) days.Add(new JsonObject { ["title"]=form["day_title"][i],["topics"]=form["day_topics"].ElementAtOrDefault(i)??"",["instructions"]=form["day_instructions"].ElementAtOrDefault(i)??"",["feedback_template_version_id"]=form["day_template"].ElementAtOrDefault(i)??"",["duration_minutes"]=form["day_duration"].ElementAtOrDefault(i)??"" });
            data["days"]=days;
            var saved=await Repo.Save("save_plan",id,data.ToJsonString(),Actor); TempData["Success"]="Draft plan saved."; return RedirectToAction(nameof(Detail),new{id=saved});
        }
        catch(Exception ex)
        {
            var item=FormRecord(form); item["id"]=id; var rows=new List<Record>();
            item["number_of_days"]=int.TryParse(form["number_of_days"],out var submittedDays) && submittedDays is >=2 and <=365 ? submittedDays : 4;
            for(var i=0;i<form["day_title"].Count;i++) rows.Add(new Record { ["day_number"]=i+1,["title"]=form["day_title"][i],["topics"]=form["day_topics"].ElementAtOrDefault(i),["instructions"]=form["day_instructions"].ElementAtOrDefault(i),["feedback_template_version_id"]=form["day_template"].ElementAtOrDefault(i),["duration_minutes"]=form["day_duration"].ElementAtOrDefault(i) });
            return View(new Screen {Title="Edit training plan",Item=item,Days=rows,Choices=await Repo.Choices(Actor),Error=ex is ArgumentException ? ex.Message : Friendly(ex)});
        }
    }
    [HttpPost,Authorize(Roles="Administrator")] public async Task<IActionResult> Action(long id,string operation)
    {
        try
        {
            switch(operation)
            {
                case "publish": await Repo.Execute("CALL schoolnet.publish_plan(@id,@actor)",("id",id),("actor",Actor)); break;
                case "archive": await Repo.Execute("CALL schoolnet.archive_plan(@id,@actor)",("id",id),("actor",Actor)); break;
                case "revise": var next=await Repo.One("SELECT schoolnet.revise_plan(@id,@actor) id",("id",id),("actor",Actor)); return RedirectToAction(nameof(Edit),new{id=next!.Id()});
                default: return BadRequest();
            }
            TempData["Success"]="Plan updated.";
        } catch(Exception ex) { TempData["Error"]=Friendly(ex); }
        return RedirectToAction(nameof(Detail),new{id});
    }
}
