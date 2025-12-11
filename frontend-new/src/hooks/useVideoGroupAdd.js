import { useState } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import VideoGroupService from '../services/VideoGroupService.js';

export const useVideoGroupAdd = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const projectId = searchParams.get('projectId');
  
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const handleSubmit = (videoGroupData) => {
    setLoading(true);
    setError(null);
    return VideoGroupService.create({
      ...videoGroupData,
      projectId: projectId ? parseInt(projectId) : null
    })
      .then(() => navigate(`/projects/${projectId}`))
      .catch(err => setError(err.message))
      .finally(() => setLoading(false));
  };

  const handleCancel = () => window.history.back();

  return {
    projectId,
    handleSubmit,
    handleCancel,
    loading,
    error
  };
};
