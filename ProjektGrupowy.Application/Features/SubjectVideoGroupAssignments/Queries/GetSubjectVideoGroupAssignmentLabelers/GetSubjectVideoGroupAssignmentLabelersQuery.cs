using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.SubjectVideoGroupAssignments.Queries.GetSubjectVideoGroupAssignmentLabelers;

public record GetSubjectVideoGroupAssignmentLabelersQuery(int Id, string UserId, bool IsAdmin)
    : BaseQuery<Result<List<User>>>(UserId, IsAdmin);
