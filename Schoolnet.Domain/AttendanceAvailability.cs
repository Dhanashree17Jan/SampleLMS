namespace Schoolnet.Domain;

public sealed record AttendanceAvailability(DateOnly Date, bool CanMark, string Message)
{
    public static AttendanceAvailability For(Record batch, int day, string role, DateTime utcNow)
    {
        var date = DateOnly.Parse(batch.Text("start_date")).AddDays(day - 1);
        var today = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(utcNow,
            TimeZoneInfo.FindSystemTimeZoneById("Asia/Kolkata")));
        if (date > today)
            return new(date, false, $"Attendance cannot be marked for a future date. Day {day} becomes available on {date:dd MMM yyyy} (India time).");
        if (batch.Text("status") is "Draft" or "Cancelled")
            return new(date, false, "Attendance is read-only for Draft or Cancelled batches.");
        if (batch.Text("status") == "Completed" && role != "Administrator")
            return new(date, false, "Completed batch corrections require an administrator.");
        return new(date, true, "");
    }
}
