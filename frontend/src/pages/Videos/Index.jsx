import React, { useEffect } from "react";
import { useParams } from "react-router-dom";
import useVideoGroup from "./hooks/useVideoGroup.jsx";
import  useVideoControls  from "./hooks/useVideoControls.jsx";
import  useLabels  from "./hooks/useLabels.jsx";
import VideoPlayers from "./components/VideoPlayers";
import ProgressBar from "./components/ProgressBar";
import BatchNavigation from "./components/BatchNavigation";
import LabelInterface from "./components/LabelInterface";
import httpClient from "../../httpclient.js";

const Videos = () => {
    const { id: assignmentId } = useParams();
    const [assignment, setAssignment] = React.useState({
        subjectId: null,
        videoGroupId: null,
    });

    const {
        videoGroup,
        batchState,
        fetchVideoGroupDetails,
    } = useVideoGroup(assignment.videoGroupId);
    const videoRefs = React.useRef([]);

    const {
        playerState,
        controls,
        handleKeyPress,
    } = useVideoControls(videoRefs, batchState);

    const {
        labels,
        assignedLabels,
        labelActions,
    } = useLabels(assignment.subjectId, videoGroup.videos, videoRefs);

    useEffect(() => {
        const fetchAssignment = async () => {
            try {
                const response = await httpClient.get(
                    `/SubjectVideoGroupAssignment/${assignmentId}`
                );
                setAssignment({
                    subjectId: response.data.subjectId,
                    videoGroupId: response.data.videoGroupId,
                });
            } catch (error) {
                console.error(error);
            }
        };

        if (assignmentId) fetchAssignment();
    }, [assignmentId]);

    useEffect(() => {
        window.addEventListener("keydown", handleKeyPress);
        return () => window.removeEventListener("keydown", handleKeyPress);
    }, [handleKeyPress, labels]);

    return (
        <div className="container content">
            <VideoPlayers
                streams={videoGroup.streams}
                videoRefs={videoRefs}
                onTimeUpdate={controls.handleTimeUpdate}
                onEnded={batchState.handleVideoEnd}
            />

            <ProgressBar
                currentTime={playerState.currentTime}
                duration={playerState.duration}
                onSeek={controls.handleSeek}
            />

            <BatchNavigation
                currentBatch={batchState.currentBatch}
                totalBatches={Object.keys(videoGroup.positions).length}
                playerState={playerState}
                controls={controls}
                batchState={batchState}
            />

            <LabelInterface
                labels={labels}
                assignedLabels={assignedLabels}
                labelActions={labelActions}
            />
        </div>
    );
};

export default Videos;