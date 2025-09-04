export const syncVideos = (videoRefs, action, value = null) => {
  if (!videoRefs?.current) return;
  
  videoRefs.current.forEach((video) => {
    if (!video) return;
    
    switch (action) {
      case 'play':
        video.play().catch(() => {}); // Silent fail
        break;
      case 'pause':
        video.pause();
        break;
      case 'setTime':
        if (typeof value === 'number' && !isNaN(value)) {
          video.currentTime = Math.max(0, Math.min(value, video.duration || 0));
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

export const getVideoGridLayout = (videoCount) => {
  if (videoCount <= 1) return 'single-video';
  if (videoCount === 2) return 'two-videos';
  if (videoCount <= 4) return 'four-videos';
  return 'many-videos';
};


export const validateVideoTime = (time, duration) => {
  if (typeof time !== 'number' || isNaN(time)) return 0;
  if (typeof duration !== 'number' || isNaN(duration)) return time;
  return Math.max(0, Math.min(time, duration));
};