using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.DTOs.Subject;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Infrastructure.Repositories;
using ProjektGrupowy.Application.SignalR;
using ProjektGrupowy.Domain.Utils;
using ProjektGrupowy.Domain.Utils;
using ProjektGrupowy.Domain.Services;

namespace ProjektGrupowy.Application.Services.Impl;

public class SubjectService(
    ISubjectRepository subjectRepository,
    IProjectRepository projectRepository,
    IMessageService messageService,
    ICurrentUserService currentUserService,
    IAuthorizationService authorizationService,
    UserManager<User> userManager)
    : ISubjectService
{
    public async Task<Optional<IEnumerable<Subject>>> GetSubjectsAsync()
    {
        var subjectsOpt = await subjectRepository.GetSubjectsAsync();
        if (subjectsOpt.IsFailure)
        {
            return subjectsOpt;
        }

        var subjects = subjectsOpt.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, subjects, new ResourceOperationRequirement(ResourceOperation.Read));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        return subjectsOpt;
    }

    public async Task<Optional<Subject>> GetSubjectAsync(int id)
    {
        var subjectOpt = await subjectRepository.GetSubjectAsync(id);
        if (subjectOpt.IsFailure)
        {
            return subjectOpt;
        }

        var subject = subjectOpt.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, subject, new ResourceOperationRequirement(ResourceOperation.Read));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        return subjectOpt;
    }

    public async Task<Optional<Subject>> AddSubjectAsync(SubjectRequest subjectRequest)
    {
        var projectOptional = await projectRepository.GetProjectAsync(subjectRequest.ProjectId);

        if (projectOptional.IsFailure)
        {
            return Optional<Subject>.Failure("No project found!");
        }

        var subject = new Subject
        {
            Name = subjectRequest.Name,
            Description = subjectRequest.Description,
            Project = projectOptional.GetValueOrThrow(),
            CreatedById = currentUserService.UserId,
        };

        var result = await subjectRepository.AddSubjectAsync(subject);
        if (result.IsFailure)
        {
            await messageService.SendErrorAsync(
                currentUserService.UserId,
                "Failed to add subject");
            return result;
        }
        await messageService.SendSuccessAsync(
            currentUserService.UserId,
            "Subject added successfully");
        return result;

    }

    public async Task<Optional<Subject>> UpdateSubjectAsync(int subjectId, SubjectRequest subjectRequest)
    {
        var subjectOptional = await subjectRepository.GetSubjectAsync(subjectId);

        if (subjectOptional.IsFailure)
        {
            return subjectOptional;
        }

        var subject = subjectOptional.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, subject, new ResourceOperationRequirement(ResourceOperation.Update));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        var projectOptional = await projectRepository.GetProjectAsync(subjectRequest.ProjectId);
        if (projectOptional.IsFailure)
        {
            return Optional<Subject>.Failure("No project found!");
        }

        var project = projectOptional.GetValueOrThrow();
        var authResultProject = await authorizationService.AuthorizeAsync(currentUserService.User, project, new ResourceOperationRequirement(ResourceOperation.Update));
        if (!authResultProject.Succeeded)
        {
            throw new ForbiddenException();
        }

        subject.Name = subjectRequest.Name;
        subject.Description = subjectRequest.Description;
        subject.Project = projectOptional.GetValueOrThrow();
        subject.CreatedById = currentUserService.UserId;

        // return await subjectRepository.UpdateSubjectAsync(subject);
        var result = await subjectRepository.UpdateSubjectAsync(subject);
        if (result.IsFailure)
        {
            await messageService.SendErrorAsync(
                currentUserService.UserId,
                "Failed to update subject");
            return result;
        }
        await messageService.SendInfoAsync(
            currentUserService.UserId,
            "Subject updated successfully");
        return result;
    }

    public async Task<Optional<IEnumerable<Subject>>> GetSubjectsByProjectAsync(int projectId)
    {
        var subjects = await subjectRepository.GetSubjectsByProjectAsync(projectId);
        if (subjects.IsFailure)
        {
            return subjects;
        }

        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, subjects.GetValueOrThrow(), new ResourceOperationRequirement(ResourceOperation.Read));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        return subjects;
    }

    public async Task DeleteSubjectAsync(int id)
    {
        var subject = await subjectRepository.GetSubjectAsync(id);

        if (subject.IsFailure)
        {
            await messageService.SendErrorAsync(
                currentUserService.UserId,
                "Failed to delete subject");
            return;
        }

        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, subject.GetValueOrThrow(), new ResourceOperationRequirement(ResourceOperation.Delete));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        await messageService.SendInfoAsync(
            currentUserService.UserId,
            "Subject deleted successfully");

        await subjectRepository.DeleteSubjectAsync(subject.GetValueOrThrow());
    }

    public async Task<Optional<IEnumerable<Label>>> GetSubjectLabelsAsync(int subjectId)
    {
        var labelsOpt = await subjectRepository.GetSubjectLabelsAsync(subjectId);
        if (labelsOpt.IsFailure)
        {
            return labelsOpt;
        }

        var labels = labelsOpt.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, labels, new ResourceOperationRequirement(ResourceOperation.Read));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        return labelsOpt;
    }
}