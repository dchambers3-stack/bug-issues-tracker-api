namespace BugIssuesTrackerApi.BugIssuesTracker.Data.Dtos;

public class CurrentUserDto
{
    public int UserId { get; set; }
    public string? Username { get; set; }
    public string? Role { get; set; }
}
