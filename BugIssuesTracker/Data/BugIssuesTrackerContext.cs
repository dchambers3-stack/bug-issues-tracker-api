using System.Net.Mail;
using BugIssuesTrackerApi.BugIssuesTracker.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BugIssuesTrackerApi.BugIssuesTracker.Data
{
    public class BugIssuesTrackerContext : DbContext
    {
        public BugIssuesTrackerContext(DbContextOptions<BugIssuesTrackerContext> options)
            : base(options) { }

        public DbSet<Attachments> Attachments => Set<Attachments>();
        public DbSet<AuditLogs> AuditLogs => Set<AuditLogs>();
        public DbSet<Comments> Comments => Set<Comments>();
        public DbSet<Issues> Issues => Set<Issues>();
        public DbSet<IssueTypes> IssueTypes => Set<IssueTypes>();
        public DbSet<Priorities> Priorities => Set<Priorities>();
        public DbSet<ProjectMembers> ProjectMembers => Set<ProjectMembers>();
        public DbSet<Projects> Projects => Set<Projects>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Statuses> Statuses => Set<Statuses>();
        public DbSet<Users> Users => Set<Users>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProjectMembers>().HasKey(pm => new { pm.ProjectId, pm.UserId });

            modelBuilder
                .Entity<Role>()
                .HasData(
                    new Role { Id = (int)UserRole.Admin, Name = nameof(UserRole.Admin) },
                    new Role { Id = (int)UserRole.Developer, Name = nameof(UserRole.Developer) },
                    new Role { Id = (int)UserRole.Reporter, Name = nameof(UserRole.Reporter) }
                );
        }
    }
}
