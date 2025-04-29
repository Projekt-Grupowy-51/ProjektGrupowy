import { useState, useEffect, useCallback } from "react";

const useVideoControls = (videoRefs, batchState) => {
    const [playerState, setPlayerState] = useState({
        isPlaying: false,
        currentTime: 0,
        duration: 0,
        timeLeft: 0,
        playbackSpeed: 1.0,
    });

    const syncVideos = useCallback((action, value = null) => {
        videoRefs.current.forEach((video) => {
            if (!video) return;
            switch (action) {
                case "play":
                    video.play().catch(console.error);
                    break;
                case "pause":
                    video.pause();
                    break;
                case "setTime":
                    video.currentTime = value;
                    break;
                case "setSpeed":
                    video.playbackRate = value;
                    break;
                case "reset":
                    video.pause();
                    video.currentTime = 0;
                    break;
            }
        });
    }, [videoRefs]);

    const handlePlayStop = useCallback(() => {
        syncVideos(playerState.isPlaying ? "pause" : "play");
        setPlayerState((prev) => ({ ...prev, isPlaying: !prev.isPlaying }));
    }, [playerState.isPlaying, syncVideos]);

    const handleSeek = useCallback(
        (newTime) => {
            syncVideos("setTime", newTime);
            setPlayerState((prev) => ({ ...prev, currentTime: newTime }));
        },
        [syncVideos]
    );

    const handleSpeedChange = useCallback(
        (speed) => {
            syncVideos("setSpeed", speed);
            setPlayerState((prev) => ({ ...prev, playbackSpeed: speed }));
        },
        [syncVideos]
    );

    const handleTimeUpdate = useCallback(() => {
        const video = videoRefs.current[0];
        if (!video) return;

        setPlayerState((prev) => ({
            ...prev,
            currentTime: video.currentTime,
            duration: video.duration,
            timeLeft: Math.round(video.duration - video.currentTime),
        }));
    }, [videoRefs]);

    const handleKeyPress = useCallback(
        (event) => {
            const { key } = event;
            const video = videoRefs.current[0];
            if (!video) return;

            switch (key) {
                case " ":
                    event.preventDefault();
                    handlePlayStop();
                    break;
                case "ArrowLeft":
                    event.preventDefault();
                    handleSeek(Math.max(video.currentTime - 1, 0));
                    break;
                case "ArrowRight":
                    event.preventDefault();
                    handleSeek(Math.min(video.currentTime + 1, video.duration));
                    break;
            }
        },
        [handlePlayStop, handleSeek, videoRefs]
    );

    useEffect(() => {
        videoRefs.current.forEach((video) => {
            if (video) video.playbackRate = playerState.playbackSpeed;
        });
    }, [playerState.playbackSpeed, videoRefs]);

    return {
        playerState,
        controls: {
            handlePlayStop,
            handleSeek,
            handleSpeedChange,
            handleTimeUpdate,
        },
        handleKeyPress,
    };
};

export default useVideoControls;