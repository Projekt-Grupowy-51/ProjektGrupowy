import { useState, useEffect, useCallback } from "react";
import httpClient from "../../../httpClient";

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

    const fetchVideoStreams = useCallback(async (videos) => {
        const videoIds = videos.slice(0, 4).map((video) => video.id);
        const streams = await Promise.all(
            videoIds.map(async (videoId) => {
                try {
                    const response = await httpClient.get(`/Video/${videoId}/stream`, {
                        responseType: "blob",
                    });
                    return URL.createObjectURL(response.data);
                } catch (error) {
                    return null;
                }
            })
        );
        return streams.filter((url) => url !== null);
    }, []);

    const fetchVideoGroupDetails = useCallback(
        async (batch = 1) => {
            if (!videoGroupId) return;

            try {
                const [groupRes, videosRes] = await Promise.all([
                    httpClient.get(`/VideoGroup/${videoGroupId}`),
                    httpClient.get(`/Video/batch/${videoGroupId}/${batch}`),
                ]);

                const streams = await fetchVideoStreams(videosRes.data);

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
        [videoGroupId, fetchVideoStreams]
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
                if (batchState.currentBatch < Object.keys(videoGroup.positions).length) {
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