using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Repositories;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services.Impl;

public class SubjectService(ISubjectRepository subjectRepository) : ISubjectService
{
    public async Task<Optional<IEnumerable<Subject>>> GetSubjectsAsync()
    {
        return await subjectRepository.GetSubjectsAsync();
    }

    public async Task<Optional<Subject>> GetSubjectAsync(int id)
    {
        return await subjectRepository.GetSubjectAsync(id);
    }

    public async Task<Optional<Subject>> AddSubjectAsync(Subject subject)
    {
        return await subjectRepository.AddSubjectAsync(subject);
    }

    public async Task<Optional<Subject>> UpdateSubjectAsync(Subject subject)
    {
        return await subjectRepository.UpdateSubjectAsync(subject);
    }

    public async Task DeleteSubjectAsync(int id)
    {
        var subject = await subjectRepository.GetSubjectAsync(id);
        if (subject.IsSuccess)
            await subjectRepository.DeleteSubjectAsync(subject.GetValueOrThrow());
    }
}