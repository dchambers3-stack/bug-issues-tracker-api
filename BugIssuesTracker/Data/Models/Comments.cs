using System.ComponentModel.DataAnnotations.Schema;

namespace BugIssuesTrackerApi.BugIssuesTracker.Data.Models;

public class Comments
{
    public int Id { get; set; }
    public int IssueId { get; set; }
    public int UserId { get; set; }
    public string Content { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("IssueId")]
    public Issues? Issue { get; set; }

    [ForeignKey("UserId")]
    public Users? User { get; set; }
}
