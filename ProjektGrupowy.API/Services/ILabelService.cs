using ProjektGrupowy.API.DTOs.Label;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services;

public interface ILabelService
{
    Task<Optional<IEnumerable<Label>>> GetLabelsAsync();
    Task<Optional<Label>> GetLabelAsync(int id);
    Task<Optional<Label>> AddLabelAsync(LabelRequest labelRequest);
    Task<Optional<Label>> UpdateLabelAsync(int labelId, LabelRequest labelRequest);
    Task DeleteLabelAsync(int id);
}