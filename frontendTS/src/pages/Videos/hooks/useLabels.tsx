import { useState, useEffect, useRef, useCallback } from "react";
import { getSubjectLabels } from "../../../services/api/subjectService";
import { getAssignedLabels } from "../../../services/api/videoService";
import { createAssignedLabel, deleteAssignedLabel } from "../../../services/api/assignedLabelService";
import { formatTime } from "../utils";

const useLabels = (subjectId, videos, videoRefs) => {
    const [labels, setLabels] = useState([]);
    const [assignedLabels, setAssignedLabels] = useState([]);
    const labelState = useRef({});
    const isFetchingLabels = useRef(false); 

    const fetchLabels = useCallback(async () => {
        if (!subjectId) return;
        try {
            const data = await getSubjectLabels(subjectId);
            setLabels(data || []);
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
                videos.map((video) => getAssignedLabels(video.id))
            );
            setAssignedLabels(results.flatMap((res) => res));
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
            if (!videos || videos.length === 0) {
                console.error("No videos available for label submission");
                return;
            }
            try {
                await Promise.all(
                    videos.map((video) =>
                        createAssignedLabel(
                            labelId,
                            video.id,
                            formatTime(start),
                            formatTime(end)
                        )
                    )
                );

                const results = await Promise.all(
                    videos.map((video) => getAssignedLabels(video.id))
                );
                const updatedAssignedLabels = results.flatMap((res) => res);

                setAssignedLabels(updatedAssignedLabels);
            } catch (error) {
                console.error("Error submitting label:", error);
            }
        },
        [videos]
    );

    const handleDeleteLabel = useCallback(async (labelId) => {
        try {
            await deleteAssignedLabel(labelId);
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
