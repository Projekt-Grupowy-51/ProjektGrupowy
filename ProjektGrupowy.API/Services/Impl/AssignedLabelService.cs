using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Repositories;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services.Impl;

public class AssignedLabelService(IAssignedLabelRepository assignedLabelRepository) : IAssignedLabelService

{
    public async Task<Optional<IEnumerable<AssignedLabel>>> GetAssignedLabelsAsync()
    {
        return await assignedLabelRepository.GetAssignedLabelsAsync();
    }

    public async Task<Optional<AssignedLabel>> GetAssignedLabelAsync(int id)
    {
        return await assignedLabelRepository.GetAssignedLabelAsync(id);
    }

    public async Task<Optional<AssignedLabel>> AddAssignedLabelAsync(AssignedLabel assignedLabel)
    {
        return await assignedLabelRepository.AddAssignedLabelAsync(assignedLabel);
    }

    public async Task<Optional<AssignedLabel>> UpdateAssignedLabelAsync(AssignedLabel assignedLabel)
    {
        return await assignedLabelRepository.UpdateAssignedLabelAsync(assignedLabel);
    }

    public async Task DeleteAssignedLabelAsync(int id)
    {
        var assignedLabel = await assignedLabelRepository.GetAssignedLabelAsync(id);
        if (assignedLabel.IsSuccess)
            await assignedLabelRepository.DeleteAssignedLabelAsync(assignedLabel.GetValueOrThrow());
    }
}