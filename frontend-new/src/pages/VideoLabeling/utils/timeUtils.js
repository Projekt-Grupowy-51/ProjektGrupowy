export const formatTime = (seconds) => {
  if (!seconds || isNaN(seconds)) return '00:00:00.000';
  
  const hours = Math.floor(seconds / 3600);
  const minutes = Math.floor((seconds % 3600) / 60);
  const secs = Math.floor(seconds % 60);
  const milliseconds = Math.floor((seconds % 1) * 1000);
  
  return `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}.${milliseconds.toString().padStart(3, '0')}`;
};

export const parseTime = (timeString) => {
  if (!timeString) return 0;
  
  const parts = timeString.split(':');
  if (parts.length !== 3) return 0;
  
  const hours = parseInt(parts[0]) || 0;
  const minutes = parseInt(parts[1]) || 0;
  const [seconds, milliseconds] = parts[2].split('.');
  
  return hours * 3600 + minutes * 60 + parseInt(seconds) + (parseInt(milliseconds) || 0) / 1000;
};

export const formatDuration = (seconds) => {
  if (!seconds || isNaN(seconds)) return '00:00';
  
  const minutes = Math.floor(seconds / 60);
  const secs = Math.floor(seconds % 60);
  
  return `${minutes.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
};

export const getProgressPercentage = (currentTime, duration) => {
  if (!duration || duration === 0) return 0;
  return Math.min((currentTime / duration) * 100, 100);
};