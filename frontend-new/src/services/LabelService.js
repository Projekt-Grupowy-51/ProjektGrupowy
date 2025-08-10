import apiClient from './ApiClient.js';

class LabelService {
  async getLabels() {
    return await apiClient.get('/Label');
  }

  async getLabel(id) {
    return await apiClient.get(`/Label/${id}`);
  }

  async createLabel(labelData) {
    return await apiClient.post('/Label', labelData);
  }

  async updateLabel(id, labelData) {
    return await apiClient.put(`/Label/${id}`, labelData);
  }

  async deleteLabel(id) {
    return await apiClient.delete(`/Label/${id}`);
  }

  async getLabelsBySubject(subjectId) {
    return await apiClient.get(`/Subject/${subjectId}/labels`);
  }
}

export default new LabelService();
