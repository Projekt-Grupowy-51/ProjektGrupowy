using Microsoft.AspNetCore.Identity;
using ProjektGrupowy.API.DTOs.Label;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Repositories;
using ProjektGrupowy.API.SignalR;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services.Impl;

public class LabelService(ILabelRepository labelRepository, IMessageService messageService, ISubjectRepository subjectRepository, UserManager<User> userManager) : ILabelService
{
    public async Task<Optional<IEnumerable<Label>>> GetLabelsAsync()
    {
        return await labelRepository.GetLabelsAsync();
    }

    public async Task<Optional<Label>> GetLabelAsync(int id)
    {
        return await labelRepository.GetLabelAsync(id);
    }

    public async Task<Optional<Label>> AddLabelAsync(LabelRequest labelRequest)
    {
        var owner = await userManager.FindByIdAsync(labelRequest.OwnerId);
        if (owner == null)
        {
            await messageService.SendErrorAsync(
                labelRequest.OwnerId,
                "No labeler found");
            return Optional<Label>.Failure("No labeler found");
        }

        var subjectOptional = await subjectRepository.GetSubjectAsync(labelRequest.SubjectId);

        if (subjectOptional.IsFailure)
        {
            await messageService.SendErrorAsync(
                labelRequest.OwnerId,
                "No subject found");
            return Optional<Label>.Failure("No subject found");
        }

        if (!char.IsLetterOrDigit(labelRequest.Shortcut))
        {
            await messageService.SendErrorAsync(
                labelRequest.OwnerId,
                "Shortcut has to be a letter or a number");
            return Optional<Label>.Failure("Shortcut has to be a letter or a number");
        }

        var label = new Label
        {
            Name = labelRequest.Name,
            Subject = subjectOptional.GetValueOrThrow(),
            ColorHex = labelRequest.ColorHex,
            Type = labelRequest.Type,
            Shortcut = labelRequest.Shortcut,
            Owner = owner
        };

        var result = await labelRepository.AddLabelAsync(label);
        if (result.IsFailure)
        {
            await messageService.SendErrorAsync(
                labelRequest.OwnerId,
                "Failed to add label");
            return Optional<Label>.Failure("Failed to add label");
        } 
        else 
        {
            await messageService.SendSuccessAsync(
                labelRequest.OwnerId,
                "Label added successfully");
            return result;
        }
    }

    public async Task<Optional<Label>> UpdateLabelAsync(int labelId, LabelRequest labelRequest)
    {
        var labelOptional = await labelRepository.GetLabelAsync(labelId);

        if (labelOptional.IsFailure)
        {
            await messageService.SendErrorAsync(
                labelRequest.OwnerId,
                "No label found");
            return labelOptional;
        }

        var label = labelOptional.GetValueOrThrow();

        var subjectOptional = await subjectRepository.GetSubjectAsync(labelRequest.SubjectId);
        if (subjectOptional.IsFailure)
        {
            await messageService.SendErrorAsync(
                labelRequest.OwnerId,
                "No subject found");
            return Optional<Label>.Failure("No subject found!");
        }

        label.Name = labelRequest.Name;
        label.Subject = subjectOptional.GetValueOrThrow();
        label.ColorHex = labelRequest.ColorHex;
        label.Type = labelRequest.Type;
        label.Shortcut = labelRequest.Shortcut;

        // return await labelRepository.UpdateLabelAsync(label);
        var result = await labelRepository.UpdateLabelAsync(label);
        if (result.IsFailure)
        {
            await messageService.SendErrorAsync(
                labelRequest.OwnerId,
                "Failed to update label");
            return Optional<Label>.Failure("Failed to update label");
        } 
        else 
        {
            await messageService.SendSuccessAsync(
                labelRequest.OwnerId,
                "Label updated successfully");
            return result;
        }
    }

    public async Task DeleteLabelAsync(int id)
    {
        var label = await labelRepository.GetLabelAsync(id);
        if (label.IsSuccess)
        {
            await messageService.SendInfoAsync(
                label.GetValueOrThrow().Owner.Id,
                "Label deleted successfully");
            await labelRepository.DeleteLabelAsync(label.GetValueOrThrow());
        }
    }
}