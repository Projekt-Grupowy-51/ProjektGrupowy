import { useState, useEffect } from 'react';
import ProjectService from '../services/ProjectService.js';
import VideoGroupService from '../services/VideoGroupService.js';

export const useProjectVideoGroups = (projectId) => {
  const [videoGroups, setVideoGroups] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const fetchVideoGroups = () => {
    if (!projectId) return Promise.resolve();
    
    setLoading(true);
    setError(null);
    return ProjectService.getVideoGroups(projectId)
      .then(setVideoGroups)
      .catch(err => setError(err.message))
      .finally(() => setLoading(false));
  };

  const deleteVideoGroup = async (videoGroupId) => {
    return VideoGroupService.delete(videoGroupId)
      .then(() => fetchVideoGroups());
  };

  useEffect(() => {
    fetchVideoGroups();
  }, [projectId]);

  return {
    videoGroups,
    loading,
    error,
    refetch: fetchVideoGroups,
    deleteVideoGroup
  };
};