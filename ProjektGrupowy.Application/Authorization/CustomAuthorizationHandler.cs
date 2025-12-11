using Microsoft.AspNetCore.Authorization;
using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Application.Services;

namespace ProjektGrupowy.Application.Authorization
{
    public class CustomAuthorizationHandler(ICurrentUserService currentUserService)
        : AuthorizationHandler<ResourceOperationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            ResourceOperationRequirement requirement)
        {
            if (currentUserService.IsAdmin)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var userId = currentUserService.UserId;

            var isAuthorized = context.Resource switch
            {
                Project p => IsAuthorized(p, userId),
                IEnumerable<Project> ps => ps.All(p => IsAuthorized(p, userId)),

                VideoGroup vg => IsAuthorized(vg, userId),
                IEnumerable<VideoGroup> vgs => vgs.All(vg => IsAuthorized(vg, userId)),

                Video v => IsAuthorized(v, userId),
                IEnumerable<Video> vs => vs.All(v => IsAuthorized(v, userId)),

                Subject s => IsAuthorized(s, userId),
                IEnumerable<Subject> ss => ss.All(s => IsAuthorized(s, userId)),

                Label l => IsAuthorized(l, userId),
                IEnumerable<Label> ls => ls.All(l => IsAuthorized(l, userId)),

                AssignedLabel al => IsAuthorized(al, userId),
                IEnumerable<AssignedLabel> als => als.All(al => IsAuthorized(al, userId)),

                ProjectAccessCode pac => IsAuthorized(pac, userId),
                IEnumerable<ProjectAccessCode> pacs => pacs.All(pac => IsAuthorized(pac, userId)),

                SubjectVideoGroupAssignment svga => IsAuthorized(svga, userId),
                IEnumerable<SubjectVideoGroupAssignment> svgas => svgas.All(svga => IsAuthorized(svga, userId)),

                GeneratedReport gr => IsAuthorized(gr, userId),
                IEnumerable<GeneratedReport> grs => grs.All(gr => IsAuthorized(gr, userId)),

                _ => false
            };

            if (isAuthorized)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }

        private bool IsAuthorized(Project p, string userId) =>
            p.CreatedById == userId || p.ProjectLabelers.Any(l => l.Id == userId);

        private bool IsAuthorized(VideoGroup vg, string userId) =>
            vg.Project.CreatedById == userId ||
            vg.SubjectVideoGroupAssignments.Any(svga => svga.Labelers.Any(l => l.Id == userId));

        private bool IsAuthorized(Video v, string userId) => v.VideoGroup.Project.CreatedById == userId ||
            v.VideoGroup.SubjectVideoGroupAssignments.Any(svga => svga.Labelers.Any(l => l.Id == userId));

        private bool IsAuthorized(Subject s, string userId) =>
            s.Project.CreatedById == userId ||
            s.SubjectVideoGroupAssignments.Any(svga => svga.Labelers.Any(l => l.Id == userId));

        private bool IsAuthorized(Label l, string userId) =>
            IsAuthorized(l.Subject, userId);

        private bool IsAuthorized(AssignedLabel al, string userId) =>
            al.CreatedById == userId || IsAuthorized(al.Video.VideoGroup, userId);

        private bool IsAuthorized(ProjectAccessCode pac, string userId) =>
            pac.CreatedById == userId;

        private bool IsAuthorized(SubjectVideoGroupAssignment svga, string userId) =>
            svga.Subject.Project.CreatedById == userId || svga.Labelers.Any(l => l.Id == userId);

        private bool IsAuthorized(GeneratedReport gr, string userId) =>
            gr.CreatedById == userId;
    }
}
