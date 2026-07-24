namespace BugIssuesTrackerApi.BugIssuesTracker.Data.Models;

using System.ComponentModel.DataAnnotations.Schema;

public class Users
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Initials { get; set; } = "";
    public int RoleId { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("RoleId")]
    public Role? Role { get; set; }
}
