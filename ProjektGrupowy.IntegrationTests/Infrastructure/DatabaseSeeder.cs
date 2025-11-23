using Microsoft.EntityFrameworkCore;
using ProjektGrupowy.Domain.Enums;
using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Infrastructure.Persistance;

namespace ProjektGrupowy.IntegrationTests.Infrastructure;

public static class DatabaseSeeder
{
    public static async Task<Project> SeedProjectAsync(AppDbContext context, string? createdById = null)
    {
        var actualCreatedById = createdById ?? "test-scientist-id";

        await EnsureUserExistsAsync(context, actualCreatedById);

        var project = new Project
        {
            Name = $"Test Project {Guid.NewGuid()}",
            Description = "Test project description",
            CreatedById = actualCreatedById,
            CreationDate = DateOnly.FromDateTime(DateTime.Today)
        };

        context.Projects.Add(project);
        await context.SaveChangesAsync();
        return project;
    }

    public static async Task EnsureUserExistsAsync(AppDbContext context, string userId)
    {
        var userExists = await context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
        {
            var user = new User
            {
                Id = userId,
                UserName = $"testuser_{userId}",
                Email = $"{userId}@test.com",
                Enabled = true,
                CreatedTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }
    }

    public static async Task<Subject> SeedSubjectAsync(AppDbContext context, int? projectId = null, string? createdById = null)
    {
        Project project;
        if (projectId == null)
        {
            project = await SeedProjectAsync(context, createdById);
        }
        else
        {
            project = await context.Projects.FindAsync(projectId)
                ?? throw new InvalidOperationException($"Project with ID {projectId} not found");
        }

        var subject = Subject.Create(
            name: $"Test Subject {Guid.NewGuid()}",
            description: "Test subject description",
            project: project,
            createdById: createdById ?? "test-scientist-id"
        );

        context.Subjects.Add(subject);
        await context.SaveChangesAsync();

        await context.Entry(subject).Reference(s => s.Project).LoadAsync();
        return subject;
    }

    public static async Task<Label> SeedLabelAsync(AppDbContext context, int? subjectId = null, string? createdById = null)
    {
        Subject subject;
        if (subjectId == null)
        {
            subject = await SeedSubjectAsync(context, null, createdById);
        }
        else
        {
            subject = await context.Subjects.FindAsync(subjectId)
                ?? throw new InvalidOperationException($"Subject with ID {subjectId} not found");
        }

        var label = Label.Create(
            name: $"Test Label {Guid.NewGuid()}",
            colorHex: "#FF0000",
            type: "Regular",
            shortcut: 'T',
            subject: subject,
            createdById: createdById ?? "test-scientist-id"
        );

        context.Labels.Add(label);
        await context.SaveChangesAsync();

        await context.Entry(label).Reference(l => l.Subject).LoadAsync();
        return label;
    }

    public static async Task<VideoGroup> SeedVideoGroupAsync(AppDbContext context, int? projectId = null, string? createdById = null)
    {
        Project project;
        if (projectId == null)
        {
            project = await SeedProjectAsync(context, createdById);
        }
        else
        {
            project = await context.Projects.FindAsync(projectId)
                ?? throw new InvalidOperationException($"Project with ID {projectId} not found");
        }

        var videoGroup = VideoGroup.Create(
            name: $"Test Video Group {Guid.NewGuid()}",
            description: "Test video group description",
            project: project,
            createdById: createdById ?? "test-scientist-id"
        );

        context.VideoGroups.Add(videoGroup);
        await context.SaveChangesAsync();

        await context.Entry(videoGroup).Reference(vg => vg.Project).LoadAsync();
        return videoGroup;
    }

    public static async Task<Video> SeedVideoAsync(AppDbContext context, int? videoGroupId = null, string? createdById = null)
    {
        VideoGroup videoGroup;
        if (videoGroupId == null)
        {
            videoGroup = await SeedVideoGroupAsync(context, null, createdById);
        }
        else
        {
            videoGroup = await context.VideoGroups.FindAsync(videoGroupId)
                ?? throw new InvalidOperationException($"VideoGroup with ID {videoGroupId} not found");
        }

        var video = Video.Create(
            title: $"Test Video {Guid.NewGuid()}",
            path: $"/videos/test-video-{Guid.NewGuid()}.mp4",
            videoGroup: videoGroup,
            contentType: "video/mp4",
            positionInQueue: 1,
            createdById: createdById ?? "test-scientist-id"
        );

        context.Videos.Add(video);
        await context.SaveChangesAsync();
        return video;
    }

    public static async Task<SubjectVideoGroupAssignment> SeedAssignmentAsync(
        AppDbContext context,
        int? subjectId = null,
        int? videoGroupId = null,
        string? createdById = null)
    {
        Subject subject;
        VideoGroup videoGroup;
        var actualCreatedById = createdById ?? "test-scientist-id";

        if (subjectId == null || videoGroupId == null)
        {
            var project = await SeedProjectAsync(context, actualCreatedById);

            if (subjectId == null)
            {
                subject = await SeedSubjectAsync(context, project.Id, actualCreatedById);
            }
            else
            {
                subject = await context.Subjects.FindAsync(subjectId)
                    ?? throw new InvalidOperationException($"Subject with ID {subjectId} not found");
            }

            if (videoGroupId == null)
            {
                videoGroup = await SeedVideoGroupAsync(context, project.Id, actualCreatedById);
            }
            else
            {
                videoGroup = await context.VideoGroups.FindAsync(videoGroupId)
                    ?? throw new InvalidOperationException($"VideoGroup with ID {videoGroupId} not found");
            }
        }
        else
        {
            subject = await context.Subjects.FindAsync(subjectId)
                ?? throw new InvalidOperationException($"Subject with ID {subjectId} not found");
            videoGroup = await context.VideoGroups.FindAsync(videoGroupId)
                ?? throw new InvalidOperationException($"VideoGroup with ID {videoGroupId} not found");
        }

        var assignment = SubjectVideoGroupAssignment.Create(
            subject: subject,
            videoGroup: videoGroup,
            createdById: actualCreatedById
        );

        context.SubjectVideoGroupAssignments.Add(assignment);
        await context.SaveChangesAsync();
        return assignment;
    }

    public static async Task<AssignedLabel> SeedAssignedLabelAsync(
        AppDbContext context,
        int? labelId = null,
        int? videoId = null,
        string? createdById = null)
    {
        Label label;
        Video video;
        var actualCreatedById = createdById ?? "test-labeler-id";

        if (labelId == null)
        {
            label = await SeedLabelAsync(context, null, actualCreatedById);
        }
        else
        {
            label = await context.Labels.FindAsync(labelId)
                ?? throw new InvalidOperationException($"Label with ID {labelId} not found");
        }

        if (videoId == null)
        {
            video = await SeedVideoAsync(context, null, actualCreatedById);
        }
        else
        {
            video = await context.Videos.FindAsync(videoId)
                ?? throw new InvalidOperationException($"Video with ID {videoId} not found");
        }

        var assignedLabel = AssignedLabel.Create(
            label: label,
            userId: actualCreatedById,
            video: video,
            start: "10.5",
            end: "20.3"
        );

        context.AssignedLabels.Add(assignedLabel);
        await context.SaveChangesAsync();
        return assignedLabel;
    }

    public static async Task<ProjectAccessCode> SeedAccessCodeAsync(AppDbContext context, int? projectId = null, string? userId = null)
    {
        Project project;
        if (projectId == null)
        {
            var scientistId = userId ?? Guid.NewGuid().ToString();
            project = await SeedProjectAsync(context, scientistId);
        }
        else
        {
            project = await context.Projects.FindAsync(projectId)
                ?? throw new InvalidOperationException($"Project with ID {projectId} not found");
        }

        var createdById = userId ?? project.CreatedById;
        var accessCode = ProjectAccessCode.Create(
            project,
            AccessCodeExpiration.In14Days,
            createdById
        );

        context.ProjectAccessCodes.Add(accessCode);
        await context.SaveChangesAsync();
        return accessCode;
    }

    public static async Task<GeneratedReport> SeedGeneratedReportAsync(AppDbContext context, int? projectId = null, string? userId = null)
    {
        Project project;
        if (projectId == null)
        {
            var scientistId = userId ?? Guid.NewGuid().ToString();
            project = await SeedProjectAsync(context, scientistId);
        }
        else
        {
            project = await context.Projects.FindAsync(projectId)
                ?? throw new InvalidOperationException($"Project with ID {projectId} not found");
        }

        var createdById = userId ?? project.CreatedById;
        var reportName = $"Report-{Guid.NewGuid().ToString()[..8]}";
        var reportPath = $"/reports/{reportName}.json";

        var report = GeneratedReport.Create(reportName, reportPath, project, createdById);

        context.GeneratedReports.Add(report);
        await context.SaveChangesAsync();
        return report;
    }

    public static async Task<SubjectVideoGroupAssignment> SeedSubjectVideoGroupAssignmentAsync(
        AppDbContext context,
        int? subjectId = null,
        int? videoGroupId = null,
        string? userId = null)
    {
        Subject subject;
        VideoGroup videoGroup;

        if (subjectId == null)
        {
            subject = await SeedSubjectAsync(context, null, userId);
        }
        else
        {
            subject = await context.Subjects.FindAsync(subjectId)
                ?? throw new InvalidOperationException($"Subject with ID {subjectId} not found");
        }

        if (videoGroupId == null)
        {
            // Use the same project as the subject
            var project = await context.Projects.FindAsync(subject.Project.Id);
            videoGroup = await SeedVideoGroupAsync(context, project!.Id, userId);
        }
        else
        {
            videoGroup = await context.VideoGroups.FindAsync(videoGroupId)
                ?? throw new InvalidOperationException($"VideoGroup with ID {videoGroupId} not found");
        }

        var createdById = userId ?? subject.CreatedById;
        var assignment = SubjectVideoGroupAssignment.Create(subject, videoGroup, createdById);

        context.SubjectVideoGroupAssignments.Add(assignment);
        await context.SaveChangesAsync();
        return assignment;
    }

    public static async Task ClearDatabaseAsync(AppDbContext context)
    {
        // Clear all entities in proper order to avoid FK constraints
        context.AssignedLabels.RemoveRange(context.AssignedLabels.IgnoreQueryFilters());
        context.SubjectVideoGroupAssignments.RemoveRange(context.SubjectVideoGroupAssignments.IgnoreQueryFilters());
        context.Videos.RemoveRange(context.Videos.IgnoreQueryFilters());
        context.VideoGroups.RemoveRange(context.VideoGroups.IgnoreQueryFilters());
        context.Labels.RemoveRange(context.Labels.IgnoreQueryFilters());
        context.Subjects.RemoveRange(context.Subjects.IgnoreQueryFilters());
        context.Projects.RemoveRange(context.Projects.IgnoreQueryFilters());
        context.ProjectAccessCodes.RemoveRange(context.ProjectAccessCodes.IgnoreQueryFilters());
        context.GeneratedReports.RemoveRange(context.GeneratedReports.IgnoreQueryFilters());
        context.DomainEvents.RemoveRange(context.DomainEvents);

        await context.SaveChangesAsync();
    }
}
