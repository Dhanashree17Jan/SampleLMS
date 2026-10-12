using System.Globalization;
using System.Text.Json.Nodes;

namespace Schoolnet.Domain;

// Database read models retain field names; typed accessors are used by Razor and controllers.
public sealed class Record : Dictionary<string, object?>
{
    public string Text(string key) => TryGetValue(key, out var value) ? value switch
    {
        null => "", DateTime date => date.ToString(key.EndsWith("_at", StringComparison.Ordinal) ? "O" : "yyyy-MM-dd", CultureInfo.InvariantCulture),
        DateOnly date => date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
        bool flag => flag ? "true" : "false", _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? ""
    } : "";
    public long Id(string key = "id") => long.TryParse(Text(key), out var id) ? id : 0;
    public bool Flag(string key = "active") => Text(key) == "true";
    public JsonArray Array(string key) => JsonNode.Parse(string.IsNullOrWhiteSpace(Text(key)) ? "[]" : Text(key))?.AsArray() ?? [];
}
public sealed record UserSession(long Id, string Name, string Username, string Role);
public sealed class Screen
{
    public string Title { get; set; } = "";
    public string Kind { get; set; } = "";
    public Record Item { get; set; } = new();
    public List<Record> Rows { get; set; } = [];
    public List<Record> Days { get; set; } = [];
    public List<Record> Roster { get; set; } = [];
    public List<Record> Eligible { get; set; } = [];
    public Dictionary<string,List<Record>> Choices { get; set; } = new();
    public string Error { get; set; } = "";
    public int Day { get; set; } = 1;
    public AttendanceAvailability? Attendance { get; set; }
    public Dictionary<string,string> Filters { get; set; } = new();
    public List<Record> Options(string key) => Choices.GetValueOrDefault(key) ?? [];
}
public interface ITrainingRepository
{
    Task<List<Record>> Query(string sql, params (string Name, object? Value)[] parameters);
    Task<Record?> One(string sql, params (string Name, object? Value)[] parameters);
    Task Execute(string sql, params (string Name, object? Value)[] parameters);
    Task<long> Save(string routine, long id, string json, long actor);
    Task<Dictionary<string,List<Record>>> Choices(long actor);
    Task Initialize(string script, string adminHash);
}
