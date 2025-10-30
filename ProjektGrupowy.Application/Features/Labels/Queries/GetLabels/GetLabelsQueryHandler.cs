using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Application.Services;
using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Application.Features.Labels.Queries.GetLabels;

public class GetLabelsQueryHandler : IRequestHandler<GetLabelsQuery, Result<List<Label>>>
{
    private readonly ILabelRepository _labelRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public GetLabelsQueryHandler(
        ILabelRepository labelRepository,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _labelRepository = labelRepository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<Result<List<Label>>> Handle(GetLabelsQuery request, CancellationToken cancellationToken)
    {
        var labels = await _labelRepository.GetLabelsAsync(request.UserId, request.IsAdmin);
        if (labels is null)
        {
            return Result.Fail("No labels found");
        }

        var authResult = await _authorizationService.AuthorizeAsync(_currentUserService.User, labels, new ResourceOperationRequirement(ResourceOperation.Read));
        return !authResult.Succeeded ? throw new ForbiddenException() : Result.Ok(labels);
    }
}
