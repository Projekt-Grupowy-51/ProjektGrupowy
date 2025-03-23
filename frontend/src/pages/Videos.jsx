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
            const response = await httpClient.get(`/subject/${subjectId}/label`, { withCredentials: true });
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
                <div className="container" id="video-container">
                    <div className="row" id='video-row'>
                        {streams.length > 0 ? (
                            streams.map((streamUrl, index) => (
                                <div
                                    className={`col-12 ${
                                        streams.length === 1
                                            ? ''
                                            : streams.length <= 4
                                            ? 'col-md-6'
                                            : 'col-md-4'
                                    }`}
                                    key={index}
                                >
                                    <div className="video-cell">
                                        <video
                                            ref={(el) => (videoRefs.current[index] = el)}
                                            className="w-100 rounded shadow-sm"
                                            src={streamUrl}
                                            type="video/mp4"
                                            controls
                                            onTimeUpdate={handleTimeUpdate}
                                        />
                                    </div>
                                </div>
                            ))
                        ) : (
                            <div className="col-12 text-center py-5">
                                <div className="spinner-border text-primary" role="status">
                                    <span className="visually-hidden">Loading...</span>
                                </div>
                                <p className="mt-3">Loading video streams...</p>
                            </div>
                        )}
                    </div>
                </div>
                
                <div className="card shadow-sm mb-4">
                    <div className="card-body">
                        <div className="progress mb-3">
                            <input
                                type="range"
                                className="form-range"
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
                        
                        <div className="d-flex justify-content-between align-items-center">
                            <div className="btn-group">
                                <button className="btn btn-primary" onClick={handleRewind}>
                                    <i className="fas fa-backward me-1"></i>5s
                                </button>
                                <button className="btn btn-primary" onClick={handlePlayStop}>
                                    <i className={`fas ${isPlaying ? 'fa-stop' : 'fa-play'}`}></i>
                                </button>
                                <button className="btn btn-primary" onClick={handleFastForward}>
                                    <i className="fas fa-forward me-1"></i>5s
                                </button>
                            </div>
                            <span className="badge bg-secondary fs-6">
                                {currentTime.toFixed(2)} / {(isNaN(duration) ? 0 : duration).toFixed(2)} s
                            </span>
                        </div>
                    </div>
                </div>

                <div className="card shadow-sm mb-4">
                    <div className="card-header bg-primary text-white">
                        <h5 className="card-title mb-0">Available Labels</h5>
                    </div>
                    <div className="card-body">
                        <div className="d-flex flex-wrap justify-content-center">
                            {labels.length > 0 ? (
                                labels.map((label, index) => {
                                    const measuring = isMeasuring(label.id);
                                    return (
                                        <button
                                            key={index}
                                            className="btn m-1"
                                            style={{ 
                                                backgroundColor: label.colorHex,
                                                color: isLightColor(label.colorHex) ? '#000' : '#fff'
                                            }}
                                            onClick={() => handleLabelClick(label.id)}
                                        >
                                            {label.name} [{label.shortcut}] {measuring ? 'STOP' : 'start'}
                                        </button>
                                    );
                                })
                            ) : (
                                <p className="text-center">No labels available</p>
                            )}
                        </div>
                    </div>
                </div>

                <div className="assigned-labels">
                    <h3>Assigned Labels</h3>
                    <div className="assigned-labels-table">
                        {assignedLabels.length > 0 ? (
                            <table className="normal-table">
                                <thead>
                                    <tr>
                                        <th>Label</th>
                                        <th>Start Time</th>
                                        <th>End Time</th>
                                        <th>Actions</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {assignedLabels.slice().reverse().map((label, index) => {
                                        const matchingLabel = labels.find(l => l.id === label.labelId);
                                        return (
                                            <tr key={index}>
                                                <td>
                                                    <div className="d-flex align-items-center">
                                                        <div 
                                                            className="color-preview me-2" 
                                                            style={{ 
                                                                backgroundColor: matchingLabel?.colorHex || "#000000",
                                                                width: '20px',
                                                                height: '20px'
                                                            }}
                                                        ></div>
                                                        {matchingLabel ? matchingLabel.name : "Unknown"}
                                                    </div>
                                                </td>
                                                <td>{label.start}</td>
                                                <td>{label.end}</td>
                                                <td>
                                                    <button 
                                                        className="btn btn-danger btn-sm" 
                                                        onClick={() => handleDelete(label.id)}
                                                    >
                                                        <i className="fas fa-trash"></i>
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

    // Helper function to determine if a color is light or dark
    function isLightColor(color) {
        const hex = color.replace('#', '');
        const r = parseInt(hex.substr(0, 2), 16);
        const g = parseInt(hex.substr(2, 2), 16);
        const b = parseInt(hex.substr(4, 2), 16);
        const brightness = (r * 299 + g * 587 + b * 114) / 1000;
        return brightness > 155;
    }
};

export default Videos;
