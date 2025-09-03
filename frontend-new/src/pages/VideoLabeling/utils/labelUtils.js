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
  return [...labels].sort((a, b) => (a.startTime || 0) - (b.startTime || 0));
};

export const generateFakeLabels = () => [
  {
    id: 1,
    name: 'Speaking',
    shortcut: 'S',
    type: LABEL_TYPES.RANGE,
    colorHex: '#3498db',
    description: 'Person is speaking'
  },
  {
    id: 2,
    name: 'Gesture',
    shortcut: 'G',
    type: LABEL_TYPES.POINT,
    colorHex: '#e74c3c',
    description: 'Notable gesture'
  },
  {
    id: 3,
    name: 'Writing',
    shortcut: 'W',
    type: LABEL_TYPES.RANGE,
    colorHex: '#f39c12',
    description: 'Writing on board'
  },
  {
    id: 4,
    name: 'Question',
    shortcut: 'Q',
    type: LABEL_TYPES.POINT,
    colorHex: '#27ae60',
    description: 'Student asking question'
  }
];

export const generateFakeAssignedLabels = (videoIds = [1, 2]) => [
  {
    id: 1,
    labelId: 1,
    labelName: 'Speaking',
    videoId: videoIds[0] || 1,
    startTime: 5.5,
    endTime: 12.3,
    colorHex: '#3498db',
    insDate: new Date().toISOString()
  },
  {
    id: 2,
    labelId: 2,
    labelName: 'Gesture',
    videoId: videoIds[0] || 1,
    startTime: 8.7,
    endTime: 8.7,
    colorHex: '#e74c3c',
    insDate: new Date().toISOString()
  },
  {
    id: 3,
    labelId: 3,
    labelName: 'Writing',
    videoId: videoIds[1] || 2,
    startTime: 15.2,
    endTime: 25.8,
    colorHex: '#f39c12',
    insDate: new Date().toISOString()
  }
];