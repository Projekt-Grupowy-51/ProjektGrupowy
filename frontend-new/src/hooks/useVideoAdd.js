import { useState, useEffect, useCallback } from 'react';
import { useLocation } from 'react-router-dom';
import { useFormNavigation, useAsyncOperation, useDataFetching } from './common';
import VideoGroupService from '../services/VideoGroupService.js';
import VideoService from '../services/VideoService.js';

export const useVideoAdd = () => {
  const location = useLocation();
  const { handleSuccess, handleCancel } = useFormNavigation();

  const [videoGroupId, setVideoGroupId] = useState(null);
  const [videoGroupName, setVideoGroupName] = useState('');
  const [uploadProgress, setUploadProgress] = useState(0);

  useEffect(() => {
    const queryParams = new URLSearchParams(location.search);
    const groupId = queryParams.get('videoGroupId');
    if (!groupId) return;

    setVideoGroupId(parseInt(groupId, 10));
  }, [location.search]);

  const fetchVideoGroup = useCallback(async () => {
    if (!videoGroupId) return null;
    const videoGroup = await VideoGroupService.getById(videoGroupId);
    setVideoGroupName(videoGroup.name);
    return videoGroup;
  }, [videoGroupId]);

  const { refetch: refetchVideoGroup } = useDataFetching(fetchVideoGroup, [videoGroupId]);

  const { execute: executeUpload, loading, error } = useAsyncOperation();

  const uploadVideosCallback = useCallback(
    async (videos) => {
      if (!videoGroupId) throw new Error('Video group ID not set');

      await executeUpload(async () => {
        const total = videos.length;

        for (let i = 0; i < total; i++) {
          const video = videos[i];
          setUploadProgress(Math.floor(((i + 0.5) / total) * 100));

          await VideoService.create({
            name: video.title,
            title: video.title,
            description: `Video: ${video.title}`,
            url: video.url || `https://example.com/video${Date.now()}.mp4`,
            duration: video.duration || Math.floor(Math.random() * 3600),
            videoGroupId,
            positionInQueue: video.positionInQueue
          });

          setUploadProgress(Math.floor(((i + 1) / total) * 100));
        }
      });
    },
    [videoGroupId, executeUpload]
  );

  const handleSubmitCallback = useCallback(
    async (videos) => {
      try {
        await uploadVideosCallback(videos);
        handleSuccess(`/videogroups/${videoGroupId}`);
      } catch (err) {
        console.error('Upload failed:', err);
      }
    },
    [uploadVideosCallback, handleSuccess, videoGroupId]
  );

  const handleCancelCallback = useCallback(() => {
    handleCancel(`/videogroups/${videoGroupId}`);
  }, [handleCancel, videoGroupId]);

  return {
    videoGroupId,
    videoGroupName,
    loading,
    uploadProgress,
    error,
    handleSubmit: handleSubmitCallback,
    handleCancel: handleCancelCallback,
    refetchVideoGroup
  };
};
