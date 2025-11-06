import { useState, useEffect, useCallback } from "react";
import VideoService from "../../../services/VideoService.js";
import VideoGroupService from "../../../services/VideoGroupService.js";

export const useBatchManagement = (assignment) => {
  const [currentBatch, setCurrentBatch] = useState(1);
  const [totalBatches, setTotalBatches] = useState(1);
  const [videos, setVideos] = useState([]);
  const [videoStreamUrls, setVideoStreamUrls] = useState({});
  const [selectedQuality, setSelectedQuality] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  // Load video group info to determine total batches
  const loadVideoGroupInfo = useCallback(async () => {
    if (!assignment?.videoGroupId) return;

    try {
      const videoGroup = await VideoGroupService.getById(
        assignment.videoGroupId
      );
      if (videoGroup?.videosAtPositions) {
        const batchCount = Object.keys(videoGroup.videosAtPositions).length;
        setTotalBatches(batchCount);
      }
    } catch (error) {
      setTotalBatches(1); // Fallback to 1 batch
    }
  }, [assignment?.videoGroupId]);

  const loadBatch = useCallback(
    async (batchNumber, quality = undefined) => {
      setLoading(true);
      setError(null);

      try {
        const batchVideos = await VideoService.getBatch(
          assignment.videoGroupId,
          batchNumber
        );

        // Only set default quality on first load if not explicitly provided
        if (
          selectedQuality === null &&
          quality === undefined &&
          batchVideos.length > 0
        ) {
          // Default to null (original quality without parameter)
          setSelectedQuality(null);
        }

        // Use provided quality, or fall back to selectedQuality
        // If both are undefined/null, pass null to get original
        const qualityToUse = quality !== undefined ? quality : selectedQuality;

        const newVideoStreamUrls = {};
        for (const video of batchVideos) {
          try {
            // Use getStreamBlob with quality parameter for authenticated access
            // Pass null for original quality (no parameter), or specific quality string
            const streamUrl = await VideoService.getStreamBlob(
              video.id,
              qualityToUse
            );

            if (streamUrl) {
              newVideoStreamUrls[video.id] = streamUrl;
            }
          } catch (streamError) {
            console.error(
              `Failed to load stream for video ${video.id}:`,
              streamError
            );
          }
        }

        setCurrentBatch(batchNumber);
        setVideos(batchVideos);
        setVideoStreamUrls(newVideoStreamUrls);
      } catch (error) {
        setError(error.message || "Failed to load batch");
      } finally {
        setLoading(false);
      }
    },
    [assignment?.videoGroupId, selectedQuality]
  );

  useEffect(() => {
    loadVideoGroupInfo();
  }, [loadVideoGroupInfo]);

  useEffect(() => {
    loadBatch(1);
  }, [loadBatch]);

  const handleQualityChange = useCallback(
    (quality) => {
      // Cleanup old blob URLs before loading new ones
      Object.values(videoStreamUrls).forEach((url) => {
        if (url.startsWith("blob:")) {
          URL.revokeObjectURL(url);
        }
      });

      setSelectedQuality(quality);
      // Reload current batch with new quality
      loadBatch(currentBatch, quality);
    },
    [currentBatch, loadBatch, videoStreamUrls]
  );

  // Cleanup on unmount
  useEffect(() => {
    return () => {
      Object.values(videoStreamUrls).forEach((url) => {
        if (url.startsWith("blob:")) {
          URL.revokeObjectURL(url);
        }
      });
    };
  }, [videoStreamUrls]);

  const getBatchInfo = () => ({
    current: currentBatch,
    total: totalBatches,
    canGoPrevious: currentBatch > 1,
    canGoNext: currentBatch < totalBatches,
  });

  return {
    currentBatch,
    totalBatches,
    videos,
    videoStreamUrls,
    selectedQuality,
    loading,
    error,
    loadBatch,
    getBatchInfo,
    loadVideoGroupInfo,
    handleQualityChange,
  };
};
