// Label management utilities
export const LABEL_TYPES = {
  POINT: 'point',
  RANGE: 'range'
};

export const getTextColor = (hexColor) => {
  if (!hexColor) return '#000000';
  
  const hex = hexColor.replace('#', '');
  const r = parseInt(hex.substr(0, 2), 16);
  const g = parseInt(hex.substr(2, 2), 16);
  const b = parseInt(hex.substr(4, 2), 16);
  
  const luminance = (0.299 * r + 0.587 * g + 0.114 * b) / 255;
  return luminance > 0.5 ? '#000000' : '#ffffff';
};

export const validateLabelTimes = (startTime, endTime, videoDuration) => {
  const start = Math.max(0, Math.min(startTime, videoDuration || Infinity));
  const end = Math.max(start, Math.min(endTime, videoDuration || Infinity));
  return { startTime: start, endTime: end };
};

export const getLabelButtonText = (label, labelState = {}) => {
  if (!label) return '';
  
  let text = label.name;
  if (label.shortcut) text += ` [${label.shortcut}]`;
  
  if (label.type === LABEL_TYPES.RANGE) {
    text += labelState.isActive ? ' STOP' : ' START';
  } else if (label.type === LABEL_TYPES.POINT) {
    text += ' ADD POINT';
  }
  
  return text;
};

export const sortAssignedLabels = (labels) => {
  if (!Array.isArray(labels)) return [];
  return [...labels].sort((a, b) => {
    // Try different possible date field names
    const dateA = new Date(a.insDate || a.InsDate || a.createdAt || a.timestamp || 0);
    const dateB = new Date(b.insDate || b.InsDate || b.createdAt || b.timestamp || 0);
    return dateB - dateA; // Most recent first
  });
};

