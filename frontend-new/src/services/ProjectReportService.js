import apiClient from './ApiClient.js';

/**
 * ProjectReportService - Handles project report operations  
 * Maps to ProjectReportController endpoints
 */
class ProjectReportService {

  /**
   * Get all project reports
   * GET /api/ProjectReport
   * @returns {Promise<Array>} Array of ProjectReportResponse objects
   */
  async getAll() {
    try {
      return await apiClient.get('/ProjectReport');
    } catch (error) {
      throw new Error(`Failed to get project reports: ${error.message}`);
    }
  }

  /**
   * Get specific project report by ID
   * GET /api/ProjectReport/{id}
   * @param {number} id - Project report ID
   * @returns {Promise<Object>} ProjectReportResponse object
   */
  async getById(id) {
    try {
      return await apiClient.get(`/ProjectReport/${id}`);
    } catch (error) {
      throw new Error(`Failed to get project report ${id}: ${error.message}`);
    }
  }

  /**
   * Generate new project report
   * POST /api/ProjectReport/{projectId}/generate-report
   * @param {Object} reportRequest - Report generation request
   * @param {number} reportRequest.projectId - Project ID
   * @param {string} reportRequest.title - Report title (ignored by backend)
   * @param {string} reportRequest.description - Report description (ignored by backend)
   * @returns {Promise<Object>} Report generation response
   */
  async create(reportRequest) {
    try {
      return await apiClient.post(`/ProjectReport/${reportRequest.projectId}/generate-report`);
    } catch (error) {
      throw new Error(`Failed to create project report: ${error.message}`);
    }
  }

  /**
   * Update project report
   * PUT /api/ProjectReport/{id}
   * @param {number} id - Project report ID
   * @param {Object} reportRequest - Project report update request
   * @returns {Promise<void>}
   */
  async update(id, reportRequest) {
    try {
      await apiClient.put(`/ProjectReport/${id}`, reportRequest);
    } catch (error) {
      throw new Error(`Failed to update project report ${id}: ${error.message}`);
    }
  }

  /**
   * Delete project report
   * DELETE /api/ProjectReport/{id}
   * @param {number} id - Project report ID to delete
   * @returns {Promise<void>}
   */
  async delete(id) {
    try {
      await apiClient.delete(`/ProjectReport/${id}`);
    } catch (error) {
      throw new Error(`Failed to delete project report ${id}: ${error.message}`);
    }
  }

  /**
   * Download project report file
   * GET /api/ProjectReport/download/{id}
   * @param {number} id - Project report ID
   * @returns {Promise<Blob>} Report file blob
   */
  async download(id) {
    try {
      return await apiClient.get(`/ProjectReport/download/${id}`, {
        responseType: 'blob'
      });
    } catch (error) {
      throw new Error(`Failed to download project report ${id}: ${error.message}`);
    }
  }

  /**
   * Get download URL for project report
   * @param {number} id - Project report ID
   * @returns {string} Download URL
   */
  getDownloadUrl(id) {
    return `${apiClient.client.defaults.baseURL}/ProjectReport/download/${id}`;
  }
}

export default new ProjectReportService();