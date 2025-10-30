using MediatR;
using ProjektGrupowy.Domain.Models;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Application.Services;

namespace ProjektGrupowy.Application.Features.Subjects.Queries.GetSubjectLabels;

public class GetSubjectLabelsQueryHandler : IRequestHandler<GetSubjectLabelsQuery, Result<List<Label>>>
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public GetSubjectLabelsQueryHandler(
        ISubjectRepository subjectRepository,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _subjectRepository = subjectRepository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<Result<List<Label>>> Handle(GetSubjectLabelsQuery request, CancellationToken cancellationToken)
    {
        var labels = await _subjectRepository.GetSubjectLabelsAsync(request.SubjectId, request.UserId, request.IsAdmin);

        var authResult = await _authorizationService.AuthorizeAsync(
            _currentUserService.User,
            labels,
            new ResourceOperationRequirement(ResourceOperation.Read));

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        return Result.Ok(labels);
    }
}
