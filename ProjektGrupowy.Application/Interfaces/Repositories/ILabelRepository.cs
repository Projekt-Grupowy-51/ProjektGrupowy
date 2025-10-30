using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Application.Interfaces.Repositories;

public interface ILabelRepository
{
    Task<List<Label>> GetLabelsAsync(string userId, bool isAdmin);
    Task<Label> GetLabelAsync(int id, string userId, bool isAdmin);
    Task AddLabelAsync(Label label);
    void DeleteLabel(Label label);
}