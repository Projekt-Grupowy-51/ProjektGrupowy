import { useState } from 'react';
import AssignedLabelService from '../services/AssignedLabelService.js';

export const useAssignedLabelSubmission = () => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const saveLabel = (labelData) => {
    setLoading(true);
    setError(null);
    return AssignedLabelService.create({
      LabelId: labelData.labelId,
      VideoId: labelData.videoId,
      Start: labelData.startTime.toString(),
      End: labelData.endTime.toString()
    })
      .catch(err => {
        setError(err.message);
        throw err;
      })
      .finally(() => setLoading(false));
  };

  const deleteLabel = (assignedLabelId) => {
    setLoading(true);
    setError(null);
    return AssignedLabelService.delete(assignedLabelId)
      .catch(err => {
        setError(err.message);
        throw err;
      })
      .finally(() => setLoading(false));
  };

  return {
    saveLabel,
    deleteLabel,
    loading,
    error
  };
};