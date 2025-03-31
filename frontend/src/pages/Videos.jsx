import React, { useState, useEffect, useRef } from "react";
import { useParams } from "react-router-dom";
import httpClient from "../httpClient";
import "./css/ScientistProjects.css";

const Videos = () => {
    const { id } = useParams();
    const [videos, setVideos] = useState([]);
    const [streams, setStreams] = useState([]);
    const [isPlaying, setIsPlaying] = useState(false);
    const [labels, setLabels] = useState([]);
    const [assignedLabels, setAssignedLabels] = useState([]);
    const [subjectId, setSubjectId] = useState(null);
    const [labelTimestamps, setLabelTimestamps] = useState({});
    const [currentTime, setCurrentTime] = useState(0);
    const [duration, setDuration] = useState(0);
    const [currentBatch, setCurrentBatch] = useState(1);
    const [videoPositions, setVideoPositions] = useState({});
    const videoRefs = useRef([]);

    useEffect(() => {
        if (id) {
            fetchVideoGroupDetails(id);
            fetchSubjectId();
        }
    }, [id]);

    useEffect(() => {
        if (subjectId !== null) {
            fetchLabels();
            fetchAssignedLabels();
        }
    }, [subjectId]);

    const fetchVideoGroupDetails = async (videoId) => {
        try {
            const response = await httpClient.get(`/VideoGroup/${videoId}`, {
                withCredentials: true,
            });
            console.log(response.data);
            setVideos(response.data.videos || []);
            setVideoPositions(response.data.videosAtPositions);

            console.log(Object.keys(videoPositions).length);

            fetchVideos(currentBatch);
        } catch (error) {
            console.error("Failed to load video group details");
        }
    };

    const onBatchChangedAsync = async (newBatch) => {
        setCurrentBatch(newBatch); 
        console.log("Current batch set to: ", newBatch);
        await fetchVideos(newBatch); 
    };

    const fetchVideos = async (batch) => {
        try {
            const batchToFetch = batch || currentBatch; 
            console.log("Fetching Current batch: ", batchToFetch);
            const response = await httpClient.get(
                `/Video/batch/${id}/${batchToFetch}`,
                {
                    withCredentials: true,
                }
            );
            setVideos(response.data);
            fetchVideoStreams(response.data);
        } catch (error) {
            console.error("Failed to load video list");
        }
    };

    async function fetchVideoStreams(videos) {
        const videoIds = videos.slice(0, 4).map((video) => video.id);
        const streamsPromises = videoIds.map(async (videoId) => {
            try {
                const response = await httpClient.get(`/Video/${videoId}/stream`, {
                    withCredentials: true,
                    responseType: "blob",
                });
                return URL.createObjectURL(response.data);
            } catch (error) {
                return null;
            }
        });

        try {
            const streamUrls = await Promise.all(streamsPromises);
            setStreams(streamUrls.filter((url) => url !== null));
        } catch (error) {
            console.error("Error fetching video streams:", error);
        }
    }

    const fetchSubjectId = async () => {
        try {
            const response = await httpClient.get(
                `/SubjectVideoGroupAssignment/${id}`,
                { withCredentials: true }
            );
            setSubjectId(response.data.subjectId || null);
        } catch (error) {
            console.error("Error fetching subject video group assignment:", error);
        }
    };

    const fetchLabels = async () => {
        if (subjectId === null) return;

        try {
            const response = await httpClient.get(`/subject/${subjectId}/label`, {
                withCredentials: true,
            });
            setLabels(response.data || []);
        } catch (error) {
            console.error("Error fetching labels:", error);
        }
    };

    const fetchAssignedLabels = async () => {
        if (subjectId === null) return;

        try {
            const response = await httpClient.get(`/AssignedLabel`, {
                withCredentials: true,
            });
            setAssignedLabels(response.data || []);
        } catch (error) {
            console.error("Error fetching assigned labels:", error);
        }
    };

    const handlePlayStop = () => {
        if (isPlaying) {
            videoRefs.current.forEach((video) => {
                if (video) {
                    video.pause();
                }
            });
        } else {
            videoRefs.current.forEach((video) => {
                if (video) {
                    video.play();
                }
            });
        }
        setIsPlaying(!isPlaying);
    };

    const labelStateRef = useRef({});

    const handleLabelClick = (labelId) => {
        const video = videoRefs.current[0];
        if (!video) return;

        const time = video.currentTime;

        setLabelTimestamps((prevTimestamps) => {
            const newTimestamps = {};
            Object.keys(prevTimestamps).forEach((key) => {
                if (parseInt(key) !== labelId) {
                    delete labelStateRef.current[key];
                }
            });

            const currentLabel = prevTimestamps[labelId] || {};
            if (!currentLabel.start) {
                newTimestamps[labelId] = { start: time, sent: false };
            } else if (!currentLabel.sent) {
                newTimestamps[labelId] = { ...currentLabel, sent: true };
            }
            return newTimestamps;
        });

        if (labelStateRef.current[labelId]?.start && !labelStateRef.current[labelId]?.sent) {
            sendLabelData(labelId, labelStateRef.current[labelId].start, time);
        } else {
            labelStateRef.current[labelId] = { start: time, sent: false };
        }
    };

    const isMeasuring = (labelId) => {
        return labelTimestamps[labelId] && !labelTimestamps[labelId].sent;
    };

    const formatTime = (seconds) => {
        const hours = Math.floor(seconds / 3600);
        const minutes = Math.floor((seconds % 3600) / 60);
        const remainingSeconds = Math.floor(seconds % 60);

        return `${padZero(hours)}:${padZero(minutes)}:${padZero(remainingSeconds)}`;
    };

    const padZero = (num) => num.toString().padStart(2, "0");

    const sendLabelData = async (labelId, start, end) => {
        const labelerId = 1;
        const startFormatted = formatTime(start);
        const endFormatted = formatTime(end);

        const data = {
            labelId,
            subjectVideoGroupAssignmentId: subjectId,
            labelerId,
            start: startFormatted,
            end: endFormatted
        };

        const newAssignedLabel = {
            labelId,
            start: startFormatted,
            end: endFormatted,
            colorHex: labels.find((label) => label.id === labelId)?.colorHex || '#000000',
        };

        try {
            await httpClient.post('/AssignedLabel', data);

            setAssignedLabels((prevLabels) => [newAssignedLabel, ...prevLabels]);

            setLabelTimestamps((prev) => {
                const newTimestamps = { ...prev };
                delete newTimestamps[labelId]; 
                return newTimestamps;
            });

            labelStateRef.current[labelId] = { start: null, sent: false }; 

            fetchAssignedLabels();
        } catch (error) {
            console.error('Error assigning label:', error);
        }
    };

    const handleTimeUpdate = () => {
        const video = videoRefs.current[0];
        if (video) {
            setCurrentTime(video.currentTime);
            setDuration(video.duration);
        }
    };

    const handleDelete = async (id) => {
        if (!window.confirm("Are you sure you want to delete this assigned label?"))
            return;

        try {
            await httpClient.delete(`/AssignedLabel/${id}`);
            setAssignedLabels((prev) =>
                prev.filter((assignedLabel) => assignedLabel.id !== id)
            );
        } catch (error) {
            setError(
                error.response?.data?.message || "Failed to delete assigned label"
            );
        }
    };

    useEffect(() => {
        const handleKeyPress = (event) => {
            const label = labels.find(
                (l) => l.shortcut.toLowerCase() === event.key.toLowerCase()
            );
            if (label) {
                handleLabelClick(label.id);
            }
        };

        window.addEventListener("keydown", handleKeyPress);
        return () => {
            window.removeEventListener("keydown", handleKeyPress);
        };
    }, [labels]);

    const handleRewind = (time) => {
        const video = videoRefs.current[0];
        if (video) {
            video.currentTime = Math.max(video.currentTime - time, 0);
            setCurrentTime(video.currentTime);
        }
    };

    const handleFastForward = (time) => {
        const video = videoRefs.current[0];
        if (video) {
            video.currentTime = Math.min(video.currentTime + time, video.duration);
            setCurrentTime(video.currentTime);
        }
    };

    return (
        <div className="container">
            <div className="content">
                <div className="container" id="video-container">
                    <div className="row" id="video-row">
                        {streams.length > 0 ? (
                            streams.map((streamUrl, index) => (
                                <div
                                    className={`col-12 ${streams.length === 1
                                            ? ""
                                            : streams.length <= 4
                                                ? "col-md-5"
                                                : "col-md-4"
                                        }`}
                                    key={index}
                                >
                                    <div className="video-cell">
                                        <video
                                            ref={(el) => (videoRefs.current[index] = el)}
                                            width="100%"
                                            height="auto"
                                            src={streamUrl}
                                            type="video/mp4"
                                            controls
                                            onTimeUpdate={handleTimeUpdate}
                                        />
                                    </div>
                                </div>
                            ))
                        ) : (
                            <p>Loading video streams...</p>
                        )}
                    </div>
                    <div className="row">
                        <div className="col-12">
                            <div className="pagination d-flex justify-content-between">
                                <button
                                    className="btn btn-primary"
                                    onClick={() => onBatchChangedAsync(currentBatch - 1)}
                                    disabled={currentBatch <= 1}
                                >
                                    Previous
                                </button>
                                <button
                                    className="btn btn-primary"
                                    onClick={() => onBatchChangedAsync(currentBatch + 1)}
                                    disabled={currentBatch === Object.keys(videoPositions).length}
                                >
                                    Next
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
                <div className="progress-bar">
                    <input
                        type="range"
                        min="0"
                        max={duration || 100}
                        value={currentTime}
                        onChange={(e) => {
                            const video = videoRefs.current[0];
                            if (video) {
                                const newTime = parseFloat(e.target.value);
                                video.currentTime = newTime;
                                setCurrentTime(newTime);
                            }
                        }}
                    />
                </div>
                <div className="controls">
                    <div className="seek-buttons">
                        <button className="btn btn-primary seek-btn" onClick={() => handleRewind(5)}>
                            <i className="fas fa-backward">
                                <p>-5s</p>
                            </i>
                        </button>
                        <button className="btn btn-primary seek-btn" onClick={() => handleRewind(1)}>
                            <i className="fas fa-backward">
                                <p>-1s</p>
                            </i>
                        </button>
                    </div>
                    <button
                        className="btn btn-primary play-stop-btn"
                        onClick={handlePlayStop}
                    >
                        <i className={`fas ${isPlaying ? "fa-stop" : "fa-play"}`}></i>
                    </button>
                    <div className="seek-buttons">
                    <button
                            className="btn btn-primary seek-btn"
                            onClick={() => handleFastForward(1)}
                        >
                            <i className="fas fa-forward">
                                <p>+1s</p>
                            </i>
                        </button>
                        <button
                            className="btn btn-primary seek-btn"
                            onClick={() => handleFastForward(5)}
                        >
                            <i className="fas fa-forward">
                                <p>+5s</p>
                            </i>
                        </button>
                    </div>
                    <span className="time-display">
                        {formatTime(currentTime)} / {formatTime(isNaN(duration) ? 0 : duration)}
                    </span>
                </div>

                <div className="labels-container">
                    {labels.length > 0 ? (
                        labels.map((label, index) => {
                            const measuring = isMeasuring(label.id);
                            return (
                                <button
                                    key={index}
                                    className="btn label-btn"
                                    style={{ backgroundColor: label.colorHex }}
                                    onClick={() => handleLabelClick(label.id)}
                                >
                                    {label.name + " [" + label.shortcut + "]"}{" "}
                                    {measuring ? "STOP" : "START"}
                                </button>
                            );
                        })
                    ) : (
                        <p>No labels available</p>
                    )}
                </div>

                <div className="assigned-labels">
                    <h3>Assigned Labels:</h3>
                    <div className="assigned-labels-table">
                        {assignedLabels.length > 0 ? (
                            <table className="normal-table">
                                <thead className="table-dark">
                                    <tr>
                                        <th>Label ID</th>
                                        <th>Start Time</th>
                                        <th>End Time</th>
                                        <th>Action</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {assignedLabels
                                        .slice()
                                        .reverse()
                                        .map((label, index) => {
                                            const matchingLabel = labels.find(
                                                (l) => l.id === label.labelId
                                            );
                                            return (
                                                <tr key={index}>
                                                    <td>
                                                        {matchingLabel ? matchingLabel.name : "Unknown"}
                                                    </td>
                                                    <td>{label.start}</td>
                                                    <td>{label.end}</td>
                                                    <td>
                                                        <button
                                                            className="btn btn-danger"
                                                            onClick={() => handleDelete(label.id)}
                                                        >
                                                            Delete
                                                        </button>
                                                    </td>
                                                </tr>
                                            );
                                        })}
                                </tbody>
                            </table>
                        ) : (
                            <div className="text-center py-4">
                                <i className="fas fa-tags fs-1 text-muted"></i>
                                <p className="text-muted mt-2">No labels assigned yet</p>
                            </div>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Videos;
