import apiClient from './ApiClient.js';

/**
 * LabelService - Handles label definition operations
 * Maps to LabelController endpoints
 * Requires Admin or Scientist role for all operations
 */
class LabelService {

  /**
   * Get all labels
   * GET /api/Label
   * @returns {Promise<Array>} Array of LabelResponse objects
   */
  async getAll() {
    try {
      return await apiClient.get('/Label');
    } catch (error) {
      throw new Error(`Failed to get labels: ${error.message}`);
    }
  }

  /**
   * Get specific label by ID
   * GET /api/Label/{id}
   * @param {number} id - Label ID
   * @returns {Promise<Object>} LabelResponse object
   */
  async getById(id) {
    try {
      return await apiClient.get(`/Label/${id}`);
    } catch (error) {
      throw new Error(`Failed to get label ${id}: ${error.message}`);
    }
  }

  /**
   * Create new label
   * POST /api/Label
   * @param {Object} labelRequest - Label creation request
   * @param {string} labelRequest.name - Label name
   * @param {string} labelRequest.description - Label description
   * @param {string} labelRequest.colorHex - Label color in hex format
   * @param {string} labelRequest.type - Label type (range/point)
   * @param {string} labelRequest.shortcut - Keyboard shortcut
   * @param {number} labelRequest.subjectId - Subject ID
   * @returns {Promise<Object>} LabelResponse object
   */
  async create(labelRequest) {
    try {
      return await apiClient.post('/Label', labelRequest);
    } catch (error) {
      throw new Error(`Failed to create label: ${error.message}`);
    }
  }

  /**
   * Update label
   * PUT /api/Label/{id}
   * @param {number} id - Label ID
   * @param {Object} labelRequest - Label update request
   * @returns {Promise<void>}
   */
  async update(id, labelRequest) {
    try {
      await apiClient.put(`/Label/${id}`, labelRequest);
    } catch (error) {
      throw new Error(`Failed to update label ${id}: ${error.message}`);
    }
  }

  /**
   * Delete label
   * DELETE /api/Label/{id}
   * @param {number} id - Label ID to delete
   * @returns {Promise<void>}
   */
  async delete(id) {
    try {
      await apiClient.delete(`/Label/${id}`);
    } catch (error) {
      throw new Error(`Failed to delete label ${id}: ${error.message}`);
    }
  }

}

export default new LabelService();
