import { useParams, useLocation } from 'react-router-dom';
import { useCallback } from 'react';
import { useDataFetching, useFormSubmission } from './common';
import VideoGroupService from '../services/VideoGroupService.js';

export const useVideoGroupEdit = () => {
  const { id } = useParams();
  const location = useLocation();
  const videoGroupId = parseInt(id, 10);

  const fetchVideoGroup = useCallback(
    () => VideoGroupService.getById(videoGroupId),
    [videoGroupId]
  );

  const { data: videoGroup, loading: videoGroupLoading, error: videoGroupError } =
    useDataFetching(fetchVideoGroup, [videoGroupId]);

  const submitOperation = useCallback(async (formData) => {
    if (!videoGroup) throw new Error('VideoGroup not loaded');
    return await VideoGroupService.update(videoGroupId, formData);
  }, [videoGroup, videoGroupId]);

  const redirectPath = location.state?.from || `/videogroups/${videoGroupId}`;

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
    id: videoGroupId,
    videoGroup,
    loading: videoGroupLoading,
    submitLoading,
    error: videoGroupError || submitError,
    handleSubmit,
    handleCancel
  };
};