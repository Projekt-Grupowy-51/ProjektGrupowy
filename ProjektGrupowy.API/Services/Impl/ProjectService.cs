using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Repositories;

namespace ProjektGrupowy.API.Services.Impl
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<IEnumerable<Project>> GetProjectsAsync()
        {
            return await _projectRepository.GetProjectsAsync();
        }

        public async Task<Project> GetProjectAsync(int id)
        {
            return await _projectRepository.GetProjectAsync(id);
        }

        public async Task<Project> AddProjectAsync(Project project)
        {
            return await _projectRepository.AddProjectAsync(project);
        }

        public async Task<Project> UpdateProjectAsync(Project project)
        {
            return await _projectRepository.UpdateProjectAsync(project);
        }

        public async Task DeleteProjectAsync(int id)
        {
            var project = await _projectRepository.GetProjectAsync(id);
            if (project != null)
            {
                await _projectRepository.DeleteProjectAsync(project);
            }
        }
    }
}
