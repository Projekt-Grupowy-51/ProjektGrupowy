using VidMark.Domain.Models;
using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.Subjects.Queries.GetSubjectsByProject;

public record GetSubjectsByProjectQuery(int ProjectId, string UserId, bool IsAdmin)
    : BaseQuery<Result<List<Subject>>>(UserId, IsAdmin);
