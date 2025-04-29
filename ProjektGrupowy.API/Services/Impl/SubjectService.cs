using Microsoft.AspNetCore.Identity;
using ProjektGrupowy.API.DTOs.Subject;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Repositories;
using ProjektGrupowy.API.SignalR;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services.Impl;

public class SubjectService(
    ISubjectRepository subjectRepository, 
    IProjectRepository projectRepository,
    IMessageService messageService, 
    UserManager<User> userManager)
    : ISubjectService
{
    public async Task<Optional<IEnumerable<Subject>>> GetSubjectsAsync()
    {
        return await subjectRepository.GetSubjectsAsync();
    }

    public async Task<Optional<Subject>> GetSubjectAsync(int id)
    {
        return await subjectRepository.GetSubjectAsync(id);
    }

    public async Task<Optional<Subject>> AddSubjectAsync(SubjectRequest subjectRequest)
    {
        var projectOptional = await projectRepository.GetProjectAsync(subjectRequest.ProjectId);

        if (projectOptional.IsFailure)
        {
            return Optional<Subject>.Failure("No project found!");
        }

        var owner = await userManager.FindByIdAsync(subjectRequest.OwnerId);
        if (owner == null)
        {
            return Optional<Subject>.Failure("No labeler found");
        }

        var subject = new Subject
        {
            Name = subjectRequest.Name,
            Description = subjectRequest.Description,
            Project = projectOptional.GetValueOrThrow(),
            Owner = owner,
        };

        // return await subjectRepository.AddSubjectAsync(subject);
        var result = await subjectRepository.AddSubjectAsync(subject);
        if (result.IsFailure)
        {
            await messageService.SendErrorAsync(
                subjectRequest.OwnerId,
                "Failed to add subject");
            return result;
        }
        await messageService.SendSuccessAsync(
            subjectRequest.OwnerId,
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

        var projectOptional = await projectRepository.GetProjectAsync(subjectRequest.ProjectId);

        if (projectOptional.IsFailure)
        {
            return Optional<Subject>.Failure("No project found!");
        }

        var owner = await userManager.FindByIdAsync(subjectRequest.OwnerId);
        if (owner == null)
        {
            return Optional<Subject>.Failure("No labeler found");
        }

        subject.Name = subjectRequest.Name;
        subject.Description = subjectRequest.Description;
        subject.Project = projectOptional.GetValueOrThrow();
        subject.Owner = owner;

        // return await subjectRepository.UpdateSubjectAsync(subject);
        var result = await subjectRepository.UpdateSubjectAsync(subject);
        if (result.IsFailure)
        {
            await messageService.SendErrorAsync(
                subjectRequest.OwnerId,
                "Failed to update subject");
            return result;
        }
        await messageService.SendInfoAsync(
            subjectRequest.OwnerId,
            "Subject updated successfully");
        return result;
    }

    public async Task<Optional<IEnumerable<Subject>>> GetSubjectsByProjectAsync(int projectId)
        => await subjectRepository.GetSubjectsByProjectAsync(projectId);

    public async Task DeleteSubjectAsync(int id)
    {
        var subject = await subjectRepository.GetSubjectAsync(id);
        if (subject.IsSuccess)
        {
            await messageService.SendInfoAsync(
                subject.GetValueOrThrow().Owner.Id,
                "Subject deleted successfully");
            await subjectRepository.DeleteSubjectAsync(subject.GetValueOrThrow());
        }
    }
    
    public async Task<Optional<IEnumerable<Label>>> GetSubjectLabelsAsync(int subjectId)
    {
        return await subjectRepository.GetSubjectLabelsAsync(subjectId);
    }
}