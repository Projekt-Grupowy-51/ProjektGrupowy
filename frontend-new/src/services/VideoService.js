import apiClient from './ApiClient.js';

class VideoService {
  async getVideos(projectId = null) {
    const params = projectId ? { projectId } : {};
    const response = await apiClient.get('/Video', { params });
    return response.data;
  }

  async getVideo(id) {
    const response = await apiClient.get(`/Video/${id}`);
    return response.data;
  }

  async createVideo(videoData) {
    const response = await apiClient.post('/Video', videoData);
    return response.data;
  }

  async updateVideo(id, videoData) {
    const response = await apiClient.put(`/Video/${id}`, videoData);
    return response.data;
  }

  async deleteVideo(id) {
    await apiClient.delete(`/Video/${id}`);
    return { success: true };
  }

  async getVideoGroups(projectId = null) {
    const params = projectId ? { projectId } : {};
    const response = await apiClient.get('/VideoGroup', { params });
    return response.data;
  }

  async getVideoGroup(id) {
    const response = await apiClient.get(`/VideoGroup/${id}`);
    return response.data;
  }

  async createVideoGroup(videoGroupData) {
    const response = await apiClient.post('/VideoGroup', videoGroupData);
    return response.data;
  }

  async updateVideoGroup(id, videoGroupData) {
    const response = await apiClient.put(`/VideoGroup/${id}`, videoGroupData);
    return response.data;
  }

  async deleteVideoGroup(id) {
    await apiClient.delete(`/VideoGroup/${id}`);
    return { success: true };
  }

  async getVideoAssignedLabels(videoId) {
    const response = await apiClient.get(`/Video/${videoId}/assigned-labels`);
    return response.data;
  }
}

export default new VideoService();
