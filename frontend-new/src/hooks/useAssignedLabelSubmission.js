import { useCallback } from 'react';
import { useAsyncOperation } from './common';
import AssignedLabelService from '../services/AssignedLabelService.js';

export const useAssignedLabelSubmission = () => {
  const { execute, loading, error } = useAsyncOperation();

  const saveLabel = useCallback(async (labelData) => {
    return await execute(() => AssignedLabelService.create({
      LabelId: labelData.labelId,
      VideoId: labelData.videoId,
      Start: labelData.startTime.toString(),
      End: labelData.endTime.toString()
    }));
  }, [execute]);

  const deleteLabel = useCallback(async (assignedLabelId) => {
    return await execute(() => AssignedLabelService.delete(assignedLabelId));
  }, [execute]);

  return {
    saveLabel,
    deleteLabel,
    loading,
    error
  };
};