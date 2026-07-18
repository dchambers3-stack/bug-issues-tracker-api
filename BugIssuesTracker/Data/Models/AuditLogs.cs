using System.ComponentModel.DataAnnotations.Schema;

namespace BugIssuesTrackerApi.BugIssuesTracker.Data.Models;

public class AuditLogs
{
    public int Id { get; set; }
    public string EntityType { get; set; } = "";
    public int EntityId { get; set; }
    public string Action { get; set; } = "";
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public int ChangedBy { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("ChangedBy")]
    public Users? ChangedByUser { get; set; }
}
