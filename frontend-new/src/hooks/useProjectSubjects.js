import { useCallback } from 'react';
import { useDataFetching } from './common';
import ProjectService from '../services/ProjectService.js';

export const useProjectSubjects = (projectId) => {
  const fetchSubjects = useCallback(async () => {
    if (!projectId) return [];
    return await ProjectService.getSubjects(projectId);
  }, [projectId]);

  const { data: subjects, loading, error, refetch } = useDataFetching(
    projectId ? fetchSubjects : null,
    [projectId]
  );

  return {
    subjects: subjects || [],
    loading,
    error,
    refetch
  };
};