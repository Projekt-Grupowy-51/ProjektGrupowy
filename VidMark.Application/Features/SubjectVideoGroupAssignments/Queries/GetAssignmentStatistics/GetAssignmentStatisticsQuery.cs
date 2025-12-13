using FluentResults;
using MediatR;

namespace VidMark.Application.Features.SubjectVideoGroupAssignments.Queries.GetAssignmentStatistics;

public record GetAssignmentStatisticsQuery(
    int AssignmentId,
    string UserId,
    bool IsAdmin) : IRequest<Result<AssignmentStatistics>>;
