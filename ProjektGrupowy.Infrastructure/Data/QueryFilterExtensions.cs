using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Infrastructure.Data
{
    public static class QueryFilterExtensions
    {
        public static IQueryable<Project> FilteredProjects(this IQueryable<Project> projects, string userId, bool isAdmin = false)
        {
            return projects
                .Where(p => isAdmin || 
                           p.CreatedBy.Id == userId || 
                           p.ProjectLabelers.Any(l => l.Id == userId));
        }

        public static IQueryable<VideoGroup> FilteredVideoGroups(this IQueryable<VideoGroup> videoGroups, string userId, bool isAdmin = false)
        {
            return videoGroups
                .Where(vg => isAdmin || 
                            vg.Project.CreatedBy.Id == userId || 
                            vg.SubjectVideoGroupAssignments.Any(svga => svga.Labelers.Any(l => l.Id == userId)));
        }

        public static IQueryable<Video> FilteredVideos(this IQueryable<Video> videos, string userId, bool isAdmin = false)
        {
            return videos
                .Where(v => isAdmin || 
                           v.VideoGroup.Project.CreatedBy.Id == userId || 
                           v.VideoGroup.SubjectVideoGroupAssignments.Any(svga => svga.Labelers.Any(l => l.Id == userId)));
        }

        public static IQueryable<Subject> FilteredSubjects(this IQueryable<Subject> subjects, string userId, bool isAdmin = false)
        {
            return subjects
                .Where(s => isAdmin || 
                           s.Project.CreatedBy.Id == userId || 
                           s.SubjectVideoGroupAssignments.Any(svga => svga.Labelers.Any(l => l.Id == userId)));
        }

        public static IQueryable<Label> FilteredLabels(this IQueryable<Label> labels, string userId, bool isAdmin = false)
        {
            return labels
                .Where(l => isAdmin || 
                           l.Subject.Project.CreatedBy.Id == userId || 
                           l.Subject.SubjectVideoGroupAssignments.Any(svga => svga.Labelers.Any(lab => lab.Id == userId)));
        }

        public static IQueryable<AssignedLabel> FilteredAssignedLabels(this IQueryable<AssignedLabel> assignedLabels, string userId, bool isAdmin = false)
        {
            return assignedLabels
                .Where(al => isAdmin || 
                            al.CreatedBy.Id == userId || 
                            al.Video.VideoGroup.Project.CreatedBy.Id == userId);
        }

        public static IQueryable<ProjectAccessCode> FilteredProjectAccessCodes(this IQueryable<ProjectAccessCode> projectAccessCodes, string userId, bool isAdmin = false)
        {
            return projectAccessCodes
                .Where(pac => isAdmin || 
                             pac.CreatedBy.Id == userId);
        }

        public static IQueryable<SubjectVideoGroupAssignment> FilteredSubjectVideoGroupAssignments(this IQueryable<SubjectVideoGroupAssignment> assignments, string userId, bool isAdmin = false)
        {
            return assignments
                .Where(svga => isAdmin || 
                              svga.Subject.Project.CreatedBy.Id == userId || 
                              svga.Labelers.Any(l => l.Id == userId));
        }

        public static IQueryable<GeneratedReport> FilteredGeneratedReports(this IQueryable<GeneratedReport> reports, string userId, bool isAdmin = false)
        {
            return reports
                .Where(r => isAdmin || r.CreatedBy.Id == userId);
        }
    }
}
