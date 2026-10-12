using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schoolnet.Domain;
namespace Schoolnet.WEBUI.Controllers;
[Authorize(Roles="Administrator,Coordinator,Trainer")]
public sealed class AttendanceController(ITrainingRepository repo) : AppController(repo)
{
    public async Task<IActionResult> Index(long id,int day=1)
    {
        var batch=await Batch(id); if(batch is null || day<1 || day>batch.Id("number_of_days")) return NotFound();
        var rows=await Repo.Query("SELECT a.*,t.name,t.username FROM schoolnet.attendance a JOIN schoolnet.batch_assignment ba ON ba.id=a.assignment_id JOIN schoolnet.trainee t ON t.id=ba.trainee_id JOIN schoolnet.trainingplandetails d ON d.training_plan_detail_id=a.training_plan_detail_id WHERE ba.batch_id=@id AND d.day_number=@day ORDER BY t.name",("id",id),("day",day));
        return View(new Screen {Title="Daily attendance",Item=batch,Day=day,Rows=rows,Attendance=AttendanceAvailability.For(batch,day,Role,DateTime.UtcNow)});
    }
    [HttpPost] public async Task<IActionResult> Save(long id,int day,IFormCollection form,string reason="")
    {
        var batch=await Batch(id);
        if(batch is null || day<1 || day>batch.Id("number_of_days")) return NotFound();
        var availability=AttendanceAvailability.For(batch,day,Role,DateTime.UtcNow);
        if(!availability.CanMark)
        {
            TempData["Error"]=availability.Message;
            return RedirectToAction(nameof(Index),new{id,day});
        }
        try
        {
            var rows=new JsonArray();
            for(var i=0;i<form["attendance_id"].Count;i++) rows.Add(new JsonObject { ["id"]=form["attendance_id"][i],["revision"]=form["revision"].ElementAtOrDefault(i),["status"]=form["status"].ElementAtOrDefault(i),["remarks"]=form["remarks"].ElementAtOrDefault(i) });
            await Repo.Execute("CALL schoolnet.save_attendance(@id,@day,@rows::jsonb,@reason,@actor)",("id",id),("day",day),("rows",rows.ToJsonString()),("reason",reason),("actor",Actor)); TempData["Success"]="Attendance saved.";
        } catch(Exception ex){TempData["Error"]=Friendly(ex);} return RedirectToAction(nameof(Index),new{id,day});
    }
}
