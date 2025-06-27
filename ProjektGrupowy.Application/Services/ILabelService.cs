using ProjektGrupowy.Application.DTOs.Label;
using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Domain.Utils;
using ProjektGrupowy.Domain.Utils;

namespace ProjektGrupowy.Application.Services;

public interface ILabelService
{
    Task<Optional<IEnumerable<Label>>> GetLabelsAsync();
    Task<Optional<Label>> GetLabelAsync(int id);
    Task<Optional<Label>> AddLabelAsync(LabelRequest labelRequest);
    Task<Optional<Label>> UpdateLabelAsync(int labelId, LabelRequest labelRequest);
    Task DeleteLabelAsync(int id);
}