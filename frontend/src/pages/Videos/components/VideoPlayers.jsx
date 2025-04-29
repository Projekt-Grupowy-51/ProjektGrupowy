import React from "react";

const VideoPlayers = ({ streams, videoRefs, onTimeUpdate, onEnded }) => (
    <div className="row" id="video-row">
        {streams.map((stream, index) => (
            <div className="col-12 col-md-6" key={index}>
                <div className="video-cell">
                    <video
                        ref={(el) => (videoRefs.current[index] = el)}
                        width="100%"
                        height="auto"
                        src={stream}
                        onTimeUpdate={onTimeUpdate}
                        onEnded={() => onEnded(index)}
                    />
                </div>
            </div>
        ))}
    </div>
);

export default VideoPlayers;