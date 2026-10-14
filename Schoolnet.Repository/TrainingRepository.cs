using Npgsql;
using NpgsqlTypes;
using Schoolnet.Domain;

namespace Schoolnet.Repository;

public sealed class TrainingRepository(NpgsqlDataSource source) : ITrainingRepository
{
    private static void Parameters(NpgsqlCommand command, (string Name, object? Value)[] parameters)
    {
        foreach (var (name, value) in parameters)
            command.Parameters.AddWithValue(name, value ?? DBNull.Value);
    }
    public async Task<List<Record>> Query(string sql, params (string Name, object? Value)[] parameters)
    {
        await using var command = source.CreateCommand(sql);
        Parameters(command, parameters);
        await using var reader = await command.ExecuteReaderAsync();
        var rows = new List<Record>();
        while (await reader.ReadAsync())
        {
            var row = new Record();
            for (var i = 0; i < reader.FieldCount; i++) row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
            rows.Add(row);
        }
        return rows;
    }
    public async Task<Record?> One(string sql, params (string Name, object? Value)[] parameters) => (await Query(sql, parameters)).FirstOrDefault();
    public async Task Execute(string sql, params (string Name, object? Value)[] parameters)
    {
        await using var command = source.CreateCommand(sql);
        Parameters(command, parameters);
        await command.ExecuteNonQueryAsync();
    }
    public async Task<long> Save(string routine, long id, string json, long actor)
    {
        // Routine names come exclusively from controllers, never request data.
        if (routine is not ("save_plan" or "save_batch" or "save_trainee_contact") && !routine.StartsWith("master:", StringComparison.Ordinal)) throw new ArgumentException("Unknown routine");
        var sql = routine.StartsWith("master:",StringComparison.Ordinal)
            ? "SELECT schoolnet.save_master(@kind,@id,@data,@actor) AS id"
            : $"SELECT schoolnet.{routine}(@id,@data,@actor) AS id";
        await using var cmd = source.CreateCommand(sql);
        cmd.Parameters.AddWithValue("id",id);
        cmd.Parameters.AddWithValue("data", NpgsqlDbType.Jsonb, json);
        cmd.Parameters.AddWithValue("actor",actor);
        if (routine.StartsWith("master:",StringComparison.Ordinal)) cmd.Parameters.AddWithValue("kind",routine[7..]);
        return Convert.ToInt64(await cmd.ExecuteScalarAsync());
    }
    public async Task<Dictionary<string,List<Record>>> Choices(long actor)
    {
        var choices = new Dictionary<string,List<Record>>
        {
            ["projects"] = await Query("SELECT * FROM schoolnet.project WHERE schoolnet.can_project(@actor,id) ORDER BY name",("actor",actor)),
            ["schools"] = await Query("SELECT s.*, (SELECT jsonb_agg(project_id) FROM schoolnet.project_school WHERE school_id=s.id) projects FROM schoolnet.school_directory s WHERE EXISTS(SELECT 1 FROM schoolnet.project_school ps WHERE ps.school_id=s.id AND schoolnet.can_project(@actor,ps.project_id)) ORDER BY name",("actor",actor)),
            ["plans"] = await Query("SELECT *,training_plan_id id,plan_name name FROM schoolnet.trainingplan WHERE status='Published' ORDER BY plan_name,version_number DESC"),
            ["templates"] = await Query("SELECT * FROM schoolnet.feedback_template ORDER BY name,version_number DESC"),
            ["locations"] = await Query("SELECT l.*,p.name parent FROM schoolnet.location l LEFT JOIN schoolnet.location p ON p.id=l.parent_id ORDER BY l.kind,l.name"),
            ["trainers"] = await Query("SELECT id,name FROM schoolnet.app_user WHERE role='Trainer' AND active ORDER BY name"),
            ["traineeusers"] = await Query("SELECT id,name FROM schoolnet.app_user WHERE role='Trainee' AND active ORDER BY name")
        };
        return choices;
    }
    public async Task Initialize(string script, string adminHash)
    {
        await using var connection = await source.OpenConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await using (var cmd = new NpgsqlCommand(script,connection,transaction)) { cmd.CommandTimeout=120; await cmd.ExecuteNonQueryAsync(); }
        await using (var cmd = new NpgsqlCommand("INSERT INTO schoolnet.app_user(username,name,password_hash,role) SELECT 'admin','Schoolnet Administrator',@hash,'Administrator' WHERE NOT EXISTS(SELECT 1 FROM schoolnet.app_user)",connection,transaction))
        { cmd.Parameters.AddWithValue("hash",adminHash); await cmd.ExecuteNonQueryAsync(); }
        await transaction.CommitAsync();
    }
}
