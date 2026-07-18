using System.ComponentModel.DataAnnotations.Schema;

namespace BugIssuesTrackerApi.BugIssuesTracker.Data.Models;

public class Attachments
{
    public int Id { get; set; }
    public int IssueId { get; set; }
    public string FilePath { get; set; } = "";
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("IssueId")]
    public Issues? Issue { get; set; }
}
