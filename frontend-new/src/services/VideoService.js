import apiClient from "./ApiClient.js";

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
      return await apiClient.get("/videos");
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
      return await apiClient.get(
        `/videos/batch/${videoGroupId}/${positionInQueue}`
      );
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
      formData.append("Title", videoRequest.title);
      formData.append("File", videoRequest.file);
      formData.append("VideoGroupId", videoRequest.videoGroupId.toString());
      formData.append(
        "PositionInQueue",
        videoRequest.positionInQueue.toString()
      );

      return await apiClient.client.post("/videos", formData, {
        headers: {
          "Content-Type": "multipart/form-data",
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
   * @param {Object} videoRequest - Video update request with [FromForm] binding
   * @param {string} videoRequest.title - Video title
   * @param {number} videoRequest.videoGroupId - Video group ID
   * @param {number} videoRequest.positionInQueue - Position in queue
   * @param {File} [videoRequest.file] - Optional video file for replacement
   * @returns {Promise<void>}
   */
  async update(id, videoRequest) {
    try {
      const formData = new FormData();
      formData.append("Title", videoRequest.title);
      formData.append("VideoGroupId", videoRequest.videoGroupId.toString());
      formData.append(
        "PositionInQueue",
        videoRequest.positionInQueue.toString()
      );

      // Only append file if provided
      if (videoRequest.file) {
        formData.append("File", videoRequest.file);
      }

      await apiClient.client.put(`/videos/${id}`, formData, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      });
    } catch (error) {
      throw new Error(`Failed to update video ${id}: ${error.message}`);
    }
  }

  /**
   * Get video stream URL (non-authenticated, for direct usage)
   * GET /api/Video/{id}/stream?quality={quality}
   * @param {number} id - Video ID
   * @param {string} [quality] - Optional video quality (e.g., "288x512"). If not provided, returns original quality
   * @returns {string} Video stream URL
   */
  getStreamUrl(id, quality) {
    const baseUrl = `${apiClient.client.defaults.baseURL}/videos/${id}/stream`;
    return quality ? `${baseUrl}?quality=${quality}` : baseUrl;
  }

  /**
   * Get pre-signed video stream URL with authentication
   * GET /api/Video/{id}/stream?quality={quality}
   * @param {number} id - Video ID
   * @param {string|null} [quality] - Optional video quality (e.g., "288x512"). Pass null or omit for original quality.
   * @returns {Promise<string>} Pre-signed URL for direct video streaming
   */
  async getStreamBlob(id, quality = null) {
    try {
      // Only add quality parameter if it's a non-null, non-empty string
      const params = quality ? { quality } : {};
      const response = await apiClient.client.get(`/videos/${id}/stream`, {
        params,
      });
      // Response is JSON with { url: "pre-signed-url" }
      if (response.data && response.data.url) {
        return response.data.url;
      }
      throw new Error("Invalid response format: missing URL");
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
      throw new Error(
        `Failed to get assigned labels for video ${id}: ${error.message}`
      );
    }
  }

  /**
   * Get video's assigned labels with pagination
   * GET /api/Video/{id}/assigned-labels/page?pageNumber={pageNumber}&pageSize={pageSize}
   * @param {number} id - Video ID
   * @param {number} pageNumber - Page number (1-based)
   * @param {number} pageSize - Number of items per page
   * @returns {Promise<Object>} Paginated response with items and pagination metadata
   */
  async getAssignedLabelsPaginated(id, pageNumber = 1, pageSize = 10) {
    try {
      return await apiClient.get(`/videos/${id}/assigned-labels/page`, {
        params: {
          pageNumber,
          pageSize,
        },
      });
    } catch (error) {
      throw new Error(
        `Failed to get assigned labels for video ${id}: ${error.message}`
      );
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
      return await apiClient.get(
        `/videos/${videoId}/${subjectId}/assigned-labels`
      );
    } catch (error) {
      throw new Error(
        `Failed to get assigned labels for video ${videoId} and subject ${subjectId}: ${error.message}`
      );
    }
  }

  /**
   * Get labels by video IDs and subject with pagination
   * GET /api/videos/{subjectId}/assigned-labels/page/video-ids?videoIds={videoIds}&pageNumber={pageNumber}&pageSize={pageSize}
   * @param {Array<number>} videoIds - Array of Video IDs
   * @param {number} subjectId - Subject ID
   * @param {number} pageNumber - Page number (1-based)
   * @param {number} pageSize - Number of items per page
   * @returns {Promise<Object>} Paginated response with assignedLabels array and totalLabelCount
   */
  async getAssignedLabelsBySubjectPaginated(
    videoIds,
    subjectId,
    pageNumber = 1,
    pageSize = 10
  ) {
    try {
      return await apiClient.get(
        `/videos/${subjectId}/assigned-labels/page/video-ids`,
        {
          params: {
            videoIds: videoIds,
            pageNumber,
            pageSize,
          },
          paramsSerializer: {
            indexes: null, // No brackets for array parameters
          },
        }
      );
    } catch (error) {
      throw new Error(
        `Failed to get assigned labels for subject ${subjectId} and videos ${videoIds}: ${error.message}`
      );
    }
  }

  /**
   * Get label statistics for a video
   * GET /api/videos/{id}/label-statistics
   * @param {number} id - Video ID
   * @returns {Promise<Object>} Statistics object with totalLabels, labelsByType, labelsBySubject, labelsByLabeler
   */
  async getLabelStatistics(id) {
    try {
      return await apiClient.get(`/videos/${id}/label-statistics`);
    } catch (error) {
      throw new Error(
        `Failed to get label statistics for video ${id}: ${error.message}`
      );
    }
  }
}

export default new VideoService();
