import { useCallback } from 'react';
import { useDataFetching } from './common';
import ProjectService from '../services/ProjectService.js';

export const useProjectVideoGroups = (projectId) => {
  const fetchVideoGroups = useCallback(async () => {
    if (!projectId) return [];
    return await ProjectService.getVideoGroups(projectId);
  }, [projectId]);

  const { data: videoGroups, loading, error, refetch } = useDataFetching(
    projectId ? fetchVideoGroups : null,
    [projectId]
  );

  return {
    videoGroups: videoGroups || [],
    loading,
    error,
    refetch
  };
};