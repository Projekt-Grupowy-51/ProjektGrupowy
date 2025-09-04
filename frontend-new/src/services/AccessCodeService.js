import apiClient from './ApiClient.js';

/**
 * AccessCodeService - Handles project access code operations
 * Maps to AccessCodeController endpoints
 * Requires Admin or Scientist role for all operations
 */
class AccessCodeService {

  /**
   * Get access codes by project ID
   * GET /api/AccessCode/project/{projectId}
   * @param {number} projectId - Project ID
   * @returns {Promise<Array>} Array of AccessCodeResponse objects
   */
  async getByProjectId(projectId) {
    try {
      return await apiClient.get(`/AccessCode/project/${projectId}`);
    } catch (error) {
      throw new Error(`Failed to get access codes for project ${projectId}: ${error.message}`);
    }
  }

  /**
   * Retire an access code
   * PUT /api/AccessCode/{code}/retire
   * @param {string} code - Access code to retire
   * @returns {Promise<void>}
   */
  async retireCode(code) {
    try {
      await apiClient.put(`/AccessCode/${encodeURIComponent(code)}/retire`);
    } catch (error) {
      throw new Error(`Failed to retire access code: ${error.message}`);
    }
  }

  /**
   * Validate access code
   * POST /api/AccessCode/validate
   * @param {Object} accessCodeRequest - Access code validation request
   * @param {string} accessCodeRequest.code - Code to validate
   * @returns {Promise<boolean>} True if code is valid
   */
  async validateCode(accessCodeRequest) {
    try {
      return await apiClient.post('/AccessCode/validate', accessCodeRequest);
    } catch (error) {
      throw new Error(`Failed to validate access code: ${error.message}`);
    }
  }

  /**
   * Create new access code for project
   * POST /api/AccessCode/project
   * @param {Object} createAccessCodeRequest - Access code creation request
   * @param {number} createAccessCodeRequest.projectId - Project ID
   * @param {string} createAccessCodeRequest.description - Code description
   * @returns {Promise<Object>} AccessCodeResponse object
   */
  async createForProject(createAccessCodeRequest) {
    try {
      return await apiClient.post('/AccessCode/project', createAccessCodeRequest);
    } catch (error) {
      throw new Error(`Failed to create access code: ${error.message}`);
    }
  }

  /**
   * Join project using access code
   * POST /api/Project/join
   * @param {Object} accessCodeRequest - Access code join request
   * @param {string} accessCodeRequest.code - Code to use for joining
   * @returns {Promise<void>} Success response
   */
  async joinProject(accessCodeRequest) {
    try {
      await apiClient.post('/Project/join', { AccessCode: accessCodeRequest.code });
    } catch (error) {
      throw new Error(`Failed to join project: ${error.message}`);
    }
  }
}

export default new AccessCodeService();