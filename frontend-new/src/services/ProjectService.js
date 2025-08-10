import apiClient from './ApiClient.js';

class ProjectService {
  async getProjects() {
    return await apiClient.get('/Project');
  }

  async getProject(id) {
    return await apiClient.get(`/Project/${id}`);
  }

  async createProject(projectData) {
    return await apiClient.post('/Project', projectData);
  }

  async updateProject(id, projectData) {
    return await apiClient.put(`/Project/${id}`, projectData);
  }

  async deleteProject(id) {
    return await apiClient.delete(`/Project/${id}`);
  }

  async getProjectSubjects(projectId) {
    return await apiClient.get(`/Project/${projectId}/subjects`);
  }

  async getProjectVideos(projectId) {
    return await apiClient.get(`/Project/${projectId}/videos`);
  }

  async getProjectLabelers(projectId) {
    return await apiClient.get(`/Project/${projectId}/labelers`);
  }

  async getProjectAccessCodes(projectId) {
    return await apiClient.get(`/Project/${projectId}/access-codes`);
  }

  async getProjectAssignments(projectId) {
    return await apiClient.get(`/Project/${projectId}/assignments`);
  }

  async getProjectReports(projectId) {
    return await apiClient.get(`/Project/${projectId}/reports`);
  }

  async generateProjectReport(projectId, reportData) {
    return await apiClient.post(`/Project/${projectId}/reports`, reportData);
  }

  async deleteProjectReport(reportId) {
    return await apiClient.delete(`/ProjectReport/${reportId}`);
  }
}

export default new ProjectService();
