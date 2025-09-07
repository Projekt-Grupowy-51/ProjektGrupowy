import { useState, useEffect, useCallback } from 'react';
import VideoService from '../../../services/VideoService.js';
import AssignedLabelService from '../../../services/AssignedLabelService.js';

export const useAssignedLabelsState = (videos, assignment) => {
  const [assignedLabels, setAssignedLabels] = useState([]);
  const [loading, setLoading] = useState(false);
  const [saveLoading, setSaveLoading] = useState(false);
  const [deleteLoading, setDeleteLoading] = useState(false);

  const loadAssignedLabels = useCallback(async () => {
    setLoading(true);
    
    const promises = videos.map(video => 
      VideoService.getAssignedLabelsBySubject(video.id, assignment.subjectId)
    );
    const arrays = await Promise.all(promises);
    const flatLabels = arrays.flat();
    
    setAssignedLabels(flatLabels);
    setLoading(false);
  }, [videos, assignment?.subjectId]);

  // Load labels when videos or subjectId change
  useEffect(() => {
    loadAssignedLabels();
  }, [loadAssignedLabels]);

  const handleSaveLabel = useCallback(async (labelData) => {
    setSaveLoading(true);
    try {
      // Save label for ALL videos, not just first one
      const savePromises = videos.map(video => {
        const requestData = {
          ...labelData,
          videoId: video.id
        };
        return AssignedLabelService.create(requestData);
      });
      
      await Promise.all(savePromises);
      
      // Reload assigned labels after saving
      setLoading(true);
      const promises = videos.map(video => 
        VideoService.getAssignedLabelsBySubject(video.id, assignment.subjectId)
      );
      const arrays = await Promise.all(promises);
      setAssignedLabels(arrays.flat());
      setLoading(false);
    } finally {
      setSaveLoading(false);
    }
  }, [videos, assignment?.subjectId]);

  const handleDeleteLabel = useCallback(async (assignedLabelId) => {
    setDeleteLoading(true);
    try {
      await AssignedLabelService.delete(assignedLabelId);
      
      // Reload assigned labels after deletion
      setLoading(true);
      const promises = videos.map(video => 
        VideoService.getAssignedLabelsBySubject(video.id, assignment.subjectId)
      );
      const arrays = await Promise.all(promises);
      setAssignedLabels(arrays.flat());
      setLoading(false);
    } finally {
      setDeleteLoading(false);
    }
  }, [videos, assignment?.subjectId]);

  return {
    assignedLabels,
    assignedLabelsLoading: loading,
    labelOperationLoading: saveLoading || deleteLoading,
    loadAssignedLabels,
    handleSaveLabel,
    handleDeleteLabel
  };
};