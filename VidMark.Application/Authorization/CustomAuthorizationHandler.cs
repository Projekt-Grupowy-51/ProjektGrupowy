using Microsoft.AspNetCore.Authorization;
using VidMark.Domain.Models;
using VidMark.Application.Services;

namespace VidMark.Application.Authorization
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
            var operation = requirement.ResourceOperation;

            var isAuthorized = operation switch
            {
                ResourceOperation.Modify => CheckModifyAuthorization(context.Resource, userId),
                ResourceOperation.Read => CheckReadAuthorization(context.Resource, userId),
                _ => false
            };

            if (isAuthorized)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }

        private bool CheckModifyAuthorization(object? resource, string userId)
        {
            return resource switch
            {
                Project p => IsAuthorizedForModify(p, userId),
                IEnumerable<Project> ps => ps.All(p => IsAuthorizedForModify(p, userId)),

                VideoGroup vg => IsAuthorizedForModify(vg, userId),
                IEnumerable<VideoGroup> vgs => vgs.All(vg => IsAuthorizedForModify(vg, userId)),

                Video v => IsAuthorizedForModify(v, userId),
                IEnumerable<Video> vs => vs.All(v => IsAuthorizedForModify(v, userId)),

                Subject s => IsAuthorizedForModify(s, userId),
                IEnumerable<Subject> ss => ss.All(s => IsAuthorizedForModify(s, userId)),

                Label l => IsAuthorizedForModify(l, userId),
                IEnumerable<Label> ls => ls.All(l => IsAuthorizedForModify(l, userId)),

                AssignedLabel al => IsAuthorizedForModify(al, userId),
                IEnumerable<AssignedLabel> als => als.All(al => IsAuthorizedForModify(al, userId)),

                ProjectAccessCode pac => IsAuthorizedForModify(pac, userId),
                IEnumerable<ProjectAccessCode> pacs => pacs.All(pac => IsAuthorizedForModify(pac, userId)),

                SubjectVideoGroupAssignment svga => IsAuthorizedForModify(svga, userId),
                IEnumerable<SubjectVideoGroupAssignment> svgas => svgas.All(svga => IsAuthorizedForModify(svga, userId)),

                GeneratedReport gr => IsAuthorizedForModify(gr, userId),
                IEnumerable<GeneratedReport> grs => grs.All(gr => IsAuthorizedForModify(gr, userId)),

                _ => false
            };
        }

        private bool CheckReadAuthorization(object? resource, string userId)
        {
            return resource switch
            {
                Project p => IsAuthorizedForRead(p, userId),
                IEnumerable<Project> ps => ps.All(p => IsAuthorizedForRead(p, userId)),

                VideoGroup vg => IsAuthorizedForRead(vg, userId),
                IEnumerable<VideoGroup> vgs => vgs.All(vg => IsAuthorizedForRead(vg, userId)),

                Video v => IsAuthorizedForRead(v, userId),
                IEnumerable<Video> vs => vs.All(v => IsAuthorizedForRead(v, userId)),

                Subject s => IsAuthorizedForRead(s, userId),
                IEnumerable<Subject> ss => ss.All(s => IsAuthorizedForRead(s, userId)),

                Label l => IsAuthorizedForRead(l, userId),
                IEnumerable<Label> ls => ls.All(l => IsAuthorizedForRead(l, userId)),

                AssignedLabel al => IsAuthorizedForRead(al, userId),
                IEnumerable<AssignedLabel> als => als.All(al => IsAuthorizedForRead(al, userId)),

                ProjectAccessCode pac => IsAuthorizedForRead(pac, userId),
                IEnumerable<ProjectAccessCode> pacs => pacs.All(pac => IsAuthorizedForRead(pac, userId)),

                SubjectVideoGroupAssignment svga => IsAuthorizedForRead(svga, userId),
                IEnumerable<SubjectVideoGroupAssignment> svgas => svgas.All(svga => IsAuthorizedForRead(svga, userId)),

                GeneratedReport gr => IsAuthorizedForRead(gr, userId),
                IEnumerable<GeneratedReport> grs => grs.All(gr => IsAuthorizedForRead(gr, userId)),

                _ => false
            };
        }

        // ===== MODIFY AUTHORIZATION (only Scientist/Creator + Admin) =====

        private bool IsAuthorizedForModify(Project p, string userId) =>
            p.CreatedById == userId;

        private bool IsAuthorizedForModify(VideoGroup vg, string userId) =>
            vg.CreatedById == userId;

        private bool IsAuthorizedForModify(Video v, string userId) =>
            v.CreatedById == userId;

        private bool IsAuthorizedForModify(Subject s, string userId) =>
            s.CreatedById == userId;

        private bool IsAuthorizedForModify(Label l, string userId) =>
            l.CreatedById == userId;

        // Exception: Labeler can modify their own AssignedLabel
        private bool IsAuthorizedForModify(AssignedLabel al, string userId) =>
            al.CreatedById == userId || al.Label.CreatedById == userId;

        private bool IsAuthorizedForModify(ProjectAccessCode pac, string userId) =>
            pac.CreatedById == userId;

        private bool IsAuthorizedForModify(SubjectVideoGroupAssignment svga, string userId) =>
            svga.CreatedById == userId;

        private bool IsAuthorizedForModify(GeneratedReport gr, string userId) =>
            gr.CreatedById == userId;

        // ===== READ AUTHORIZATION (Scientist/Creator + Labeler if assigned + Admin) =====

        private bool IsAuthorizedForRead(Project p, string userId) =>
            p.CreatedById == userId || p.ProjectLabelers.Any(l => l.Id == userId);

        private bool IsAuthorizedForRead(VideoGroup vg, string userId) =>
            vg.CreatedById == userId ||
            vg.SubjectVideoGroupAssignments.Any(svga => svga.Labelers.Any(l => l.Id == userId));

        private bool IsAuthorizedForRead(Video v, string userId) =>
            v.CreatedById == userId ||
            v.VideoGroup.SubjectVideoGroupAssignments.Any(svga => svga.Labelers.Any(l => l.Id == userId));

        private bool IsAuthorizedForRead(Subject s, string userId) =>
            s.CreatedById == userId ||
            s.SubjectVideoGroupAssignments.Any(svga => svga.Labelers.Any(l => l.Id == userId));

        private bool IsAuthorizedForRead(Label l, string userId) =>
            l.CreatedById == userId ||
            l.Subject.SubjectVideoGroupAssignments.Any(svga => svga.Labelers.Any(l => l.Id == userId));

        private bool IsAuthorizedForRead(AssignedLabel al, string userId) =>
            al.CreatedById == userId ||
            al.Label.CreatedById == userId;

        private bool IsAuthorizedForRead(ProjectAccessCode pac, string userId) =>
            pac.CreatedById == userId;

        private bool IsAuthorizedForRead(SubjectVideoGroupAssignment svga, string userId) =>
            svga.CreatedById == userId || svga.Labelers.Any(l => l.Id == userId);

        private bool IsAuthorizedForRead(GeneratedReport gr, string userId) =>
            gr.CreatedById == userId;
    }
}
