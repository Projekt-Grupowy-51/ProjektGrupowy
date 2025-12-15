import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import useVideoGroup from "./hooks/useVideoGroup";
import useVideoControls from "./hooks/useVideoControls";
import useLabels from "./hooks/useLabels";
import VideoPlayers from "./components/VideoPlayers";
import ProgressBar from "./components/ProgressBar";
import BatchNavigation from "./components/BatchNavigation";
import LabelInterface from "./components/LabelInterface";
import { getAssignment } from "../../services/api/assignmentService";

const Videos = () => {
    const { id: assignmentId } = useParams();
    const [assignment, setAssignment] = useState({
        subjectId: null,
        videoGroupId: null,
    });
    const [resetTrigger, setResetTrigger] = useState(0); 

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
                const data = await getAssignment(parseInt(assignmentId));
                setAssignment({
                    subjectId: data.subjectId,
                    videoGroupId: data.videoGroupId,
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

    const handleAllVideosEnded = () => {
        if (batchState.currentBatch < Object.keys(videoGroup.positions).length) {
            batchState.handleBatchChange(batchState.currentBatch + 1); 
            controls.handlePlayStop(); 
            controls.resetPlaybackSpeed();
            setResetTrigger((prev) => prev + 1); 
        }
    };

    const handleBatchChange = (newBatch) => {
        batchState.handleBatchChange(newBatch);
        controls.handleBatchChange(newBatch);
        setResetTrigger((prev) => prev + 1);

        setTimeout(() => {
            controls.resetPlaybackSpeed(); 
            controls.handleSeek(0); 
            controls.setPlayerState((prev) => {
                const newState = {
                    ...prev,
                    isPlaying: false, 
                    currentTime: 0, 
                    duration: 0, 
                };
                return newState;
            });
        }, 0);
    };

    return (
        <div className="container">
            <div className="content">
                <div className="row">
                    <div className="col-12">
                        <VideoPlayers
                            streams={videoGroup.streams}
                            videoRefs={videoRefs}
                            onTimeUpdate={controls.handleTimeUpdate}
                            onAllVideosEnded={handleAllVideosEnded}
                            resetTrigger={resetTrigger}
                            onLoadedMetadata={controls.handleTimeUpdate} 
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
                            handleBatchChange={handleBatchChange}
                        />
                    </div>
                </div>
                <div className="row">
                    <div className="col-12">
                        <LabelInterface
                            labels={labels}
                            assignedLabels={assignedLabels}
                            labelActions={labelActions}
                        />
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Videos;
