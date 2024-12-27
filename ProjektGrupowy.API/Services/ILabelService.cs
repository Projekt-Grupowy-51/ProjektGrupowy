using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services;

public interface ILabelService
{
    Task<Optional<IEnumerable<Label>>> GetLabelsAsync();
    Task<Optional<Label>> GetLabelAsync(int id);
    Task<Optional<Label>> AddLabelAsync(Label label);
    Task<Optional<Label>> UpdateLabelAsync(Label label);
    Task DeleteLabelAsync(int id);
}