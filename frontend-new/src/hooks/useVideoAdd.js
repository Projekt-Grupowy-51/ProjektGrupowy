import { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import VideoGroupService from '../services/VideoGroupService.js';
import VideoService from '../services/VideoService.js';

export const useVideoAdd = () => {
  const location = useLocation();
  const navigate = useNavigate();

  const [videoGroupId, setVideoGroupId] = useState(null);
  const [videoGroupName, setVideoGroupName] = useState('');
  const [uploadProgress, setUploadProgress] = useState(0);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    const queryParams = new URLSearchParams(location.search);
    const groupId = queryParams.get('videoGroupId');
    if (!groupId) return;

    setVideoGroupId(parseInt(groupId, 10));
  }, [location.search]);

  const fetchVideoGroup = () => {
    if (!videoGroupId) return Promise.resolve();
    
    return VideoGroupService.getById(videoGroupId)
      .then(videoGroup => setVideoGroupName(videoGroup.name))
      .catch(err => setError(err.message));
  };

  useEffect(() => {
    if (videoGroupId) {
      fetchVideoGroup();
    }
  }, [videoGroupId]);

  const handleSubmit = async (videos) => {
    if (!videoGroupId) {
      setError('Video group ID not set');
      return;
    }

    setLoading(true);
    setError(null);
    setUploadProgress(0);
    
    try {
      const total = videos.length;

      for (let i = 0; i < total; i++) {
        const video = videos[i];
        setUploadProgress(Math.floor(((i + 0.5) / total) * 100));

        await VideoService.create({
          title: video.title,
          file: video.file,
          videoGroupId,
          positionInQueue: parseInt(video.positionInQueue, 10)
        });

        setUploadProgress(Math.floor(((i + 1) / total) * 100));
      }
      
      navigate(`/videogroups/${videoGroupId}`);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleCancel = () => navigate(`/videogroups/${videoGroupId}`);

  return {
    videoGroupId,
    videoGroupName,
    loading,
    uploadProgress,
    error,
    handleSubmit,
    handleCancel,
    refetchVideoGroup: fetchVideoGroup
  };
};
