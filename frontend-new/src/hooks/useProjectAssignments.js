import { useCallback } from 'react';
import { useDataFetching } from './common';
import ProjectService from '../services/ProjectService.js';

export const useProjectAssignments = (projectId) => {
  const fetchAssignments = useCallback(async () => {
    if (!projectId) return [];
    return await ProjectService.getAssignments(projectId);
  }, [projectId]);

  const { data: assignments, loading, error, refetch } = useDataFetching(
    projectId ? fetchAssignments : null,
    [projectId]
  );

  return {
    assignments: assignments || [],
    loading,
    error,
    refetch
  };
};