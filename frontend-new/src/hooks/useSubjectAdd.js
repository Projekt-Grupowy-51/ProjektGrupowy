import { useLocation } from 'react-router-dom';
import { useMemo, useCallback } from 'react';
import { useFormSubmission } from './common';
import SubjectService from '../services/SubjectService.js';

export const useSubjectAdd = () => {
  const location = useLocation();

  const projectId = useMemo(() => {
    const id = new URLSearchParams(location.search).get('projectId');
    return id ? parseInt(id, 10) : null;
  }, [location.search]);

  const submitOperation = useCallback(
    async (subjectData) => {
      if (!projectId) throw new Error('Project ID not specified');
      const subjectRequest = {
        ...subjectData,
        projectId
      };
      return await SubjectService.create(subjectRequest);
    },
    [projectId]
  );

  const successPath = `/projects/${projectId}`;
  const cancelPath = `/projects/${projectId}`;

  const { handleSubmit, handleCancel, loading, error } = useFormSubmission(
    submitOperation,
    successPath,
    cancelPath
  );

  return {
    projectId,
    handleSubmit,
    handleCancel,
    loading,
    error
  };
};
