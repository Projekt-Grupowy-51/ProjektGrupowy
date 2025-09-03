import apiClient from './ApiClient.js';

/**
 * SubjectVideoGroupAssignmentService - Handles subject-video group assignment operations
 * Maps to SubjectVideoGroupAssignmentController endpoints
 */
class SubjectVideoGroupAssignmentService {

  /**
   * Get all subject video group assignments
   * GET /api/SubjectVideoGroupAssignment
   * @returns {Promise<Array>} Array of SubjectVideoGroupAssignmentResponse objects
   */
  async getAll() {
    try {
      return await apiClient.get('/SubjectVideoGroupAssignment');
    } catch (error) {
      throw new Error(`Failed to get subject video group assignments: ${error.message}`);
    }
  }

  /**
   * Get specific assignment by ID
   * GET /api/SubjectVideoGroupAssignment/{id}
   * @param {number} id - Assignment ID
   * @returns {Promise<Object>} SubjectVideoGroupAssignmentResponse object
   */
  async getById(id) {
    try {
      return await apiClient.get(`/SubjectVideoGroupAssignment/${id}`);
    } catch (error) {
      throw new Error(`Failed to get assignment ${id}: ${error.message}`);
    }
  }

  /**
   * Create new assignment
   * POST /api/SubjectVideoGroupAssignment
   * @param {Object} assignmentRequest - Assignment creation request
   * @param {number} assignmentRequest.subjectId - Subject ID
   * @param {number} assignmentRequest.videoGroupId - Video group ID
   * @returns {Promise<Object>} SubjectVideoGroupAssignmentResponse object
   */
  async create(assignmentRequest) {
    try {
      return await apiClient.post('/SubjectVideoGroupAssignment', assignmentRequest);
    } catch (error) {
      throw new Error(`Failed to create assignment: ${error.message}`);
    }
  }

  /**
   * Update assignment
   * PUT /api/SubjectVideoGroupAssignment/{id}
   * @param {number} id - Assignment ID
   * @param {Object} assignmentRequest - Assignment update request
   * @returns {Promise<void>}
   */
  async update(id, assignmentRequest) {
    try {
      await apiClient.put(`/SubjectVideoGroupAssignment/${id}`, assignmentRequest);
    } catch (error) {
      throw new Error(`Failed to update assignment ${id}: ${error.message}`);
    }
  }

  /**
   * Delete assignment
   * DELETE /api/SubjectVideoGroupAssignment/{id}
   * @param {number} id - Assignment ID to delete
   * @returns {Promise<void>}
   */
  async delete(id) {
    try {
      await apiClient.delete(`/SubjectVideoGroupAssignment/${id}`);
    } catch (error) {
      throw new Error(`Failed to delete assignment ${id}: ${error.message}`);
    }
  }

  /**
   * Get labelers for assignment
   * GET /api/SubjectVideoGroupAssignment/{id}/labelers
   * @param {number} id - Assignment ID
   * @returns {Promise<Array>} Array of labeler objects
   */
  async getLabelers(id) {
    try {
      return await apiClient.get(`/SubjectVideoGroupAssignment/${id}/labelers`);
    } catch (error) {
      throw new Error(`Failed to get labelers for assignment ${id}: ${error.message}`);
    }
  }

  /**
   * Add labeler to assignment
   * POST /api/SubjectVideoGroupAssignment/{id}/labelers/{labelerId}
   * @param {number} id - Assignment ID
   * @param {string} labelerId - Labeler ID
   * @returns {Promise<void>}
   */
  async addLabeler(id, labelerId) {
    try {
      await apiClient.post(`/SubjectVideoGroupAssignment/${id}/labelers/${labelerId}`);
    } catch (error) {
      throw new Error(`Failed to add labeler ${labelerId} to assignment ${id}: ${error.message}`);
    }
  }

  /**
   * Remove labeler from assignment
   * DELETE /api/SubjectVideoGroupAssignment/{id}/labelers/{labelerId}
   * @param {number} id - Assignment ID
   * @param {string} labelerId - Labeler ID
   * @returns {Promise<void>}
   */
  async removeLabeler(id, labelerId) {
    try {
      await apiClient.delete(`/SubjectVideoGroupAssignment/${id}/labelers/${labelerId}`);
    } catch (error) {
      throw new Error(`Failed to remove labeler ${labelerId} from assignment ${id}: ${error.message}`);
    }
  }
}

export default new SubjectVideoGroupAssignmentService();