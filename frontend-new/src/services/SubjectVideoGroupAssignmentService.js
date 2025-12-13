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
      return await apiClient.get('/subject-video-group-assignments');
    } catch (error) {
      throw new Error(`Failed to get subject video group assignments: ${error.message}`);
    }
  }

  /**
   * Get all assignments for current labeler with completion status
   * GET /api/subject-video-group-assignments/labeler
   * @returns {Promise<Array>} Array of LabelerAssignmentResponse objects
   */
  async getLabelerAssignments() {
    try {
      return await apiClient.get('/subject-video-group-assignments/labeler');
    } catch (error) {
      throw new Error(`Failed to get labeler assignments: ${error.message}`);
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
      return await apiClient.get(`/subject-video-group-assignments/${id}`);
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
      return await apiClient.post('/subject-video-group-assignments', assignmentRequest);
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
      await apiClient.put(`/subject-video-group-assignments/${id}`, assignmentRequest);
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
      await apiClient.delete(`/subject-video-group-assignments/${id}`);
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
      return await apiClient.get(`/subject-video-group-assignments/${id}/labelers`);
    } catch (error) {
      throw new Error(`Failed to get labelers for assignment ${id}: ${error.message}`);
    }
  }

  /**
   * Add labeler to assignment
   * POST /api/SubjectVideoGroupAssignment/{id}/assign-labeler/{labelerId}
   * @param {number} id - Assignment ID
   * @param {string} labelerId - Labeler ID
   * @returns {Promise<void>}
   */
  async addLabeler(id, labelerId) {
    try {
      await apiClient.post(`/subject-video-group-assignments/${id}/assign-labeler/${labelerId}`);
    } catch (error) {
      throw new Error(`Failed to add labeler ${labelerId} to assignment ${id}: ${error.message}`);
    }
  }

  /**
   * Remove labeler from assignment
   * DELETE /api/SubjectVideoGroupAssignment/{id}/unassign-labeler/{labelerId}
   * @param {number} id - Assignment ID
   * @param {string} labelerId - Labeler ID
   * @returns {Promise<void>}
   */
  async removeLabeler(id, labelerId) {
    try {
      await apiClient.delete(`/subject-video-group-assignments/${id}/unassign-labeler/${labelerId}`);
    } catch (error) {
      throw new Error(`Failed to remove labeler ${labelerId} from assignment ${id}: ${error.message}`);
    }
  }

  /**
   * Get statistics for assignment
   * GET /api/subject-video-group-assignments/{id}/statistics
   * @param {number} id - Assignment ID
   * @returns {Promise<Object>} Assignment statistics with videos, labelers, and completion data
   */
  async getStatistics(id) {
    try {
      const data = await apiClient.get(`/subject-video-group-assignments/${id}/statistics`);
      // Convert snake_case to camelCase
      return {
        totalVideos: data.total_videos,
        totalLabels: data.total_labels,
        assignedLabelersCount: data.assigned_labelers_count,
        completionPercentage: data.completion_percentage,
        videos: data.videos?.map(v => ({
          id: v.id,
          title: v.title,
          labelsReceived: v.labels_received,
          expectedLabels: v.expected_labels,
          completionPercentage: v.completion_percentage
        })) || [],
        topLabelers: data.top_labelers || {},
        allLabelers: data.all_labelers?.map(l => ({
          id: l.id,
          name: l.name,
          email: l.email,
          labelCount: l.label_count,
          completionPercentage: l.completion_percentage
        })) || [],
        videoStatus: {
          completed: data.video_status?.completed || 0,
          inProgress: data.video_status?.in_progress || 0,
          notStarted: data.video_status?.not_started || 0
        }
      };
    } catch (error) {
      throw new Error(`Failed to get statistics for assignment ${id}: ${error.message}`);
    }
  }

  /**
   * Toggle completion status for an assignment
   * POST /api/subject-video-group-assignments/{id}/completion
   * @param {number} id - Assignment ID
   * @param {boolean} isCompleted - Completion status
   * @returns {Promise<void>}
   */
  async toggleCompletion(id, isCompleted) {
    try {
      await apiClient.post(`/subject-video-group-assignments/${id}/completion`, {
        is_completed: isCompleted
      });
    } catch (error) {
      throw new Error(`Failed to toggle completion for assignment ${id}: ${error.message}`);
    }
  }

  /**
   * Get statistics for a specific labeler in an assignment
   * GET /api/subject-video-group-assignments/{id}/labelers/{labelerId}/statistics
   * @param {number} assignmentId - Assignment ID
   * @param {string} labelerId - Labeler ID
   * @returns {Promise<Object>} Labeler statistics for this assignment
   */
  async getLabelerStatistics(assignmentId, labelerId) {
    try {
      const data = await apiClient.get(`/subject-video-group-assignments/${assignmentId}/labelers/${labelerId}/statistics`);
      return {
        labelerId: data.labeler_id,
        labelerName: data.labeler_name,
        labelerEmail: data.labeler_email,
        assignmentId: data.assignment_id,
        subjectName: data.subject_name,
        videoGroupName: data.video_group_name,
        totalVideos: data.total_videos,
        labeledVideos: data.labeled_videos,
        totalLabels: data.total_labels,
        isCompleted: data.is_completed,
        videos: data.videos?.map(v => ({
          videoId: v.video_id,
          videoTitle: v.video_title,
          labelCount: v.label_count,
          hasLabeled: v.has_labeled
        })) || []
      };
    } catch (error) {
      throw new Error(`Failed to get labeler statistics for assignment ${assignmentId}: ${error.message}`);
    }
  }
}

export default new SubjectVideoGroupAssignmentService();