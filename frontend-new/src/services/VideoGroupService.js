import apiClient from './ApiClient.js';

class VideoGroupService {
  async getVideoGroups() {
    return await apiClient.get('/VideoGroup');
  }

  async getVideoGroup(id) {
    return await apiClient.get(`/VideoGroup/${id}`);
  }

  async createVideoGroup(videoGroupData) {
    return await apiClient.post('/VideoGroup', videoGroupData);
  }

  async updateVideoGroup(id, videoGroupData) {
    return await apiClient.put(`/VideoGroup/${id}`, videoGroupData);
  }

  async deleteVideoGroup(id) {
    return await apiClient.delete(`/VideoGroup/${id}`);
  }

  async getVideoGroupVideos(videoGroupId) {
    return await apiClient.get(`/VideoGroup/${videoGroupId}/videos`);
  }

  async getVideoGroupsByProject(projectId) {
    return await apiClient.get(`/Project/${projectId}/videogroups`);
  }
}

export default new VideoGroupService();
