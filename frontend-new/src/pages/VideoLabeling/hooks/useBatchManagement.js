import { useState, useEffect, useCallback } from 'react';
import VideoService from '../../../services/VideoService.js';
import VideoGroupService from '../../../services/VideoGroupService.js';

export const useBatchManagement = (assignment) => {
  const [currentBatch, setCurrentBatch] = useState(1);
  const [totalBatches, setTotalBatches] = useState(1);
  const [videos, setVideos] = useState([]);
  const [videoStreamUrls, setVideoStreamUrls] = useState({});
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  // Load video group info to determine total batches
  const loadVideoGroupInfo = useCallback(async () => {
    if (!assignment?.videoGroupId) return;

    try {
      const videoGroup = await VideoGroupService.getById(assignment.videoGroupId);
      if (videoGroup?.videosAtPositions) {
        const batchCount = Object.keys(videoGroup.videosAtPositions).length;
        setTotalBatches(batchCount);
      }
    } catch (error) {
      setTotalBatches(1); // Fallback to 1 batch
    }
  }, [assignment?.videoGroupId]);

  const loadBatch = useCallback(async (batchNumber) => {
    setLoading(true);
    setError(null);
    
    try {
      const batchVideos = await VideoService.getBatch(assignment.videoGroupId, batchNumber);
      
      const newVideoStreamUrls = {};
      for (const video of batchVideos) {
        try {
          const streamUrl = await VideoService.getStreamBlob(video.id);
          if (streamUrl) {
            newVideoStreamUrls[video.id] = streamUrl;
          }
        } catch (streamError) {

        }
      }
      
      setCurrentBatch(batchNumber);
      setVideos(batchVideos);
      setVideoStreamUrls(current => ({ ...current, ...newVideoStreamUrls }));
    } catch (error) {
      setError(error.message || 'Failed to load batch');
    } finally {
      setLoading(false);
    }
  }, [assignment?.videoGroupId]);

  useEffect(() => {
    loadVideoGroupInfo();
  }, [loadVideoGroupInfo]);

  useEffect(() => {
    loadBatch(1);
  }, [loadBatch]);

  useEffect(() => {
    return () => {
      Object.values(videoStreamUrls).forEach(URL.revokeObjectURL);
    };
  }, []);

  const getBatchInfo = () => ({
    current: currentBatch,
    total: totalBatches,
    canGoPrevious: currentBatch > 1,
    canGoNext: currentBatch < totalBatches
  });

  return {
    currentBatch,
    totalBatches,
    videos,
    videoStreamUrls,
    loading,
    error,
    loadBatch,
    getBatchInfo,
    loadVideoGroupInfo
  };
};