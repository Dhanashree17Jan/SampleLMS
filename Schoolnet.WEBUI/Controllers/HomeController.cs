using Microsoft.AspNetCore.Mvc;
using Schoolnet.Domain;
namespace Schoolnet.WEBUI.Controllers;
public sealed class HomeController(ITrainingRepository repo) : AppController(repo)
{
    public async Task<IActionResult> Index(long project=0,long state=0,long school=0)
    {
        var screen=new Screen { Title="Dashboard",Choices=await Repo.Choices(Actor) };
        screen.Filters=new() { ["project"]=project.ToString(),["state"]=state.ToString(),["school"]=school.ToString() };
        screen.Rows=await Repo.Query("SELECT * FROM schoolnet.batch_directory WHERE schoolnet.can_batch(@actor,id) AND (@project=0 OR project_id=@project) AND (@state=0 OR state_id=@state) AND (@school=0 OR school_id=@school) ORDER BY start_date DESC,id DESC",("actor",Actor),("project",project),("state",state),("school",school));
        screen.Item=(await Repo.One("SELECT count(*) count FROM schoolnet.trainee t JOIN schoolnet.school_directory s ON s.id=t.school_id WHERE t.active AND (schoolnet.can_project(@actor,t.project_id) OR t.user_id=@actor OR EXISTS(SELECT 1 FROM schoolnet.batch_assignment a JOIN schoolnet.batch b ON b.id=a.batch_id WHERE a.trainee_id=t.id AND b.trainer_id=@actor AND schoolnet.actor_role(@actor)='Trainer')) AND (@project=0 OR t.project_id=@project) AND (@state=0 OR s.state_id=@state) AND (@school=0 OR t.school_id=@school)",("actor",Actor),("project",project),("state",state),("school",school)))!;
        return View(screen);
    }
    public IActionResult Error() { Response.StatusCode=500; return View(); }
}
