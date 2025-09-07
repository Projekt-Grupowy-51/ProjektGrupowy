import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import VideoGroupService from '../services/VideoGroupService.js';
import VideoService from '../services/VideoService.js';

export const useVideoGroupDetails = (videoGroupId) => {
  const navigate = useNavigate();

  const [videoGroup, setVideoGroup] = useState(null);
  const [videos, setVideos] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const fetchData = () => {
    if (!videoGroupId) return Promise.resolve();
    
    const id = parseInt(videoGroupId);
    setLoading(true);
    setError(null);
    
    return Promise.all([
      VideoGroupService.getById(id),
      VideoGroupService.getVideos(id)
    ])
      .then(([videoGroupData, videosData]) => {
        setVideoGroup(videoGroupData);
        setVideos(videosData || []);
      })
      .catch(err => setError(err.message))
      .finally(() => setLoading(false));
  };

  const fetchVideos = () => {
    if (!videoGroupId) return Promise.resolve();
    
    return VideoGroupService.getVideos(parseInt(videoGroupId))
      .then(setVideos)
      .catch(err => setError(err.message));
  };

  useEffect(() => {
    fetchData();
  }, [videoGroupId]);

  const deleteVideo = async (videoId) => {
    await VideoService.delete(videoId);
    await fetchVideos();
  };

  const handleBack = () => {
    if (videoGroup?.projectId) {
      navigate(`/projects/${videoGroup.projectId}`);
    } else {
      navigate('/videogroups');
    }
  };

  const handleAddVideo = () => {
    navigate(`/videos/add?videoGroupId=${videoGroupId}`);
  };

  return {
    videoGroup,
    videos,
    loading,
    error,
    deleteVideo,
    handleBack,
    handleAddVideo
  };
};
