import { useParams, useLocation } from 'react-router-dom';
import { useCallback } from 'react';
import { useDataFetching, useFormSubmission } from './common';
import ProjectService from '../services/ProjectService.js';

export const useProjectEdit = () => {
  const { id } = useParams();
  const location = useLocation();
  const projectId = parseInt(id, 10);

  const fetchProject = useCallback(
    () => ProjectService.getById(projectId),
    [projectId]
  );

  const { data: project, loading: projectLoading, error: projectError } =
    useDataFetching(fetchProject, [projectId]);

  const submitOperation = useCallback(
    async (formData) => {
      return await ProjectService.update(projectId, formData);
    },
    [projectId]
  );

  // Check if user came from project details page
  const redirectPath = location.state?.from || (projectId ? `/projects/${projectId}` : `/projects`);

  const {
    handleSubmit,
    handleCancel: defaultHandleCancel,
    loading: submitLoading,
    error: submitError
  } = useFormSubmission(submitOperation, redirectPath, redirectPath);

  // Override cancel to use browser back
  const handleCancel = useCallback(() => {
    window.history.back();
  }, []);

  return {
    id: projectId,
    project,
    loading: projectLoading,
    submitLoading,
    error: projectError || submitError,
    handleSubmit,
    handleCancel
  };
};
