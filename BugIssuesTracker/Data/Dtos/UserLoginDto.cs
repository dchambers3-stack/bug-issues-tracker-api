namespace BugIssuesTrackerApi.BugIssuesTracker.Data.Dtos;

public class UserLoginDto
{
    public string? UsernameOrEmail { get; set; }
    public string? Password { get; set; }
}
