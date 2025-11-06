import { useState, useEffect, useCallback } from "react";
import VideoService from "../../../services/VideoService.js";
import VideoGroupService from "../../../services/VideoGroupService.js";

export const useBatchManagement = (assignment) => {
  const [currentBatch, setCurrentBatch] = useState(1);
  const [totalBatches, setTotalBatches] = useState(1);
  const [videos, setVideos] = useState([]);
  const [videoStreamUrls, setVideoStreamUrls] = useState({});
  const [selectedQualityIndex, setSelectedQualityIndex] = useState("original"); // "original", "2x", or "4x"
  const [availableQualities, setAvailableQualities] = useState([]);
  const [originalQuality, setOriginalQuality] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [initialLoadDone, setInitialLoadDone] = useState(false);

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

  // Helper function to calculate quality string from index and video metadata
  const calculateQualityFromIndex = useCallback(
    (qualityIndex, videoOriginal, videoQualities) => {
      if (qualityIndex === "original") {
        return null; // null means use original quality
      }

      const [origHeight] = videoOriginal.split("x").map(Number);

      if (qualityIndex === "2x") {
        // Find 2x lower quality
        const targetHeight = Math.floor(origHeight / 2);
        return videoQualities.find((q) => {
          const [height] = q.split("x").map(Number);
          return height === targetHeight;
        });
      }

      if (qualityIndex === "4x") {
        // Find 4x lower quality
        const targetHeight = Math.floor(origHeight / 4);
        return videoQualities.find((q) => {
          const [height] = q.split("x").map(Number);
          return height === targetHeight;
        });
      }

      return null; // Fallback to original
    },
    []
  );

  const loadBatch = useCallback(
    async (batchNumber, qualityIndex = undefined) => {
      if (!assignment?.videoGroupId) return;

      setLoading(true);
      setError(null);

      try {
        const batchVideos = await VideoService.getBatch(
          assignment.videoGroupId,
          batchNumber
        );

        // Extract quality information from the first video (all videos at same position have same qualities)
        if (batchVideos.length > 0) {
          const firstVideo = batchVideos[0];
          if (firstVideo.availableQualities && firstVideo.originalQuality) {
            setAvailableQualities(firstVideo.availableQualities);
            setOriginalQuality(firstVideo.originalQuality);
          }
        }

        // Use provided quality index, or fall back to selectedQualityIndex
        const qualityIndexToUse =
          qualityIndex !== undefined ? qualityIndex : selectedQualityIndex;

        const newVideoStreamUrls = {};
        for (const video of batchVideos) {
          try {
            // Calculate the appropriate quality string for this specific video based on the quality index
            const qualityString = calculateQualityFromIndex(
              qualityIndexToUse,
              video.originalQuality,
              video.availableQualities
            );

            // Get pre-signed URL for authenticated access
            // Pass null for original quality (no parameter), or specific quality string
            const preSignedUrl = await VideoService.getStreamBlob(
              video.id,
              qualityString
            );

            if (preSignedUrl) {
              newVideoStreamUrls[video.id] = preSignedUrl;
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
        setInitialLoadDone(true);
      } catch (error) {
        setError(error.message || "Failed to load batch");
      } finally {
        setLoading(false);
      }
    },
    [assignment?.videoGroupId, selectedQualityIndex, calculateQualityFromIndex]
  );

  useEffect(() => {
    loadVideoGroupInfo();
  }, [loadVideoGroupInfo]);

  // Initial load only - load batch 1 when assignment changes
  useEffect(() => {
    if (assignment?.videoGroupId && !initialLoadDone) {
      loadBatch(1);
    }
  }, [assignment?.videoGroupId, initialLoadDone, loadBatch]);

  const handleQualityChange = useCallback(
    (qualityIndex) => {
      // No need to cleanup pre-signed URLs, they're not blob URLs
      setSelectedQualityIndex(qualityIndex);
      // Reload current batch with new quality index
      loadBatch(currentBatch, qualityIndex);
    },
    [currentBatch, loadBatch]
  );

  // No cleanup needed for pre-signed URLs (not blob URLs)

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
    selectedQualityIndex,
    availableQualities,
    originalQuality,
    loading,
    error,
    loadBatch,
    getBatchInfo,
    loadVideoGroupInfo,
    handleQualityChange,
  };
};
