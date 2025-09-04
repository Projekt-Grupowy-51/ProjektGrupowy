import { useCallback } from 'react';
import { useDataFetching } from './common';
import ProjectService from '../services/ProjectService.js';

/**
 * Hook for fetching and managing project statistics
 * @param {number} projectId - Project ID
 * @returns {Object} Object containing stats, loading state, error and refetch function
 */
export const useProjectStats = (projectId) => {
  const fetchStats = useCallback(() => {
    if (!projectId) return null;
    return ProjectService.getStats(projectId);
  }, [projectId]);

  const { data: stats, loading, error, refetch } = useDataFetching(
    projectId ? fetchStats : null,
    [projectId]
  );

  return {
    stats,
    loading,
    error,
    refetchStats: refetch
  };
};