import apiClient from "./ApiClient.js";

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
      return await apiClient.get(`/access-codes/projects/${projectId}`);
    } catch (error) {
      throw new Error(
        `Failed to get access codes for project ${projectId}: ${error.message}`
      );
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
      await apiClient.put(`/access-codes/${encodeURIComponent(code)}/retire`);
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
      return await apiClient.post("/access-codes/validate", accessCodeRequest);
    } catch (error) {
      throw new Error(`Failed to validate access code: ${error.message}`);
    }
  }

  /**
   * Create new access code for project
   * POST /api/AccessCode/project
   * @param {Object} createAccessCodeRequest - Access code creation request
   * @param {number} createAccessCodeRequest.projectId - Project ID
   * @param {number} createAccessCodeRequest.expiration - Expiration type (0=In14Days, 1=In30Days, 2=Never, 3=Custom)
   * @param {number} createAccessCodeRequest.customExpiration - Custom expiration time in hours (required if expiration=3)
   * @returns {Promise<Object>} AccessCodeResponse object
   */
  async createForProject(createAccessCodeRequest) {
    try {
      const payload = {
        projectId: createAccessCodeRequest.projectId,
        expiration: createAccessCodeRequest.expiration,
        customExpiration: createAccessCodeRequest.customExpiration,
      };
      return await apiClient.post("/access-codes/project", payload);
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
      await apiClient.post("/projects/join", {
        AccessCode: accessCodeRequest.code,
      });
    } catch (error) {
      throw new Error(`Failed to join project: ${error.message}`);
    }
  }
}

export default new AccessCodeService();
