export const syncVideos = (videoRefs, action, value = null) => {
  if (!videoRefs?.current) return;
  
  videoRefs.current.forEach((video) => {
    if (!video) return;
    
    try {
      switch (action) {
        case 'play':
          video.play().catch(console.error);
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
    } catch (error) {
      console.error(`Error during video sync action ${action}:`, error);
    }
  });
};

export const getVideoGridLayout = (videoCount) => {
  if (videoCount <= 1) return 'single-video';
  if (videoCount === 2) return 'two-videos';
  if (videoCount <= 4) return 'four-videos';
  return 'many-videos';
};

export const generateFakeVideoStreams = (count = 2) => {
  const fakeStreams = [
    'https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4',
    'https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ElephantsDream.mp4',
    'https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ForBiggerBlazes.mp4',
    'https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ForBiggerEscapes.mp4',
  ];
  
  return Array.from({ length: Math.min(count, 4) }, (_, index) => ({
    id: index + 1,
    url: fakeStreams[index] || fakeStreams[0],
    title: `Video ${index + 1}`,
    duration: 30 + Math.random() * 60 // 30-90 seconds
  }));
};

export const validateVideoTime = (time, duration) => {
  if (typeof time !== 'number' || isNaN(time)) return 0;
  if (typeof duration !== 'number' || isNaN(duration)) return time;
  return Math.max(0, Math.min(time, duration));
};