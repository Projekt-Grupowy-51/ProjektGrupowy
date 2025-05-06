import React, { useState, useEffect } from "react";

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
            <div className="container" id="video-container">
                <div
                    className="row"
                    id="video-row"
                    style={
                        streams.length === 2
                            ? {
                                  display: "flex",
                                  justifyContent: "space-between",
                              }
                            : streams.length >= 3 && streams.length <= 4
                            ? {
                                  display: "grid",
                                  gridTemplateColumns: "1fr 1fr",
                                  gridTemplateRows: "1fr 1fr",
                                  gap: "10px",
                                  height: "75vh",
                                  placeItems: "center",
                              }
                            : {}
                    }
                >
                    {streams.map((stream, index) => (
                        <div
                            key={index}
                            style={
                                streams.length === 2
                                    ? {
                                          width: "50%",
                                          height: "auto",
                                      }
                                    : streams.length >= 3 && streams.length <= 4
                                    ? {
                                          height: "calc(75vh / 2)",
                                          width: "calc((75vh / 2) * (16 / 9))",
                                      }
                                    : {}
                            }
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
        </div>
    );
};

export default VideoPlayers;