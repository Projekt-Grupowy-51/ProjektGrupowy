import React, { useState, useEffect } from "react";
import "../css/VideoPlayers.css"; 

const VideoPlayers = ({ streams, videoRefs, onTimeUpdate, onAllVideosEnded, resetTrigger }) => {
    const [endedVideos, setEndedVideos] = useState(new Set());

    const handleVideoEnded = (index) => {
        setEndedVideos((prev) => {
            const updated = new Set(prev);
            updated.add(index);

            if (updated.size === streams.length && typeof onAllVideosEnded === "function") {
                onAllVideosEnded();
            }

            return updated;
        });
    };

    useEffect(() => {
        setEndedVideos(new Set());
    }, [resetTrigger]);

    return (
        <div className="content">
            <div
                className={`container ${
                    streams.length === 2
                        ? "video-container-two"
                        : streams.length >= 3 && streams.length <= 4
                        ? "video-container-grid"
                        : "video-container-default"
                }`}
                id="video-container"
            >
                    {streams.map((stream, index) => (
                        <div
                            key={index}
                            className={`${
                                streams.length === 2
                                    ? "video-cell-two"
                                    : streams.length >= 3 && streams.length <= 4
                                    ? "video-cell-grid"
                                    : "video-cell-default"
                            }`}
                        >
                            <div className="video-cell">
                                <video
                                    ref={(el) => (videoRefs.current[index] = el)}
                                    width="100%"
                                    height="100%"
                                    src={stream}
                                    type="video/mp4"
                                    onTimeUpdate={onTimeUpdate}
                                    onEnded={() => handleVideoEnded(index)}
                                />
                            </div>
                        </div>
                    ))}
            </div>
        </div>
    );
};

export default VideoPlayers;
