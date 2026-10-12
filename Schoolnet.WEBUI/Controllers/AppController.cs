using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Schoolnet.Domain;

namespace Schoolnet.WEBUI.Controllers;

[Authorize]
public abstract class AppController(ITrainingRepository repository) : Controller
{
    protected ITrainingRepository Repo => repository;
    protected long Actor => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    protected string Role => User.FindFirstValue(ClaimTypes.Role)!;
    protected bool Staff => Role is "Administrator" or "Coordinator";
    protected static string Friendly(Exception exception) => exception is PostgresException db ? db.SqlState switch
    {
        "23505"=>"This record already exists. Codes, usernames and assignments must be unique.",
        "23503"=>"This record is linked to another record. Check your selections.",
        "23514"=>"A field does not meet the required format or business rule.",
        "23502"=>"Complete all required fields.",
        "P0001"=>db.MessageText,
        _=>"The change could not be saved. Check your entries and try again."
    } : exception is ArgumentException or FormatException or JsonException ? "Check the required fields and values." : "The change could not be saved. Please try again.";
    protected static JsonObject Data(IFormCollection form)
    {
        var data=new JsonObject();
        foreach(var key in form.Keys.Where(k=>k!="__RequestVerificationToken")) data[key]=form[key].FirstOrDefault()??"";
        data["active"]=form["active"].Contains("true");
        data["projects"]=new JsonArray(form["projects"].Where(v=>!string.IsNullOrWhiteSpace(v)).Select(v=>(JsonNode?)JsonValue.Create(long.Parse(v!))).ToArray());
        return data;
    }
    protected static Record FormRecord(IFormCollection form)
    {
        var item=new Record(); foreach(var key in form.Keys) item[key]=form[key].FirstOrDefault()??"";
        item["active"]=form["active"].Contains("true");
        item["projects"]=JsonSerializer.Serialize(form["projects"].Where(v=>long.TryParse(v,out _)).Select(v=>long.Parse(v!)));
        return item;
    }
    protected void RequireFields(JsonObject data,params string[] fields)
    {
        foreach(var field in fields) if(string.IsNullOrWhiteSpace(data[field]?.ToString())) throw new ArgumentException(field);
    }
    protected async Task<Record?> Batch(long id) => await Repo.One("SELECT * FROM schoolnet.batch_directory WHERE id=@id AND schoolnet.can_batch(@actor,id)",("id",id),("actor",Actor));
}
