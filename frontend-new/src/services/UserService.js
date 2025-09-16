import apiClient from './ApiClient.js';

/**
 * UserService - Handles user profile and authentication operations
 * Maps to UserController endpoints
 */
class UserService {
  
  /**
   * Get current user profile
   * GET /api/User/profile
   * @returns {Promise<Object>} User profile with userId, username, roles, isAdmin
   */
  async getProfile() {
    try {
      return await apiClient.get('/users/profile');
    } catch (error) {
      throw new Error(`Failed to get user profile: ${error.message}`);
    }
  }

  /**
   * Check authentication status
   * GET /api/User/check-auth
   * @returns {Promise<Object>} Authentication status info
   */
  async checkAuth() {
    try {
      return await apiClient.get('/users/check-auth');
    } catch (error) {
      throw new Error(`Failed to check authentication: ${error.message}`);
    }
  }
}

export default new UserService();