namespace BugIssuesTrackerApi.BugIssuesTracker.Data.Dtos;

public class CurrentUserDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int UserId { get; set; }
    public string? Username { get; set; }
    public string? Role { get; set; }
}
