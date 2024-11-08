using Microsoft.EntityFrameworkCore;
using ProjektGrupowy.API.Data;
using ProjektGrupowy.API.Models;

namespace ProjektGrupowy.API.Repositories.Impl
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _context;

        public ProjectRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Project> AddProjectAsync(Project project)
        {
            var p = await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();

            return p.Entity;
        }

        public async Task DeleteProjectAsync(Project project)
        {
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
        }

        public async Task<Project> GetProjectAsync(int id)
        {
            return await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Project>> GetProjectsAsync()
        {
            return await _context.Projects.ToListAsync();
        }

        public async Task<Project> UpdateProjectAsync(Project project)
        {
            var p = _context.Projects.Update(project);
            await _context.SaveChangesAsync();

            return p.Entity;
        }
    }
}
