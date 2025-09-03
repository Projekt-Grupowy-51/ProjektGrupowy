import { useCallback } from 'react';
import { useMultipleDataFetching, useNavigation } from './common';
import VideoGroupService from '../services/VideoGroupService.js';
import VideoService from '../services/VideoService.js';

export const useVideoGroupDetails = (videoGroupId) => {
  const { goTo } = useNavigation();

  const fetchOperations = {
    videoGroup: useCallback(() => VideoGroupService.getById(parseInt(videoGroupId)), [videoGroupId]),
    videos: useCallback(() => VideoGroupService.getVideos(parseInt(videoGroupId)), [videoGroupId])
  };

  const { data, loading, error, fetchData } = useMultipleDataFetching(videoGroupId ? fetchOperations : {});

  const deleteVideo = useCallback(
    async (videoId) => {
      await VideoService.delete(videoId);
      await fetchData('videos', fetchOperations.videos);
    },
    [fetchData, fetchOperations.videos]
  );

  const handleBack = useCallback(() => {
    const videoGroup = data.videoGroup;
    if (videoGroup?.projectId) goTo(`/projects/${videoGroup.projectId}`);
    else goTo('/videogroups');
  }, [data.videoGroup, goTo]);

  const handleAddVideo = useCallback(() => {
    goTo(`/videos/add?videoGroupId=${videoGroupId}`);
  }, [goTo, videoGroupId]);

  return {
    videoGroup: data.videoGroup,
    videos: data.videos || [],
    loading: loading.videoGroup || loading.videos,
    error: error.videoGroup || error.videos,
    deleteVideo,
    handleBack,
    handleAddVideo
  };
};
