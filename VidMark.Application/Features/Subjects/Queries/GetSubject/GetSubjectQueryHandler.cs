using MediatR;
using VidMark.Domain.Models;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;

namespace VidMark.Application.Features.Subjects.Queries.GetSubject;

public class GetSubjectQueryHandler : IRequestHandler<GetSubjectQuery, Result<Subject>>
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public GetSubjectQueryHandler(
        ISubjectRepository subjectRepository,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _subjectRepository = subjectRepository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<Result<Subject>> Handle(GetSubjectQuery request, CancellationToken cancellationToken)
    {
        var subject = await _subjectRepository.GetSubjectAsync(request.Id, request.UserId, request.IsAdmin);

        if (subject is null)
        {
            return Result.Fail("No subject found");
        }

        var authResult = await _authorizationService.AuthorizeAsync(
            _currentUserService.User,
            subject,
            new ResourceOperationRequirement(ResourceOperation.Read));

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        return Result.Ok(subject);
    }
}
