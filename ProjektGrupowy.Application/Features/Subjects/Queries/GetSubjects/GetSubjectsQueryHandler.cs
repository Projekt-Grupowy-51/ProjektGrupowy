using MediatR;
using ProjektGrupowy.Domain.Models;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Application.Services;

namespace ProjektGrupowy.Application.Features.Subjects.Queries.GetSubjects;

public class GetSubjectsQueryHandler : IRequestHandler<GetSubjectsQuery, Result<List<Subject>>>
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public GetSubjectsQueryHandler(
        ISubjectRepository subjectRepository,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _subjectRepository = subjectRepository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<Result<List<Subject>>> Handle(GetSubjectsQuery request, CancellationToken cancellationToken)
    {
        var subjects = await _subjectRepository.GetSubjectsAsync(request.UserId, request.IsAdmin);

        var authResult = await _authorizationService.AuthorizeAsync(
            _currentUserService.User,
            subjects,
            new ResourceOperationRequirement(ResourceOperation.Read));

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        return Result.Ok(subjects);
    }
}
