using System.ComponentModel.DataAnnotations.Schema;

namespace BugIssuesTrackerApi.BugIssuesTracker.Data.Models;

public class Issues
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public int PriorityId { get; set; }
    public int StatusId { get; set; }
    public int IssueTypeId { get; set; }
    public int? AssignedUserId { get; set; }
    public int ReporterUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("ProjectId")]
    public Projects? Project { get; set; }

    [ForeignKey("PriorityId")]
    public Priorities? Priority { get; set; }

    [ForeignKey("StatusId")]
    public Statuses? Status { get; set; }

    [ForeignKey("IssueTypeId")]
    public IssueTypes? IssueType { get; set; }

    [ForeignKey("AssignedUserId")]
    public Users? AssignedUser { get; set; }

    [ForeignKey("ReporterUserId")]
    public Users? ReporterUser { get; set; }
}
