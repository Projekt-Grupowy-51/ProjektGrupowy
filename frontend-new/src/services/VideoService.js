import apiClient from './ApiClient.js';

/**
 * VideoService - Handles video file operations and streaming
 * Maps to VideoController endpoints
 * Streaming and labeling are authenticated, CRUD requires Admin/Scientist
 */
class VideoService {

  /**
   * Get all videos
   * GET /api/Video
   * @returns {Promise<Array>} Array of VideoResponse objects
   */
  async getAll() {
    try {
      return await apiClient.get('/Video');
    } catch (error) {
      throw new Error(`Failed to get videos: ${error.message}`);
    }
  }

  /**
   * Get batch of videos by video group and position
   * GET /api/Video/batch/{videoGroupId}/{positionInQueue}
   * @param {number} videoGroupId - Video group ID
   * @param {number} positionInQueue - Position in queue
   * @returns {Promise<Array>} Array of VideoResponse objects
   */
  async getBatch(videoGroupId, positionInQueue) {
    try {
      return await apiClient.get(`/Video/batch/${videoGroupId}/${positionInQueue}`);
    } catch (error) {
      throw new Error(`Failed to get video batch: ${error.message}`);
    }
  }

  /**
   * Get specific video by ID
   * GET /api/Video/{id}
   * @param {number} id - Video ID
   * @returns {Promise<Object>} VideoResponse object
   */
  async getById(id) {
    try {
      return await apiClient.get(`/Video/${id}`);
    } catch (error) {
      throw new Error(`Failed to get video ${id}: ${error.message}`);
    }
  }

  /**
   * Create new video
   * POST /api/Video
   * @param {Object} videoRequest - Video creation request
   * @param {string} videoRequest.name - Video name
   * @param {string} videoRequest.title - Video title
   * @param {string} videoRequest.description - Video description
   * @param {string} videoRequest.url - Video URL
   * @param {number} videoRequest.duration - Video duration in seconds
   * @param {number} videoRequest.videoGroupId - Video group ID
   * @param {number} videoRequest.positionInQueue - Position in queue
   * @returns {Promise<Object>} VideoResponse object
   */
  async create(videoRequest) {
    try {
      return await apiClient.post('/Video', videoRequest);
    } catch (error) {
      throw new Error(`Failed to create video: ${error.message}`);
    }
  }

  /**
   * Update video
   * PUT /api/Video/{id}
   * @param {number} id - Video ID
   * @param {Object} videoRequest - Video update request
   * @returns {Promise<void>}
   */
  async update(id, videoRequest) {
    try {
      await apiClient.put(`/Video/${id}`, videoRequest);
    } catch (error) {
      throw new Error(`Failed to update video ${id}: ${error.message}`);
    }
  }

  /**
   * Get video stream URL
   * GET /api/Video/{id}/stream
   * @param {number} id - Video ID
   * @returns {string} Video stream URL
   */
  getStreamUrl(id) {
    return `${apiClient.client.defaults.baseURL}/Video/${id}/stream`;
  }

  /**
   * Delete video
   * DELETE /api/Video/{id}
   * @param {number} id - Video ID to delete
   * @returns {Promise<void>}
   */
  async delete(id) {
    try {
      await apiClient.delete(`/Video/${id}`);
    } catch (error) {
      throw new Error(`Failed to delete video ${id}: ${error.message}`);
    }
  }

  /**
   * Get video's assigned labels
   * GET /api/Video/{id}/assignedlabels
   * @param {number} id - Video ID
   * @returns {Promise<Array>} Array of AssignedLabelResponse objects
   */
  async getAssignedLabels(id) {
    try {
      return await apiClient.get(`/Video/${id}/assignedlabels`);
    } catch (error) {
      throw new Error(`Failed to get assigned labels for video ${id}: ${error.message}`);
    }
  }

  /**
   * Get labels by video and subject
   * GET /api/Video/{videoId}/{subjectId}/assignedlabels
   * @param {number} videoId - Video ID
   * @param {number} subjectId - Subject ID
   * @returns {Promise<Array>} Array of AssignedLabelResponse objects
   */
  async getAssignedLabelsBySubject(videoId, subjectId) {
    try {
      return await apiClient.get(`/Video/${videoId}/${subjectId}/assignedlabels`);
    } catch (error) {
      throw new Error(`Failed to get assigned labels for video ${videoId} and subject ${subjectId}: ${error.message}`);
    }
  }

}

export default new VideoService();
