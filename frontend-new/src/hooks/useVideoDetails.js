import { useCallback, useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { useMultipleDataFetching, useNavigation } from './common';
import VideoService from '../services/VideoService.js';

export const useVideoDetails = () => {
  const { id } = useParams();
  const { goTo } = useNavigation();
  const [videoStreamUrl, setVideoStreamUrl] = useState(null);
  const [videoStreamLoading, setVideoStreamLoading] = useState(false);

  const fetchVideo = useCallback(() => VideoService.getById(parseInt(id)), [id]);
  const fetchAssignedLabels = useCallback(() => VideoService.getAssignedLabels(parseInt(id)), [id]);

  const { data, loading, error } = useMultipleDataFetching({
    video: fetchVideo,
    assignedLabels: fetchAssignedLabels
  });

  // Load video stream URL when video data is available
  useEffect(() => {
    const loadVideoStream = async () => {
      if (data.video?.id && !videoStreamUrl) {
        setVideoStreamLoading(true);
        try {
          const streamUrl = await VideoService.getStreamBlob(data.video.id);
          setVideoStreamUrl(streamUrl);
        } catch (error) {
          console.error('Failed to load video stream:', error);
        } finally {
          setVideoStreamLoading(false);
        }
      }
    };

    loadVideoStream();
  }, [data.video?.id, videoStreamUrl]);

  // Cleanup blob URL on unmount
  useEffect(() => {
    return () => {
      if (videoStreamUrl) {
        URL.revokeObjectURL(videoStreamUrl);
      }
    };
  }, [videoStreamUrl]);

  const handleBackToVideoGroup = useCallback(() => {
    const video = data.video;
    if (video?.videoGroupId) {
      goTo(`/videogroups/${video.videoGroupId}`);
    }
  }, [data.video, goTo]);

  const formatDuration = useCallback((seconds) => {
    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = seconds % 60;
    return `${minutes}:${remainingSeconds.toString().padStart(2, '0')}`;
  }, []);

  return {
    video: data.video,
    videoLoading: loading.video,
    videoError: error.video,
    assignedLabels: data.assignedLabels || [],
    labelsLoading: loading.assignedLabels,
    labelsError: error.assignedLabels,
    videoStreamUrl,
    videoStreamLoading,
    handleBackToVideoGroup,
    formatDuration
  };
};
