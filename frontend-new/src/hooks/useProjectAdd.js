import { useCallback } from 'react';
import { useFormSubmission } from './common';
import ProjectService from '../services/ProjectService.js';

export const useProjectAdd = () => {
  const submitOperation = useCallback(
    (projectData) => ProjectService.create(projectData),
    []
  );

  const { handleSubmit, handleCancel, loading, error } = useFormSubmission(
    submitOperation,
    '/projects',
    '/projects'
  );

  return {
    handleSubmit,
    handleCancel,
    loading,
    error
  };
};
