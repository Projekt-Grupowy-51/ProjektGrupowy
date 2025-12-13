import { useState, useEffect } from 'react';
import { useParams, useLocation, useNavigate } from 'react-router-dom';
import VideoGroupService from '../services/VideoGroupService.js';

export const useVideoGroupEdit = () => {
  const { id } = useParams();
  const location = useLocation();
  const navigate = useNavigate();
  const videoGroupId = parseInt(id, 10);

  const [videoGroup, setVideoGroup] = useState(null);
  const [loading, setLoading] = useState(false);
  const [submitLoading, setSubmitLoading] = useState(false);
  const [error, setError] = useState(null);

  const fetchVideoGroup = () => {
    setLoading(true);
    setError(null);
    return VideoGroupService.getById(videoGroupId)
      .then(setVideoGroup)
      .catch(err => setError(err.message))
      .finally(() => setLoading(false));
  };

  const handleSubmit = (formData) => {
    if (!videoGroup) {
      setError('VideoGroup not loaded');
      return Promise.reject(new Error('VideoGroup not loaded'));
    }
    
    setSubmitLoading(true);
    setError(null);
    const redirectPath = location.state?.from || `/videogroups/${videoGroupId}`;
    
    return VideoGroupService.update(videoGroupId, formData)
      .then(() => navigate(redirectPath))
      .catch(err => setError(err.message))
      .finally(() => setSubmitLoading(false));
  };

  const handleCancel = () => window.history.back();

  useEffect(() => {
    fetchVideoGroup();
  }, [videoGroupId]);

  return {
    id: videoGroupId,
    videoGroup,
    loading,
    submitLoading,
    error,
    handleSubmit,
    handleCancel
  };
};