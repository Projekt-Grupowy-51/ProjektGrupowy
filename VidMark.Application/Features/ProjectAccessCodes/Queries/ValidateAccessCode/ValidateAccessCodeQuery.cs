using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.ProjectAccessCodes.Queries.ValidateAccessCode;

public record ValidateAccessCodeQuery(string Code, string UserId, bool IsAdmin)
    : BaseQuery<Result<bool>>(UserId, IsAdmin);
