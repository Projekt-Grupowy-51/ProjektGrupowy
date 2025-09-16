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
      return await apiClient.get('/videos');
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
      return await apiClient.get(`/videos/batch/${videoGroupId}/${positionInQueue}`);
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
      return await apiClient.get(`/videos/${id}`);
    } catch (error) {
      throw new Error(`Failed to get video ${id}: ${error.message}`);
    }
  }

  /**
   * Create new video (upload file)
   * POST /api/Video
   * @param {Object} videoRequest - Video creation request
   * @param {string} videoRequest.title - Video title
   * @param {File} videoRequest.file - Video file
   * @param {number} videoRequest.videoGroupId - Video group ID
   * @param {number} videoRequest.positionInQueue - Position in queue
   * @returns {Promise<Object>} VideoResponse object
   */
  async create(videoRequest) {
    try {
      const formData = new FormData();
      formData.append('Title', videoRequest.title);
      formData.append('File', videoRequest.file);
      formData.append('VideoGroupId', videoRequest.videoGroupId.toString());
      formData.append('PositionInQueue', videoRequest.positionInQueue.toString());

      return await apiClient.client.post('/videos', formData, {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      });
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
      await apiClient.put(`/videos/${id}`, videoRequest);
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
    return `${apiClient.client.defaults.baseURL}/videos/${id}/stream`;
  }

  /**
   * Get video stream as blob with authentication
   * @param {number} id - Video ID
   * @returns {Promise<string>} Blob URL
   */
  async getStreamBlob(id) {
    try {
      const response = await apiClient.client.get(`/videos/${id}/stream`, {
        responseType: 'blob'
      });
      return URL.createObjectURL(response.data);
    } catch (error) {
      throw new Error(`Failed to get video stream ${id}: ${error.message}`);
    }
  }

  /**
   * Delete video
   * DELETE /api/Video/{id}
   * @param {number} id - Video ID to delete
   * @returns {Promise<void>}
   */
  async delete(id) {
    try {
      await apiClient.delete(`/videos/${id}`);
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
      return await apiClient.get(`/videos/${id}/assigned-labels`);
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
      return await apiClient.get(`/videos/${videoId}/${subjectId}/assigned-labels`);
    } catch (error) {
      throw new Error(`Failed to get assigned labels for video ${videoId} and subject ${subjectId}: ${error.message}`);
    }
  }

}

export default new VideoService();
