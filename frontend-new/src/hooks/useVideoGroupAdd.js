import { useSearchParams } from 'react-router-dom';
import { useCallback } from 'react';
import { useFormSubmission } from './common';
import VideoGroupService from '../services/VideoGroupService.js';

export const useVideoGroupAdd = () => {
  const [searchParams] = useSearchParams();
  const projectId = searchParams.get('projectId');

  const submitOperation = useCallback(
    (videoGroupData) => {
      return VideoGroupService.create({
        ...videoGroupData,
        projectId: projectId ? parseInt(projectId) : null
      });
    },
    [projectId]
  );

  const successPath = `/projects/${projectId}`;
  const cancelPath = `/projects/${projectId}`;

  const { handleSubmit, handleCancel: defaultHandleCancel, loading, error } = useFormSubmission(
    submitOperation,
    successPath,
    cancelPath
  );

  // Override cancel to use browser back
  const handleCancel = useCallback(() => {
    window.history.back();
  }, []);

  return {
    projectId,
    handleSubmit,
    handleCancel,
    loading,
    error
  };
};
