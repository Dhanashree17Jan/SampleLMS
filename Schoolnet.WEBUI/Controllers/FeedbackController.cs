using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using Schoolnet.Domain;
namespace Schoolnet.WEBUI.Controllers;
public sealed class FeedbackController(ITrainingRepository repo) : AppController(repo)
{
    private Task<Record?> Feedback(long assignment,int day) => Repo.One("SELECT a.id,a.batch_id,t.name trainee,t.user_id,b.status,b.start_date,d.day_number,ft.name template,ft.questions,f.answers,f.reason,(b.start_date+d.day_number-1) day_date FROM schoolnet.batch_assignment a JOIN schoolnet.trainee t ON t.id=a.trainee_id JOIN schoolnet.batch b ON b.id=a.batch_id JOIN schoolnet.trainingplandetails d ON d.training_plan_id=b.training_plan_id JOIN schoolnet.feedback_template ft ON ft.id=d.feedback_template_version_id LEFT JOIN schoolnet.feedback_submission f ON f.assignment_id=a.id AND f.training_plan_detail_id=d.training_plan_detail_id WHERE a.id=@assignment AND d.day_number=@day AND d.activity_type='Feedback' AND schoolnet.can_batch(@actor,b.id) AND (t.user_id=@actor OR schoolnet.can_project(@actor,b.project_id))",("assignment",assignment),("day",day),("actor",Actor));
    public async Task<IActionResult> Index(long assignment,int day)
    {
        var feedback=await Feedback(assignment,day); if(feedback is null) return NotFound();
        return View(new Screen {Title=day==1?"Initial feedback":"Final feedback",Item=feedback,Day=day});
    }
    [HttpPost] public async Task<IActionResult> Save(long assignment,int day,IFormCollection form,string reason="")
    {
        var feedback=await Feedback(assignment,day); if(feedback is null) return NotFound();
        var answers=new JsonObject(); foreach(var q in feedback.Array("questions")) {var key=q!["id"]!.ToString(); answers[key]=form[key].ToString();}
        try {await Repo.Execute("CALL schoolnet.submit_feedback(@assignment,@day,@answers::jsonb,@reason,@actor)",("assignment",assignment),("day",day),("answers",answers.ToJsonString()),("reason",reason),("actor",Actor));TempData["Success"]="Feedback submitted.";return RedirectToAction("Detail","Batches",new{id=feedback.Id("batch_id")});}
        catch(Exception ex) {feedback["answers"]=answers.ToJsonString();return View("Index",new Screen {Title="Feedback",Item=feedback,Day=day,Error=Friendly(ex)});}
    }
}
