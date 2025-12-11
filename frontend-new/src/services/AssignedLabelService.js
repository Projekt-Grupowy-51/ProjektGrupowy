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
      return await apiClient.get('/assigned-labels');
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
      return await apiClient.get(`/assigned-labels/${id}`);
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
   * @param {string} assignedLabelRequest.start - Start time for the assigned label
   * @param {string} assignedLabelRequest.end - End time for the assigned label
   * @returns {Promise<Object>} AssignedLabelResponse object
   */
  async create(assignedLabelRequest) {
    try {
      return await apiClient.post('/assigned-labels', assignedLabelRequest);
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
      await apiClient.delete(`/assigned-labels/${id}`);
    } catch (error) {
      throw new Error(`Failed to delete assigned label ${id}: ${error.message}`);
    }
  }
}

export default new AssignedLabelService();