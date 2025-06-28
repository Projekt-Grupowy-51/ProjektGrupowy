using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.DTOs.Label;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.SignalR;
using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Application.Services;
using ProjektGrupowy.Domain.Utils;
using ProjektGrupowy.Infrastructure.Repositories;

namespace ProjektGrupowy.Application.Services.Impl;

public class LabelService(ILabelRepository labelRepository, IMessageService messageService, ISubjectRepository subjectRepository, UserManager<User> userManager, ICurrentUserService currentUserService, IAuthorizationService authorizationService) : ILabelService
{
    public async Task<Optional<IEnumerable<Label>>> GetLabelsAsync()
    {
        var labelsOpt = await labelRepository.GetLabelsAsync(currentUserService.UserId, currentUserService.IsAdmin);
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

    public async Task<Optional<Label>> GetLabelAsync(int id)
    {
        var labelOpt = await labelRepository.GetLabelAsync(id, currentUserService.UserId, currentUserService.IsAdmin);
        if (labelOpt.IsFailure)
        {
            return labelOpt;
        }
        var label = labelOpt.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, label, new ResourceOperationRequirement(ResourceOperation.Read));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }
        return labelOpt;
    }

    public async Task<Optional<Label>> AddLabelAsync(LabelRequest labelRequest)
    {
        var subjectOptional = await subjectRepository.GetSubjectAsync(labelRequest.SubjectId, currentUserService.UserId, currentUserService.IsAdmin);

        if (subjectOptional.IsFailure)
        {
            return Optional<Label>.Failure("No subject found");
        }

        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, subjectOptional.GetValueOrThrow(), new ResourceOperationRequirement(ResourceOperation.Create));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        if (!char.IsLetterOrDigit(labelRequest.Shortcut))
        {
            return Optional<Label>.Failure("Shortcut has to be a letter or a number");
        }

        var label = new Label
        {
            Name = labelRequest.Name,
            Subject = subjectOptional.GetValueOrThrow(),
            ColorHex = labelRequest.ColorHex,
            Type = labelRequest.Type,
            Shortcut = labelRequest.Shortcut,
            CreatedById = currentUserService.UserId,
        };

        var result = await labelRepository.AddLabelAsync(label);
        if (result.IsFailure)
        {
            return Optional<Label>.Failure("Failed to add label");
        }
        else
        {
            await messageService.SendSuccessAsync(
                currentUserService.UserId,
                "Label added successfully");
            return result;
        }
    }

    public async Task<Optional<Label>> UpdateLabelAsync(int labelId, LabelRequest labelRequest)
    {
        var labelOptional = await labelRepository.GetLabelAsync(labelId, currentUserService.UserId, currentUserService.IsAdmin);

        if (labelOptional.IsFailure)
        {
            return labelOptional;
        }

        var label = labelOptional.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, label, new ResourceOperationRequirement(ResourceOperation.Update));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        var subjectOptional = await subjectRepository.GetSubjectAsync(labelRequest.SubjectId, currentUserService.UserId, currentUserService.IsAdmin);
        if (subjectOptional.IsFailure)
        {
            return Optional<Label>.Failure("No subject found!");
        }

        var authResultSubject = await authorizationService.AuthorizeAsync(currentUserService.User, subjectOptional.GetValueOrThrow(), new ResourceOperationRequirement(ResourceOperation.Update));
        if (!authResultSubject.Succeeded)
        {
            throw new ForbiddenException();
        }

        if (!char.IsLetterOrDigit(labelRequest.Shortcut))
        {
            return Optional<Label>.Failure("Shortcut has to be a letter or a number");
        }

        label.Name = labelRequest.Name;
        label.Subject = subjectOptional.GetValueOrThrow();
        label.ColorHex = labelRequest.ColorHex;
        label.Type = labelRequest.Type;
        label.Shortcut = labelRequest.Shortcut;

        var result = await labelRepository.UpdateLabelAsync(label);
        if (result.IsFailure)
        {
            return Optional<Label>.Failure("Failed to update label");
        }
        else
        {
            await messageService.SendSuccessAsync(
                currentUserService.UserId,
                "Label updated successfully");
            return result;
        }
    }

    public async Task DeleteLabelAsync(int id)
    {
        var label = await labelRepository.GetLabelAsync(id, currentUserService.UserId, currentUserService.IsAdmin);

        if (label.IsFailure)
        {
            await messageService.SendErrorAsync(
                currentUserService.UserId,
                "Failed to delete label");
            return;
        }

        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, label.GetValueOrThrow(), new ResourceOperationRequirement(ResourceOperation.Delete));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        await labelRepository.DeleteLabelAsync(label.GetValueOrThrow());
        await messageService.SendInfoAsync(
            currentUserService.UserId,
            "Label deleted successfully");
    }
}