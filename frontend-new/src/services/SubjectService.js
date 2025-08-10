import apiClient from './ApiClient.js';

class SubjectService {
  async getSubjects(projectId = null) {
    const params = projectId ? { projectId } : {};
    const response = await apiClient.get('/Subject', { params });
    return response.data;
  }

  async getSubject(id) {
    const response = await apiClient.get(`/subject/${id}`);
    return response.data;
  }

  async createSubject(subjectData) {
    const response = await apiClient.post('/Subject', subjectData);
    return response.data;
  }

  async updateSubject(id, subjectData) {
    const response = await apiClient.put(`/Subject/${id}`, subjectData);
    return response.data;
  }

  async deleteSubject(id) {
    await apiClient.delete(`/Subject/${id}`);
    return { success: true };
  }

  async getSubjectLabels(subjectId) {
    const response = await apiClient.get(`/subject/${subjectId}/label`);
    return response.data.filter(l => l.subjectId === parseInt(subjectId)).sort((a, b) => a.id - b.id);
  }

  async deleteLabel(labelId) {
    await apiClient.delete(`/label/${labelId}`);
    return { success: true };
  }
}

export default new SubjectService();
