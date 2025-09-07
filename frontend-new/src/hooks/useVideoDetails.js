import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import VideoService from '../services/VideoService.js';

export const useVideoDetails = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  
  const [video, setVideo] = useState(null);
  const [assignedLabels, setAssignedLabels] = useState([]);
  const [videoLoading, setVideoLoading] = useState(false);
  const [labelsLoading, setLabelsLoading] = useState(false);
  const [videoError, setVideoError] = useState(null);
  const [labelsError, setLabelsError] = useState(null);
  const [videoStreamUrl, setVideoStreamUrl] = useState(null);
  const [videoStreamLoading, setVideoStreamLoading] = useState(false);

  const fetchData = () => {
    const videoId = parseInt(id);
    
    setVideoLoading(true);
    setLabelsLoading(true);
    setVideoError(null);
    setLabelsError(null);
    
    Promise.all([
      VideoService.getById(videoId),
      VideoService.getAssignedLabels(videoId)
    ])
      .then(([videoData, labelsData]) => {
        setVideo(videoData);
        setAssignedLabels(labelsData || []);
      })
      .catch(err => {
        setVideoError(err.message);
        setLabelsError(err.message);
      })
      .finally(() => {
        setVideoLoading(false);
        setLabelsLoading(false);
      });
  };

  useEffect(() => {
    fetchData();
  }, [id]);

  // Load video stream URL when video data is available
  useEffect(() => {
    const loadVideoStream = async () => {
      if (video?.id && !videoStreamUrl) {
        setVideoStreamLoading(true);
        try {
          const streamUrl = await VideoService.getStreamBlob(video.id);
          setVideoStreamUrl(streamUrl);
        } catch (error) {
          console.error('Failed to load video stream:', error);
        } finally {
          setVideoStreamLoading(false);
        }
      }
    };

    loadVideoStream();
  }, [video?.id, videoStreamUrl]);

  // Cleanup blob URL on unmount
  useEffect(() => {
    return () => {
      if (videoStreamUrl) {
        URL.revokeObjectURL(videoStreamUrl);
      }
    };
  }, [videoStreamUrl]);

  const handleBackToVideoGroup = () => {
    if (video?.videoGroupId) {
      navigate(`/videogroups/${video.videoGroupId}`);
    }
  };

  const formatDuration = (seconds) => {
    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = seconds % 60;
    return `${minutes}:${remainingSeconds.toString().padStart(2, '0')}`;
  };

  return {
    video,
    videoLoading,
    videoError,
    assignedLabels,
    labelsLoading,
    labelsError,
    videoStreamUrl,
    videoStreamLoading,
    handleBackToVideoGroup,
    formatDuration
  };
};
