using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.Application.Features.Labels.Queries.GetLabel;

public class GetLabelQueryHandler : IRequestHandler<GetLabelQuery, Result<Label>>
{
    private readonly ILabelRepository _labelRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public GetLabelQueryHandler(
        ILabelRepository labelRepository,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _labelRepository = labelRepository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<Result<Label>> Handle(GetLabelQuery request, CancellationToken cancellationToken)
    {
        var label = await _labelRepository.GetLabelAsync(request.Id, request.UserId, request.IsAdmin);
        if (label is null)
        {
            return Result.Fail("Label does not exist");
        }

        var authResult = await _authorizationService.AuthorizeAsync(_currentUserService.User, label, new ResourceOperationRequirement(ResourceOperation.Read));
        return !authResult.Succeeded ? throw new ForbiddenException() : Result.Ok(label);
    }
}
