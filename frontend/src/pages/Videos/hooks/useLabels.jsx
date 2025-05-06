import { useState, useEffect, useRef, useCallback } from "react";
import httpClient from "../../../httpclient";
import { formatTime } from "../utils";

const useLabels = (subjectId, videos, videoRefs) => {
    const [labels, setLabels] = useState([]);
    const [assignedLabels, setAssignedLabels] = useState([]);
    const labelState = useRef({});
    const isFetchingLabels = useRef(false); 

    const fetchLabels = useCallback(async () => {
        if (!subjectId) return;
        try {
            const response = await httpClient.get(`/subject/${subjectId}/label`, {skipLoadingScreen: true});
            setLabels(response.data || []);
        } catch (error) {
            console.error("Error fetching labels:", error);
        }
    }, [subjectId]);

    const fetchAssignedLabels = useCallback(async () => {
        if (!videos || videos.length === 0 || isFetchingLabels.current) {
            console.log("Skipping fetchAssignedLabels due to empty videos array or ongoing fetch");
            return;
        }
        isFetchingLabels.current = true; 
        try {
            const results = await Promise.all(
                videos.map((video) =>
                    httpClient.get(`/video/${video.id}/${subjectId}/assignedlabels`, { skipLoadingScreen: true })
                )
            );
            setAssignedLabels(results.flatMap((res) => res.data));
        } catch (error) {
            console.error("Error fetching assigned labels:", error);
        } finally {
            isFetchingLabels.current = false; 
        }
    }, [videos]);

    const handleLabelAction = useCallback(
        async (labelId) => {
            const video = videoRefs.current[0];
            if (!video) return;

            const time = video.currentTime;
            const label = labels.find((l) => l.id === labelId);

            if (!videos || videos.length === 0) {
                console.error("No videos available for label assignment");
                return;
            }

            if (label?.type === "point") {
                await submitLabel(labelId, time, time);
                return;
            }

            if (labelState.current[labelId]?.start) {
                await submitLabel(labelId, labelState.current[labelId].start, time);
                labelState.current[labelId] = {};
            } else {
                labelState.current[labelId] = { start: time };
            }
        },
        [labels, videos, videoRefs]
    );

    const submitLabel = useCallback(
        async (labelId, start, end) => {
            console.log("Submitting label:", { labelId, start, end, videos });
            if (!videos || videos.length === 0) {
                console.error("No videos available for label submission");
                return;
            }
            try {
                await Promise.all(
                    videos.map((video) =>
                        httpClient.post("/AssignedLabel", {
                            labelId,
                            videoId: video.id,
                            start: formatTime(start),
                            end: formatTime(end),
                        }, { skipLoadingScreen: true })
                    )
                );

                const fetchPromises = videos.map((video) =>
                    httpClient.get(`/video/${video.id}/${subjectId}/assignedlabels`, {
                        withCredentials: true,
                        skipLoadingScreen: true,
                    })
                );

                const results = await Promise.all(fetchPromises);
                const updatedAssignedLabels = results.flatMap((res) => res.data);

                setAssignedLabels(updatedAssignedLabels);
            } catch (error) {
                console.error("Error submitting label:", error);
            }
        },
        [videos]
    );

    const handleDeleteLabel = useCallback(async (labelId) => {
        try {
            await httpClient.delete(`/AssignedLabel/${labelId}`);
            setAssignedLabels((prev) => prev.filter((l) => l.id !== labelId));
        } catch (error) {
            console.error("Error deleting label:", error);
        }
    }, []);

    const getLabelState = useCallback((labelId) => {
        return labelState.current[labelId] || {};
    }, []);

    useEffect(() => {
        fetchLabels();
    }, [fetchLabels]);

    useEffect(() => {
        fetchAssignedLabels();
    }, [fetchAssignedLabels]);

    return {
        labels,
        assignedLabels,
        labelActions: {
            handleLabelClick: handleLabelAction,
            handleDelete: handleDeleteLabel,
            getLabelState, 
        },
    };
};

export default useLabels;