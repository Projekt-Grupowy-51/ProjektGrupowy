import apiClient from './ApiClient.js';

/**
 * SubjectService - Handles research subject operations
 * Maps to SubjectController endpoints
 * Most operations require Admin or Scientist role
 */
class SubjectService {

  /**
   * Get all subjects
   * GET /api/Subject
   * @returns {Promise<Array>} Array of SubjectResponse objects
   */
  async getAll() {
    try {
      return await apiClient.get('/subjects');
    } catch (error) {
      throw new Error(`Failed to get subjects: ${error.message}`);
    }
  }

  /**
   * Get specific subject by ID
   * GET /api/Subject/{id}
   * @param {number} id - Subject ID
   * @returns {Promise<Object>} SubjectResponse object
   */
  async getById(id) {
    try {
      return await apiClient.get(`/subjects/${id}`);
    } catch (error) {
      throw new Error(`Failed to get subject ${id}: ${error.message}`);
    }
  }

  /**
   * Create new subject
   * POST /api/Subject
   * @param {Object} subjectRequest - Subject creation request
   * @param {string} subjectRequest.name - Subject name
   * @param {string} subjectRequest.description - Subject description
   * @param {number} subjectRequest.projectId - Project ID
   * @returns {Promise<Object>} SubjectResponse object
   */
  async create(subjectRequest) {
    try {
      return await apiClient.post('/subjects', subjectRequest);
    } catch (error) {
      throw new Error(`Failed to create subject: ${error.message}`);
    }
  }

  /**
   * Update subject
   * PUT /api/Subject/{id}
   * @param {number} id - Subject ID
   * @param {Object} subjectRequest - Subject update request
   * @returns {Promise<void>}
   */
  async update(id, subjectRequest) {
    try {
      await apiClient.put(`/subjects/${id}`, subjectRequest);
    } catch (error) {
      throw new Error(`Failed to update subject ${id}: ${error.message}`);
    }
  }

  /**
   * Delete subject
   * DELETE /api/Subject/{id}
   * @param {number} id - Subject ID to delete
   * @returns {Promise<void>}
   */
  async delete(id) {
    try {
      await apiClient.delete(`/subjects/${id}`);
    } catch (error) {
      throw new Error(`Failed to delete subject ${id}: ${error.message}`);
    }
  }

  /**
   * Get subject labels
   * GET /api/Subject/{id}/label
   * @param {number} id - Subject ID
   * @returns {Promise<Array>} Array of LabelResponse objects
   */
  async getLabels(id) {
    try {
      return await apiClient.get(`/subjects/${id}/labels`);
    } catch (error) {
      throw new Error(`Failed to get labels for subject ${id}: ${error.message}`);
    }
  }

}

export default new SubjectService();
