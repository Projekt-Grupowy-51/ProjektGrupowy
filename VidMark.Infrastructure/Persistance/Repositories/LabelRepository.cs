using Microsoft.EntityFrameworkCore;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Domain.Models;

namespace VidMark.Infrastructure.Persistance.Repositories;

public class LabelRepository(IReadWriteContext context) : ILabelRepository
{
    public Task<List<Label>> GetLabelsAsync(string userId, bool isAdmin)
    {
        return context.Labels.FilteredLabels(userId, isAdmin).ToListAsync();
    }

    public Task<Label> GetLabelAsync(int id, string userId, bool isAdmin)
    {
        return context.Labels.FilteredLabels(userId, isAdmin).FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task AddLabelAsync(Label label)
    {
        _ = await context.Labels.AddAsync(label);
    }

    public void DeleteLabel(Label label)
    {
        _ = context.Labels.Remove(label);
    }
}