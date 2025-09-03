import apiClient from './ApiClient.js';

/**
 * AssignedLabelService - Handles assigned label operations
 * Maps to AssignedLabelController endpoints
 */
class AssignedLabelService {

  /**
   * Get all assigned labels
   * GET /api/AssignedLabel
   * @returns {Promise<Array>} Array of AssignedLabelResponse objects
   */
  async getAll() {
    try {
      return await apiClient.get('/AssignedLabel');
    } catch (error) {
      throw new Error(`Failed to get assigned labels: ${error.message}`);
    }
  }

  /**
   * Get specific assigned label by ID
   * GET /api/AssignedLabel/{id}
   * @param {number} id - Assigned label ID
   * @returns {Promise<Object>} AssignedLabelResponse object
   */
  async getById(id) {
    try {
      return await apiClient.get(`/AssignedLabel/${id}`);
    } catch (error) {
      throw new Error(`Failed to get assigned label ${id}: ${error.message}`);
    }
  }

  /**
   * Create new assigned label
   * POST /api/AssignedLabel
   * @param {Object} assignedLabelRequest - Assigned label creation request
   * @param {number} assignedLabelRequest.labelId - Label ID
   * @param {number} assignedLabelRequest.videoId - Video ID
   * @param {number} assignedLabelRequest.subjectId - Subject ID
   * @param {string} assignedLabelRequest.labelerId - Labeler ID
   * @param {number} assignedLabelRequest.timestamp - Timestamp in video
   * @param {string} assignedLabelRequest.value - Label value
   * @returns {Promise<Object>} AssignedLabelResponse object
   */
  async create(assignedLabelRequest) {
    try {
      return await apiClient.post('/AssignedLabel', assignedLabelRequest);
    } catch (error) {
      throw new Error(`Failed to create assigned label: ${error.message}`);
    }
  }

  /**
   * Delete assigned label
   * DELETE /api/AssignedLabel/{id}
   * @param {number} id - Assigned label ID to delete
   * @returns {Promise<void>}
   */
  async delete(id) {
    try {
      await apiClient.delete(`/AssignedLabel/${id}`);
    } catch (error) {
      throw new Error(`Failed to delete assigned label ${id}: ${error.message}`);
    }
  }
}

export default new AssignedLabelService();