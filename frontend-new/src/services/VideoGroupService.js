import apiClient from './ApiClient.js';

/**
 * VideoGroupService - Handles video group operations
 * Maps to VideoGroupController endpoints
 * Read operations are authenticated, write operations require Admin/Scientist
 */
class VideoGroupService {

  /**
   * Get all video groups
   * GET /api/VideoGroup
   * @returns {Promise<Array>} Array of VideoGroupResponse objects
   */
  async getAll() {
    try {
      return await apiClient.get('/VideoGroup');
    } catch (error) {
      throw new Error(`Failed to get video groups: ${error.message}`);
    }
  }

  /**
   * Get specific video group by ID
   * GET /api/VideoGroup/{id}
   * @param {number} id - Video group ID
   * @returns {Promise<Object>} VideoGroupResponse object
   */
  async getById(id) {
    try {
      return await apiClient.get(`/VideoGroup/${id}`);
    } catch (error) {
      throw new Error(`Failed to get video group ${id}: ${error.message}`);
    }
  }

  /**
   * Create new video group
   * POST /api/VideoGroup
   * @param {Object} videoGroupRequest - Video group creation request
   * @param {string} videoGroupRequest.name - Video group name
   * @param {string} videoGroupRequest.description - Video group description
   * @param {number} videoGroupRequest.projectId - Project ID
   * @returns {Promise<Object>} VideoGroupResponse object
   */
  async create(videoGroupRequest) {
    try {
      return await apiClient.post('/VideoGroup', videoGroupRequest);
    } catch (error) {
      throw new Error(`Failed to create video group: ${error.message}`);
    }
  }

  /**
   * Update video group
   * PUT /api/VideoGroup/{id}
   * @param {number} id - Video group ID
   * @param {Object} videoGroupRequest - Video group update request
   * @returns {Promise<void>}
   */
  async update(id, videoGroupRequest) {
    try {
      await apiClient.put(`/VideoGroup/${id}`, videoGroupRequest);
    } catch (error) {
      throw new Error(`Failed to update video group ${id}: ${error.message}`);
    }
  }

  /**
   * Delete video group
   * DELETE /api/VideoGroup/{id}
   * @param {number} id - Video group ID to delete
   * @returns {Promise<void>}
   */
  async delete(id) {
    try {
      await apiClient.delete(`/VideoGroup/${id}`);
    } catch (error) {
      throw new Error(`Failed to delete video group ${id}: ${error.message}`);
    }
  }

  /**
   * Get videos in video group
   * GET /api/VideoGroup/{id}/videos
   * @param {number} id - Video group ID
   * @returns {Promise<Array>} Array of VideoResponse objects
   */
  async getVideos(id) {
    try {
      return await apiClient.get(`/VideoGroup/${id}/videos`);
    } catch (error) {
      throw new Error(`Failed to get videos for video group ${id}: ${error.message}`);
    }
  }

}

export default new VideoGroupService();
