import { useParams } from 'react-router-dom';
import { useCallback } from 'react';
import { useDataFetching, useFormSubmission } from './common';
import ProjectService from '../services/ProjectService.js';

export const useProjectEdit = () => {
  const { id } = useParams();
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

  const redirectPath = `/projects`;

  const {
    handleSubmit,
    handleCancel,
    loading: submitLoading,
    error: submitError
  } = useFormSubmission(submitOperation, redirectPath, redirectPath);

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
