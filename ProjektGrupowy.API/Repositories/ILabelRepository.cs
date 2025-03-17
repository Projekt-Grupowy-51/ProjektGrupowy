using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Repositories;

public interface ILabelRepository
{
    Task<Optional<IEnumerable<Label>>> GetLabelsAsync();
    Task<Optional<Label>> GetLabelAsync(int id);
    Task<Optional<Label>> AddLabelAsync(Label label);
    Task<Optional<Label>> UpdateLabelAsync(Label label);
    Task DeleteLabelAsync(Label label);
    Task<Optional<IEnumerable<Label>>> GetLabelsBySubjectIdAsync(int subjectId);
}