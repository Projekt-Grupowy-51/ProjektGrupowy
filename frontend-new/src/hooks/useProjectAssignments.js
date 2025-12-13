import { useState, useEffect } from 'react';
import ProjectService from '../services/ProjectService.js';
import SubjectVideoGroupAssignmentService from '../services/SubjectVideoGroupAssignmentService.js';

export const useProjectAssignments = (projectId) => {
  const [assignments, setAssignments] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const fetchAssignments = () => {
    if (!projectId) return Promise.resolve();
    
    setLoading(true);
    setError(null);
    return ProjectService.getAssignments(projectId)
      .then(setAssignments)
      .catch(err => setError(err.message))
      .finally(() => setLoading(false));
  };

  const deleteAssignment = async (assignmentId) => {
    return SubjectVideoGroupAssignmentService.delete(assignmentId)
      .then(() => fetchAssignments());
  };

  useEffect(() => {
    fetchAssignments();
  }, [projectId]);

  return {
    assignments,
    loading,
    error,
    refetch: fetchAssignments,
    deleteAssignment
  };
};