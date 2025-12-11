using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.ProjectAccessCodes.Queries.ValidateAccessCode;

public record ValidateAccessCodeQuery(string Code, string UserId, bool IsAdmin)
    : BaseQuery<Result<bool>>(UserId, IsAdmin);
