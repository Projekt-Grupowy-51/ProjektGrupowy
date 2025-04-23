using Microsoft.AspNetCore.Identity;
using ProjektGrupowy.API.DTOs.Subject;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Repositories;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services.Impl;

public class SubjectService(ISubjectRepository subjectRepository, IProjectRepository projectRepository, UserManager<User> userManager)
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

        return await subjectRepository.AddSubjectAsync(subject);
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

        return await subjectRepository.UpdateSubjectAsync(subject);
    }

    public async Task<Optional<IEnumerable<Subject>>> GetSubjectsByProjectAsync(int projectId)
        => await subjectRepository.GetSubjectsByProjectAsync(projectId);

    public async Task DeleteSubjectAsync(int id)
    {
        var subject = await subjectRepository.GetSubjectAsync(id);
        if (subject.IsSuccess)
            await subjectRepository.DeleteSubjectAsync(subject.GetValueOrThrow());
    }
    
    public async Task<Optional<IEnumerable<Label>>> GetSubjectLabelsAsync(int subjectId)
    {
        return await subjectRepository.GetSubjectLabelsAsync(subjectId);
    }
}