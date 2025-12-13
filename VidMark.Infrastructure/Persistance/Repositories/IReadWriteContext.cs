using Microsoft.EntityFrameworkCore;
using VidMark.Domain.Models;

namespace VidMark.Infrastructure.Persistance.Repositories;

public interface IReadWriteContext
{
    DbSet<Project> Projects { get; }
    DbSet<Video> Videos { get; }
    DbSet<Subject> Subjects { get; }
    DbSet<AssignedLabel> AssignedLabels { get; }
    DbSet<Label> Labels { get; }
    DbSet<SubjectVideoGroupAssignment> SubjectVideoGroupAssignments { get; }
    DbSet<VideoGroup> VideoGroups { get; }
    DbSet<ProjectAccessCode> ProjectAccessCodes { get; }
    DbSet<GeneratedReport> GeneratedReports { get; }
    DbSet<User> Users { get; }
    DbSet<SubjectVideoGroupAssignmentCompletion> AssignmentCompletions { get; }
}
