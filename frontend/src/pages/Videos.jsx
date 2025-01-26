import React, { useState, useEffect, useRef } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import './css/ScientistProjects.css';

const Videos = () => {
    const { id } = useParams(); // Get `id` from URL
    const [videos, setVideos] = useState([]); // Renamed from labels to videos
    const [streams, setStreams] = useState([]); // State for video streams
    const [isPlaying, setIsPlaying] = useState(false); // Play/Stop state
    const [currentTime, setCurrentTime] = useState(0); // Shared current time for all videos
    const [duration, setDuration] = useState(0); // Shared video duration
    const videoRefs = useRef([]); // Refs for video elements
    const navigate = useNavigate();

    // Fetch video group details
    async function fetchVideoGroupDetails() {
        try {
            const response = await fetch(`http://localhost:5000/api/videogroup/${id}`);
            if (!response.ok) {
                throw new Error('Failed to fetch video group details');
            }
            const data = await response.json();
            //setVideoGroupDetails(data);
            fetchVideos(data); // Fetch videos associated with the video group
        } catch (error) {
            console.error('Error fetching video group details:', error);
        }
    }

    // Fetch the list of videos
    async function fetchVideos(videoGroupData) {
        try {
            const response = await fetch(`http://localhost:5000/api/VideoGroup/${id}/videos`);
            if (!response.ok) {
                throw new Error('Failed to fetch videos');
            }
            const data = await response.json();
            setVideos(data); // Set the fetched video data

            // Fetch video streams after fetching videos
            fetchVideoStreams(data);
        } catch (error) {
            console.error('Error fetching videos:', error);
        }
    }

    // Fetch video stream URLs for a given set of videos
    async function fetchVideoStreams(videos) {
        const videoIds = videos.slice(0, 4).map((video) => video.id);
        const streamsPromises = videoIds.map(async (videoId) => {
            const response = await fetch(`http://localhost:5000/api/Video/${videoId}/stream`);
            if (!response.ok) {
                throw new Error('Failed to fetch video stream');
            }
            return response.url; // Get the stream URL
        });

        try {
            const streamUrls = await Promise.all(streamsPromises);
            setStreams(streamUrls); // Set the stream URLs for the 4 videos
        } catch (error) {
            console.error('Error fetching video streams:', error);
        }
    }

    // Fetch video group details when component is mounted
    useEffect(() => {
        if (id) fetchVideoGroupDetails();
    }, [id]);

    // Handle play/stop for all videos
    const handlePlayStop = () => {
        if (isPlaying) {
            // Pause all videos
            videoRefs.current.forEach((video) => video.pause());
        } else {
            // Play all videos
            videoRefs.current.forEach((video) => video.play());
        }
        setIsPlaying(!isPlaying); // Toggle play/stop state
    };

    // Handle scrubbing the timeline
    const handleTimelineChange = (event) => {
        const newTime = parseFloat(event.target.value);
        setCurrentTime(newTime); // Update shared time
        videoRefs.current.forEach((video) => video.currentTime = newTime); // Set the new time for all videos
    };

    // Handle video time updates
    const handleTimeUpdate = () => {
        // Find the maximum current time across all videos
        const maxTime = Math.max(...videoRefs.current.map((video) => video.currentTime));
        setCurrentTime(maxTime); // Update the shared current time
    };

    // Set video duration once the first video has been loaded
    const setVideoDuration = () => {
        if (videoRefs.current[0]) {
            setDuration(videoRefs.current[0].duration);
        }
    };

    // Check if videos exists before rendering
    if (!videos) return <div>Loading...</div>;

    return (
        <div className="container">
            <div className="content">
                <h2>Video Stream Preview (2x2 Grid)</h2>
                <div className="video-grid">
                    {streams.length > 0 ? (
                        streams.map((streamUrl, index) => (
                            <div className="video-container" key={index}>
                                <video
                                    ref={(el) => videoRefs.current[index] = el}
                                    width="300"
                                    height="200"
                                    src={streamUrl}
                                    type="video/mp4"
                                    onTimeUpdate={handleTimeUpdate}
                                    onLoadedMetadata={setVideoDuration}
                                >
                                    Your browser does not support the video tag.
                                </video>
                            </div>
                        ))
                    ) : (
                        <p>Loading video streams...</p>
                    )}
                </div>

                <div className="timeline-container">
                    <input
                        type="range"
                        min="0"
                        max={duration}
                        value={currentTime}
                        onChange={handleTimelineChange}
                        step="0.1"
                        className="timeline"
                    />
                    <span>{Math.floor(currentTime)} / {Math.floor(duration)} sec</span>
                </div>

                <button className="play-stop-btn" onClick={handlePlayStop}>
                    {isPlaying ? 'Stop All Videos' : 'Play All Videos'}
                </button>
            </div>
        </div>
    );
};

export default Videos;
