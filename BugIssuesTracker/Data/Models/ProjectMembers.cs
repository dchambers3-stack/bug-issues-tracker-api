using System.ComponentModel.DataAnnotations.Schema;

namespace BugIssuesTrackerApi.BugIssuesTracker.Data.Models;

public class ProjectMembers // Many to many relationship
{
    public int ProjectId { get; set; }
    public int UserId { get; set; }

    [ForeignKey("ProjectId")]
    public Projects? Project { get; set; }

    [ForeignKey("UserId")]
    public Users? User { get; set; }
}
