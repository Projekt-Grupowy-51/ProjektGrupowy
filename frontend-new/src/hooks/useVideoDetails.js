import { useCallback } from 'react';
import { useParams } from 'react-router-dom';
import { useMultipleDataFetching, useNavigation } from './common';
import VideoService from '../services/VideoService.js';

export const useVideoDetails = () => {
  const { id } = useParams();
  const { goTo } = useNavigation();

  const fetchVideo = useCallback(() => VideoService.getById(parseInt(id)), [id]);
  const fetchAssignedLabels = useCallback(() => VideoService.getAssignedLabels(parseInt(id)), [id]);

  const { data, loading, error } = useMultipleDataFetching({
    video: fetchVideo,
    assignedLabels: fetchAssignedLabels
  });

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
    handleBackToVideoGroup,
    formatDuration
  };
};
