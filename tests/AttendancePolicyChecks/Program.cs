using Schoolnet.Domain;

var batch = new Record { ["start_date"] = "2026-10-13", ["status"] = "Scheduled" };
var beforeMidnight = new DateTime(2026, 10, 12, 18, 29, 59, DateTimeKind.Utc);
var atMidnight = beforeMidnight.AddSeconds(1);
var checks = 0;
void Check(bool condition, string label)
{
    if (!condition) throw new InvalidOperationException(label);
    Console.WriteLine("PASS: " + label);
    checks++;
}
foreach (var role in new[] { "Administrator", "Coordinator", "Trainer" })
{
    var future = AttendanceAvailability.For(batch, 1, role, beforeMidnight);
    Check(!future.CanMark && future.Date == new DateOnly(2026, 10, 13), role + " cannot mark tomorrow just before India midnight");
    Check(AttendanceAvailability.For(batch, 1, role, atMidnight).CanMark, role + " can mark today at India midnight while UTC is still yesterday");
    Check(!AttendanceAvailability.For(batch, 2, role, atMidnight).CanMark, role + " cannot mark tomorrow in a batch that starts today");
}
batch["status"] = "Completed";
Check(AttendanceAvailability.For(batch, 1, "Administrator", atMidnight).CanMark, "Administrator retains completed batch corrections for available dates");
Check(!AttendanceAvailability.For(batch, 1, "Trainer", atMidnight).CanMark, "Trainer cannot correct completed batch attendance");
Check(!AttendanceAvailability.For(batch, 2, "Administrator", atMidnight).CanMark, "Completed status does not bypass future-date restriction");
batch["status"] = "Draft";
Check(!AttendanceAvailability.For(batch, 1, "Administrator", atMidnight).CanMark, "Draft batch remains read-only");
batch["status"] = "Cancelled";
Check(!AttendanceAvailability.For(batch, 1, "Administrator", atMidnight).CanMark, "Cancelled batch remains read-only");
Console.WriteLine($"{checks} attendance policy checks passed.");
