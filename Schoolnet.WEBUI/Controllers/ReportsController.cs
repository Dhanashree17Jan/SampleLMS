using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schoolnet.Domain;
namespace Schoolnet.WEBUI.Controllers;
public sealed class ReportsController(ITrainingRepository repo) : AppController(repo)
{
    public async Task<IActionResult> Index(long project=0,long school=0,long batch=0,long plan=0,string from="",string to="",string trainee="",bool export=false)
    {
        var rows=await Repo.Query("SELECT b.id batch_id,b.code batch_code,b.name batch,b.project,b.school,b.start_date,b.end_date,b.plan_name,b.version_number,b.number_of_days,t.username,t.name trainee,d.day_number,d.title activity,(b.start_date+d.day_number-1) day_date,at.status attendance,at.remarks,CASE WHEN f.id IS NOT NULL THEN 'Submitted' WHEN d.activity_type='Feedback' THEN 'Missing' ELSE 'Not applicable' END feedback FROM schoolnet.batch_directory b JOIN schoolnet.batch_assignment a ON a.batch_id=b.id JOIN schoolnet.trainee t ON t.id=a.trainee_id JOIN schoolnet.attendance at ON at.assignment_id=a.id JOIN schoolnet.trainingplandetails d ON d.training_plan_detail_id=at.training_plan_detail_id LEFT JOIN schoolnet.feedback_submission f ON f.assignment_id=a.id AND f.training_plan_detail_id=d.training_plan_detail_id WHERE schoolnet.can_batch(@actor,b.id) AND (@staff OR t.user_id=@actor) AND (@project=0 OR b.project_id=@project) AND (@school=0 OR b.school_id=@school) AND (@batch=0 OR b.id=@batch) AND (@plan=0 OR b.training_plan_id=@plan) AND (t.name ILIKE @trainee OR t.username ILIKE @trainee) ORDER BY b.start_date DESC,b.id,t.name,d.day_number",("actor",Actor),("staff",Role!="Trainee"),("project",project),("school",school),("batch",batch),("plan",plan),("trainee","%"+trainee+"%"));
        if(DateOnly.TryParse(from,out var fromDate)) rows=rows.Where(r=>DateOnly.Parse(r.Text("day_date"),CultureInfo.InvariantCulture)>=fromDate).ToList();
        if(DateOnly.TryParse(to,out var toDate)) rows=rows.Where(r=>DateOnly.Parse(r.Text("day_date"),CultureInfo.InvariantCulture)<=toDate).ToList();
        if(export)
        {
            var columns=new[] {"batch_code","batch","project","school","start_date","end_date","plan_name","version_number","number_of_days","username","trainee","day_number","activity","day_date","attendance","remarks","feedback"};
            static string Cell(string s) { if(s.TrimStart().StartsWith('=') || s.TrimStart().StartsWith('+') || s.TrimStart().StartsWith('-') || s.TrimStart().StartsWith('@')) s="'"+s; return "\""+s.Replace("\"","\"\"")+"\""; }
            var csv=new StringBuilder().AppendLine(string.Join(',',columns)); foreach(var row in rows) csv.AppendLine(string.Join(',',columns.Select(c=>Cell(row.Text(c)))));
            return File(Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray(),"text/csv","schoolnet-training-report.csv");
        }
        var choices=await Repo.Choices(Actor); choices["batches"]=await Repo.Query("SELECT id,name FROM schoolnet.batch WHERE schoolnet.can_batch(@actor,id) ORDER BY name",("actor",Actor));
        choices["plans"]=await Repo.Query("SELECT DISTINCT p.training_plan_id id,p.plan_name name,p.version_number FROM schoolnet.trainingplan p JOIN schoolnet.batch b ON b.training_plan_id=p.training_plan_id WHERE schoolnet.can_batch(@actor,b.id) ORDER BY p.plan_name",("actor",Actor));
        return View(new Screen {Title="Reports",Rows=rows,Choices=choices,Filters=new() { ["project"]=project.ToString(),["school"]=school.ToString(),["batch"]=batch.ToString(),["plan"]=plan.ToString(),["from"]=from,["to"]=to,["trainee"]=trainee }});
    }
    [Authorize(Roles="Administrator")] public async Task<IActionResult> Audit() => View(new Screen {Title="Audit history",Rows=await Repo.Query("SELECT a.*,u.name actor FROM schoolnet.audit_log a LEFT JOIN schoolnet.app_user u ON u.id=a.actor_id ORDER BY a.id DESC LIMIT 300")});
}
