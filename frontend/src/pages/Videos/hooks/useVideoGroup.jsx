import { useState, useEffect, useCallback } from "react";
import httpClient, { API_BASE_URL } from "../../../httpclient";

const useVideoGroup = (videoGroupId) => {
  const [videoGroup, setVideoGroup] = useState({
    positions: {},
    videos: [],
    streams: [],
  });
  const [batchState, setBatchState] = useState({
    currentBatch: 1,
    endedVideos: new Set(),
  });

  const fetchVideoGroupDetails = useCallback(
    async (batch = 1) => {
      if (!videoGroupId) return;

      try {
        const [groupRes, videosRes] = await Promise.all([
          httpClient.get(`/VideoGroup/${videoGroupId}`),
          httpClient.get(`/Video/batch/${videoGroupId}/${batch}`),
        ]);

        // Generate streaming URLs directly
        const streams = videosRes.data
          .slice(0, 4)
          .map((video) => `${API_BASE_URL}/Video/${video.id}/stream`);

        setVideoGroup({
          positions: groupRes.data.videosAtPositions,
          videos: videosRes.data,
          streams,
        });
        setBatchState((prev) => ({ ...prev, currentBatch: batch }));
      } catch (error) {
        console.error("Error fetching video group:", error);
      }
    },
    [videoGroupId]
  );

  const handleBatchChange = useCallback(
    async (newBatch) => {
      if (newBatch < 1 || newBatch > Object.keys(videoGroup.positions).length)
        return;
      await fetchVideoGroupDetails(newBatch);
    },
    [videoGroup.positions, fetchVideoGroupDetails]
  );

  const handleVideoEnd = useCallback(
    (index) => {
      const newEndedVideos = new Set(batchState.endedVideos);
      newEndedVideos.add(index);

      if (newEndedVideos.size === videoGroup.streams.length) {
        if (
          batchState.currentBatch < Object.keys(videoGroup.positions).length
        ) {
          handleBatchChange(batchState.currentBatch + 1);
        }
        newEndedVideos.clear();
      }

      setBatchState((prev) => ({
        ...prev,
        endedVideos: newEndedVideos,
      }));
    },
    [batchState, videoGroup, handleBatchChange]
  );

  useEffect(() => {
    fetchVideoGroupDetails();
  }, [fetchVideoGroupDetails]);

  return {
    videoGroup,
    batchState: {
      ...batchState,
      handleBatchChange,
      handleVideoEnd,
    },
    fetchVideoGroupDetails,
  };
};

export default useVideoGroup;
