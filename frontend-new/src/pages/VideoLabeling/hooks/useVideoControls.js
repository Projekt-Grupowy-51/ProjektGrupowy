import { useState, useRef } from 'react';
import { validateVideoTime } from '../utils/videoUtils.js';

export const useVideoControls = () => {
  const [isPlaying, setIsPlaying] = useState(false);
  const [currentTime, setCurrentTime] = useState(0);
  const [duration, setDuration] = useState(0);
  const [playbackSpeed, setPlaybackSpeed] = useState(1.0);
  
  const videoRefs = useRef([]);

  const syncVideos = (action, value = null) => {
    if (!videoRefs.current) return;
    
    videoRefs.current.forEach((video) => {
      if (!video) return;
      
      switch (action) {
        case 'play':
          video.play().catch(() => {}); // Silent fail dla video.play()
          break;
        case 'pause':
          video.pause();
          break;
        case 'setTime':
          if (typeof value === 'number') {
            video.currentTime = validateVideoTime(value, video.duration);
          }
          break;
        case 'setSpeed':
          if (typeof value === 'number' && value > 0) {
            video.playbackRate = value;
          }
          break;
        case 'reset':
          video.pause();
          video.currentTime = 0;
          video.playbackRate = 1.0;
          break;
      }
    });
  };

  const handlePlayPause = () => {
    const newIsPlaying = !isPlaying;
    syncVideos(newIsPlaying ? 'play' : 'pause');
    setIsPlaying(newIsPlaying);
  };

  const handleSeek = (newTime) => {
    const validTime = validateVideoTime(newTime, duration);
    syncVideos('setTime', validTime);
    setCurrentTime(validTime);
  };

  const handleSpeedChange = (speed) => {
    syncVideos('setSpeed', speed);
    setPlaybackSpeed(speed);
  };

  const handleTimeUpdate = () => {
    const video = videoRefs.current[0];
    if (!video) return;
    
    setCurrentTime(video.currentTime);
    if (video.duration) {
      setDuration(video.duration);
    }
  };

  const resetPlayerState = () => {
    setIsPlaying(false);
    setCurrentTime(0);
    setDuration(0);
    setPlaybackSpeed(1.0);
  };

  return {
    playerState: {
      isPlaying,
      currentTime,
      duration,
      playbackSpeed
    },
    videoRefs,
    syncVideos,
    handlePlayPause,
    handleSeek,
    handleSpeedChange,
    handleTimeUpdate,
    resetPlayerState
  };
};