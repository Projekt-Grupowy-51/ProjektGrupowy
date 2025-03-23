import React, { useState, useEffect, useRef } from 'react';
import { useParams } from 'react-router-dom';
import httpClient from '../httpClient';
import './css/ScientistProjects.css';

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
            const response = await httpClient.get(`/VideoGroup/${videoId}`, { withCredentials: true });
            setVideos(response.data.videos || []);
            fetchVideos(response.data.videos || []);
        } catch (error) {
            console.error('Failed to load video group details');
        }
    };

    const fetchVideos = async () => {
        try {
            const response = await httpClient.get(`/VideoGroup/${id}/videos`, { withCredentials: true });
            setVideos(response.data);
            fetchVideoStreams(response.data);
        } catch (error) {
            console.error('Failed to load video list');
        }
    };

    async function fetchVideoStreams(videos) {
        const videoIds = videos.slice(0, 4).map((video) => video.id);
        const streamsPromises = videoIds.map(async (videoId) => {
            try {
                const response = await httpClient.get(`/Video/${videoId}/stream`, {
                    withCredentials: true,
                    responseType: 'blob'
                });
                return URL.createObjectURL(response.data);
            } catch (error) {
                return null;
            }
        });

        try {
            const streamUrls = await Promise.all(streamsPromises);
            setStreams(streamUrls.filter(url => url !== null));
        } catch (error) {
            console.error('Error fetching video streams:', error);
        }
    }

    const fetchSubjectId = async () => {
        try {
            const response = await httpClient.get(`/SubjectVideoGroupAssignment/${id}`, { withCredentials: true });
            setSubjectId(response.data.subjectId || null);
        } catch (error) {
            console.error('Error fetching subject video group assignment:', error);
        }
    };

    const fetchLabels = async () => {
        if (subjectId === null) return;

        try {
            const response = await httpClient.get(`/Label/${subjectId}/subject`, { withCredentials: true });
            setLabels(response.data || []);
        } catch (error) {
            console.error('Error fetching labels:', error);
        }
    };

    const fetchAssignedLabels = async () => {
        if (subjectId === null) return;

        try {
            const response = await httpClient.get(`/AssignedLabel`, { withCredentials: true });
            setAssignedLabels(response.data || []);
        } catch (error) {
            console.error('Error fetching assigned labels:', error);
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

        const labelState = labelStateRef.current[labelId] || {};

        setLabelTimestamps((prevTimestamps) => {
            const newTimestamps = { ...prevTimestamps };
            const currentLabel = newTimestamps[labelId] || {};
            if (!currentLabel.start) {
                newTimestamps[labelId] = { start: time, sent: false }; 
            } else if (!currentLabel.sent) {
                newTimestamps[labelId] = { ...currentLabel, sent: true }; 
            }
            return newTimestamps;
        });

        if (labelState.start && !labelState.sent) {
            sendLabelData(labelId, labelState.start, time);
            console.log("jestem tu teraz, " + time);
        } else if (!labelState.start) {
            labelStateRef.current[labelId] = { start: time, sent: false };
            console.log("Starting to measure: ", labelId, time);
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

    const padZero = (num) => num.toString().padStart(2, '0');

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
                if (newTimestamps[labelId]) {
                    newTimestamps[labelId].sent = true;
                }
                return newTimestamps;
            });

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
        if (!window.confirm('Are you sure you want to delete this assigned label?')) return;

        try {
            await httpClient.delete(`/AssignedLabel/${id}`);
            setAssignedLabels(prev => prev.filter(assignedLabel => assignedLabel.id !== id));
        } catch (error) {
            setError(error.response?.data?.message || 'Failed to delete assigned label');
        }
    };

    useEffect(() => {
        const handleKeyPress = (event) => {
            const label = labels.find(l => l.shortcut.toLowerCase() === event.key.toLowerCase());
            if (label) {
                handleLabelClick(label.id);
            }
        };

        window.addEventListener("keydown", handleKeyPress);
        return () => {
            window.removeEventListener("keydown", handleKeyPress);
        };
    }, [labels]);

    const handleRewind = () => {
        const video = videoRefs.current[0];
        if (video) {
            video.currentTime = Math.max(video.currentTime - 5, 0); 
            setCurrentTime(video.currentTime);
        }
    };

    const handleFastForward = () => {
        const video = videoRefs.current[0];
        if (video) {
            video.currentTime = Math.min(video.currentTime + 5, video.duration);  
            setCurrentTime(video.currentTime);
        }
    };

    return (
        <div className="container">
            <div className="content">
                {/* <h2>Video Stream Preview</h2> */}
                <div className="container" id="video-container">
                    <div className="row" id='video-row'>
                        {streams.length > 0 ? (
                            streams.map((streamUrl, index) => (
                                <div
                                    className={`col-12 ${
                                        streams.length === 1
                                            ? ''
                                            : streams.length <= 4
                                            ? 'col-md-5'
                                            : 'col-md-4'
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
                                video.currentTime = e.target.value;
                                setCurrentTime(e.target.value);
                            }
                        }}
                    />
                </div>
                <div className="controls">
                    <div className="seek-buttons">
                        <button className="btn btn-primary seek-btn" onClick={handleRewind}>
                            <i className="fas fa-backward"><p>5s</p></i>
                        </button>
                    </div>
                    <button className="btn btn-primary play-stop-btn" onClick={handlePlayStop}>
                        <i className={`fas ${isPlaying ? 'fa-stop' : 'fa-play'}`}></i>
                    </button>
                    <div className="seek-buttons">

                        <button className="btn btn-primary seek-btn" onClick={handleFastForward}>
                            <i className="fas fa-forward"><p>5s</p></i>
                        </button>
                    </div>
                    <span>{currentTime.toFixed(2)} / {(isNaN(duration) ? 0 : duration).toFixed(2)} s</span>
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
                                    {label.name + ' [' + label.shortcut + ']'} {measuring ? 'STOP' : 'start'}
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
                                {assignedLabels.slice().reverse().map((label, index) => {
                                    const matchingLabel = labels.find(l => l.id === label.labelId);
                                    return (
                                        <tr key={index}>
                                            <td>{matchingLabel ? matchingLabel.name : "Unknown"}</td>
                                            <td>{label.start}</td>
                                            <td>{label.end}</td>
                                            <td><button className="btn btn-danger" onClick={() => handleDelete(label.id)}>Delete</button></td>
                                        </tr>
                                    );
                                })}
                            </tbody>

                        </table>
                    ) : (
                        <p>No labels assigned yet</p>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Videos;
