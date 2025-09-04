import apiClient from './ApiClient.js';

/**
 * ProjectService - Handles project operations
 * Maps to ProjectController endpoints
 */
class ProjectService {

  /**
   * Get all projects
   * GET /api/Project
   * @returns {Promise<Array>} Array of ProjectResponse objects
   */
  async getAll() {
    try {
      console.log('ProjectService.getAll - making API call to:', apiClient.client.defaults.baseURL + '/Project');
      return await apiClient.get('/Project');
    } catch (error) {
      console.error('ProjectService.getAll error:', error);
      throw new Error(`Failed to get projects: ${error.message}`);
    }
  }

  /**
   * Get specific project by ID
   * GET /api/Project/{id}
   * @param {number} id - Project ID
   * @returns {Promise<Object>} ProjectResponse object
   */
  async getById(id) {
    try {
      return await apiClient.get(`/Project/${id}`);
    } catch (error) {
      throw new Error(`Failed to get project ${id}: ${error.message}`);
    }
  }

  /**
   * Create new project
   * POST /api/Project
   * @param {Object} projectRequest - Project creation request
   * @param {string} projectRequest.name - Project name
   * @param {string} projectRequest.description - Project description
   * @returns {Promise<Object>} ProjectResponse object
   */
  async create(projectRequest) {
    try {
      return await apiClient.post('/Project', projectRequest);
    } catch (error) {
      throw new Error(`Failed to create project: ${error.message}`);
    }
  }

  /**
   * Update project
   * PUT /api/Project/{id}
   * @param {number} id - Project ID
   * @param {Object} projectRequest - Project update request
   * @returns {Promise<void>}
   */
  async update(id, projectRequest) {
    try {
      await apiClient.put(`/Project/${id}`, projectRequest);
    } catch (error) {
      throw new Error(`Failed to update project ${id}: ${error.message}`);
    }
  }

  /**
   * Delete project
   * DELETE /api/Project/{id}
   * @param {number} id - Project ID to delete
   * @returns {Promise<void>}
   */
  async delete(id) {
    try {
      await apiClient.delete(`/Project/${id}`);
    } catch (error) {
      throw new Error(`Failed to delete project ${id}: ${error.message}`);
    }
  }

  /**
   * Get project labelers
   * GET /api/Project/{id}/Labelers
   * @param {number} id - Project ID
   * @returns {Promise<Array>} Array of LabelerResponse objects
   */
  async getUsers(id) {
    try {
      return await apiClient.get(`/Project/${id}/Labelers`);
    } catch (error) {
      throw new Error(`Failed to get labelers for project ${id}: ${error.message}`);
    }
  }

  /**
   * Get project subjects
   * GET /api/Project/{id}/subjects
   * @param {number} id - Project ID
   * @returns {Promise<Array>} Array of SubjectResponse objects
   */
  async getSubjects(id) {
    try {
      return await apiClient.get(`/Project/${id}/subjects`);
    } catch (error) {
      throw new Error(`Failed to get subjects for project ${id}: ${error.message}`);
    }
  }

  /**
   * Get project video groups
   * GET /api/Project/{id}/VideoGroups
   * @param {number} id - Project ID
   * @returns {Promise<Array>} Array of VideoGroupResponse objects
   */
  async getVideoGroups(id) {
    try {
      return await apiClient.get(`/Project/${id}/VideoGroups`);
    } catch (error) {
      throw new Error(`Failed to get video groups for project ${id}: ${error.message}`);
    }
  }

  /**
   * Get project reports
   * GET /api/Project/{id}/reports
   * @param {number} id - Project ID
   * @returns {Promise<Array>} Array of ProjectReportResponse objects
   */
  async getReports(id) {
    try {
      return await apiClient.get(`/Project/${id}/reports`);
    } catch (error) {
      throw new Error(`Failed to get reports for project ${id}: ${error.message}`);
    }
  }

  /**
   * Add user to project
   * POST /api/Project/{id}/users/{userId}
   * @param {number} id - Project ID
   * @param {string} userId - User ID to add
   * @returns {Promise<void>}
   */
  async addUser(id, userId) {
    try {
      await apiClient.post(`/Project/${id}/users/${userId}`);
    } catch (error) {
      throw new Error(`Failed to add user ${userId} to project ${id}: ${error.message}`);
    }
  }

  /**
   * Remove user from project
   * DELETE /api/Project/{id}/users/{userId}
   * @param {number} id - Project ID
   * @param {string} userId - User ID to remove
   * @returns {Promise<void>}
   */
  async removeUser(id, userId) {
    try {
      await apiClient.delete(`/Project/${id}/users/${userId}`);
    } catch (error) {
      throw new Error(`Failed to remove user ${userId} from project ${id}: ${error.message}`);
    }
  }

  /**
   * Get project assignments
   * GET /api/Project/{id}/assignments
   * @param {number} id - Project ID
   * @returns {Promise<Array>} Array of SubjectVideoGroupAssignmentResponse objects
   */
  async getAssignments(id) {
    try {
      return await apiClient.get(`/Project/${id}/SubjectVideoGroupAssignments`);
    } catch (error) {
      throw new Error(`Failed to get assignments for project ${id}: ${error.message}`);
    }
  }

  /**
   * Get project statistics
   * GET /api/Project/{id}/stats
   * @param {number} id - Project ID
   * @returns {Promise<Object>} Project statistics object
   */
  async getStats(id) {
    try {
      return await apiClient.get(`/Project/${id}/stats`);
    } catch (error) {
      throw new Error(`Failed to get statistics for project ${id}: ${error.message}`);
    }
  }

  /**
   * Get unassigned labelers for project
   * GET /api/Project/{id}/unassigned-labelers
   * @param {number} id - Project ID
   * @returns {Promise<Array>} Array of unassigned labeler objects
   */
  async getUnassignedLabelers(id) {
    try {
      return await apiClient.get(`/Project/${id}/unassigned-labelers`);
    } catch (error) {
      throw new Error(`Failed to get unassigned labelers for project ${id}: ${error.message}`);
    }
  }

  /**
   * Distribute labelers across assignments
   * POST /api/Project/{id}/distribute
   * @param {number} id - Project ID
   * @returns {Promise<void>}
   */
  async distributeLabelers(id) {
    try {
      await apiClient.post(`/Project/${id}/distribute`);
    } catch (error) {
      throw new Error(`Failed to distribute labelers for project ${id}: ${error.message}`);
    }
  }

  /**
   * Unassign all labelers from assignments
   * POST /api/Project/{id}/unassign-all
   * @param {number} id - Project ID
   * @returns {Promise<void>}
   */
  async unassignAllLabelers(id) {
    try {
      await apiClient.post(`/Project/${id}/unassign-all`);
    } catch (error) {
      throw new Error(`Failed to unassign all labelers for project ${id}: ${error.message}`);
    }
  }
}

export default new ProjectService();
