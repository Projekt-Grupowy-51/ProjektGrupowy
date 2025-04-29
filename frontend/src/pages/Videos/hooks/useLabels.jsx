import {useState, useEffect, useRef, useCallback} from "react";
import httpClient from "../../../httpclient";
import { formatTime } from "../utils";

const useLabels = (subjectId, videos, videoRefs) => {
    const [labels, setLabels] = useState([]);
    const [assignedLabels, setAssignedLabels] = useState([]);
    const [labelTimestamps, setLabelTimestamps] = useState({});
    const labelState = useRef({});

    const fetchLabels = useCallback(async () => {
        if (!subjectId) return;
        try {
            const response = await httpClient.get(`/subject/${subjectId}/label`);
            setLabels(response.data || []);
        } catch (error) {
            console.error("Error fetching labels:", error);
        }
    }, [subjectId]);

    const fetchAssignedLabels = useCallback(async () => {
        console.log('videos', videos);
        try {
            const results = await Promise.all(
                videos.map((video) =>
                    httpClient.get(`/video/${video.id}/assignedlabels`)
                )
            );
            setAssignedLabels(results.flatMap((res) => res.data));
        } catch (error) {
            console.error("Error fetching assigned labels:", error);
        }
    }, [videos]);

    const handleLabelAction = useCallback(
        async (labelId) => {
            const video = videoRefs.current[0];
            if (!video) return;

            const time = video.currentTime;
            const label = labels.find((l) => l.id === labelId);

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
        [labels, videoRefs]
    );

    const submitLabel = useCallback(
        async (labelId, start, end) => {
            try {
                await Promise.all(
                    videos.map((video) =>
                        httpClient.post("/AssignedLabel", {
                            labelId,
                            videoId: video.id,
                            start: formatTime(start),
                            end: formatTime(end),
                        })
                    )
                );
                fetchAssignedLabels();
            } catch (error) {
                console.error("Error submitting label:", error);
            }
        },
        [videos, fetchAssignedLabels]
    );

    const handleDeleteLabel = useCallback(async (labelId) => {
        try {
            await httpClient.delete(`/AssignedLabel/${labelId}`);
            setAssignedLabels((prev) => prev.filter((l) => l.id !== labelId));
        } catch (error) {
            console.error("Error deleting label:", error);
        }
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
        },
    };
};

export default useLabels;