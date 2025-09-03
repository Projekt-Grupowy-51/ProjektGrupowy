import { useState, useEffect, useRef, useCallback } from 'react';
import { generateFakeVideoStreams, validateVideoTime } from '../utils/videoUtils.js';
import { generateFakeLabels, generateFakeAssignedLabels, LABEL_TYPES } from '../utils/labelUtils.js';
import { formatTime } from '../utils/timeUtils.js';

const INITIAL_STATE = {
  // Assignment data
  assignment: null,
  videoGroup: null,
  
  // Video data
  currentBatch: 1,
  totalBatches: 3,
  videos: [],
  
  // Labels
  labels: [],
  assignedLabels: [],
  labelingState: {}, // Track range label states
  
  // Player state
  playerState: {
    isPlaying: false,
    currentTime: 0,
    duration: 0,
    playbackSpeed: 1.0
  },
  
  // UI state
  loading: true,
  error: null
};

const useVideoLabelingSession = (assignmentId) => {
  const [state, setState] = useState(INITIAL_STATE);
  const videoRefs = useRef([]);
  const keyboardHandlerRef = useRef(null);

  const initializeSession = useCallback(async () => {
    try {
      setState(prev => ({ ...prev, loading: true, error: null }));
      
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      const fakeVideos = generateFakeVideoStreams(4);
      const fakeLabels = generateFakeLabels();
      const fakeAssignedLabels = generateFakeAssignedLabels(fakeVideos.map(v => v.id));
      
      setState(prev => ({
        ...prev,
        assignment: {
          id: parseInt(assignmentId),
          subjectId: 1,
          videoGroupId: 1,
          subjectName: 'Mathematics',
          videoGroupName: 'Algebra Videos'
        },
        videoGroup: {
          id: 1,
          name: 'Algebra Videos Batch 1',
          description: 'Introduction to algebraic concepts',
          projectId: 1
        },
        videos: fakeVideos,
        labels: fakeLabels,
        assignedLabels: fakeAssignedLabels,
        loading: false
      }));
      
    } catch (error) {
      console.error('Error initializing session:', error);
      setState(prev => ({
        ...prev,
        error: 'Failed to load labeling session',
        loading: false
      }));
    }
  }, [assignmentId]);

  // Video synchronization utilities
  const syncVideos = useCallback((action, value = null) => {
    if (!videoRefs.current) return;
    
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
      } catch (error) {
        console.error(`Error during video sync action ${action}:`, error);
      }
    });
  }, []);

  // Media control actions
  const handlePlayPause = useCallback(() => {
    const newIsPlaying = !state.playerState.isPlaying;
    syncVideos(newIsPlaying ? 'play' : 'pause');
    
    setState(prev => ({
      ...prev,
      playerState: {
        ...prev.playerState,
        isPlaying: newIsPlaying
      }
    }));
  }, [state.playerState.isPlaying, syncVideos]);

  const handleSeek = useCallback((newTime) => {
    const validTime = validateVideoTime(newTime, state.playerState.duration);
    syncVideos('setTime', validTime);
    
    setState(prev => ({
      ...prev,
      playerState: {
        ...prev.playerState,
        currentTime: validTime
      }
    }));
  }, [state.playerState.duration, syncVideos]);

  const handleSpeedChange = useCallback((speed) => {
    syncVideos('setSpeed', speed);
    
    setState(prev => ({
      ...prev,
      playerState: {
        ...prev.playerState,
        playbackSpeed: speed
      }
    }));
  }, [syncVideos]);

  const handleBatchChange = useCallback(async (newBatch) => {
    if (newBatch < 1 || newBatch > state.totalBatches) return;
    
    try {
      setState(prev => ({ ...prev, loading: true }));
      syncVideos('reset');
      
      await new Promise(resolve => setTimeout(resolve, 500));
      
      setState(prev => ({
        ...prev,
        currentBatch: newBatch,
        videos: generateFakeVideoStreams(4),
        playerState: {
          ...prev.playerState,
          isPlaying: false,
          currentTime: 0,
          duration: 0,
          playbackSpeed: 1.0
        },
        loading: false
      }));
      
    } catch (error) {
      console.error('Error changing batch:', error);
      setState(prev => ({
        ...prev,
        error: 'Failed to load batch',
        loading: false
      }));
    }
  }, [state.totalBatches, syncVideos]);

  const handleLabelAction = useCallback((labelId) => {
    const label = state.labels.find(l => l.id === labelId);
    if (!label) return;
    
    const currentTime = state.playerState.currentTime;
    const currentLabelState = state.labelingState[labelId] || {};
    
    const createLabel = (startTime, endTime) => ({
      id: Date.now(),
      labelId,
      labelName: label.name,
      videoId: state.videos[0]?.id || 1,
      startTime,
      endTime,
      colorHex: label.colorHex,
      insDate: new Date().toISOString()
    });
    
    if (label.type === LABEL_TYPES.POINT) {
      setState(prev => ({
        ...prev,
        assignedLabels: [...prev.assignedLabels, createLabel(currentTime, currentTime)]
      }));
    } else if (label.type === LABEL_TYPES.RANGE) {
      if (currentLabelState.isActive) {
        setState(prev => ({
          ...prev,
          assignedLabels: [...prev.assignedLabels, createLabel(currentLabelState.startTime, currentTime)],
          labelingState: {
            ...prev.labelingState,
            [labelId]: { isActive: false }
          }
        }));
      } else {
        setState(prev => ({
          ...prev,
          labelingState: {
            ...prev.labelingState,
            [labelId]: { isActive: true, startTime: currentTime }
          }
        }));
      }
    }
  }, [state.labels, state.playerState.currentTime, state.labelingState, state.videos]);

  const handleDeleteLabel = useCallback((assignedLabelId) => {
    setState(prev => ({
      ...prev,
      assignedLabels: prev.assignedLabels.filter(l => l.id !== assignedLabelId)
    }));
  }, []);

  // Time update handler
  const handleTimeUpdate = useCallback(() => {
    const video = videoRefs.current[0];
    if (!video) return;
    
    setState(prev => ({
      ...prev,
      playerState: {
        ...prev.playerState,
        currentTime: video.currentTime,
        duration: video.duration || prev.playerState.duration
      }
    }));
  }, []);

  // Keyboard shortcuts
  const handleKeyPress = useCallback((event) => {
    if (!state.assignment) return;
    
    const { key } = event;
    
    switch (key) {
      case ' ':
        event.preventDefault();
        handlePlayPause();
        break;
      case 'ArrowLeft':
        event.preventDefault();
        handleSeek(state.playerState.currentTime - 1);
        break;
      case 'ArrowRight':
        event.preventDefault();
        handleSeek(state.playerState.currentTime + 1);
        break;
      default:
        // Check for label shortcuts
        const label = state.labels.find(l => l.shortcut?.toLowerCase() === key.toLowerCase());
        if (label) {
          event.preventDefault();
          handleLabelAction(label.id);
        }
        break;
    }
  }, [state.assignment, state.playerState.currentTime, state.labels, handlePlayPause, handleSeek, handleLabelAction]);

  useEffect(() => {
    keyboardHandlerRef.current = handleKeyPress;
    
    const keyHandler = (e) => keyboardHandlerRef.current?.(e);
    
    window.addEventListener('keydown', keyHandler);
    return () => window.removeEventListener('keydown', keyHandler);
  }, [handleKeyPress]);

  useEffect(() => {
    if (assignmentId) {
      initializeSession();
    }
  }, [assignmentId, initializeSession]);

  return {
    // State
    ...state,
    
    // Refs
    videoRefs,
    
    // Actions
    handlePlayPause,
    handleSeek,
    handleSpeedChange,
    handleBatchChange,
    handleLabelAction,
    handleDeleteLabel,
    handleTimeUpdate,
    
    // Utils
    getLabelState: (labelId) => state.labelingState[labelId] || { isActive: false },
    getBatchInfo: () => ({
      current: state.currentBatch,
      total: state.totalBatches,
      canGoPrevious: state.currentBatch > 1,
      canGoNext: state.currentBatch < state.totalBatches
    })
  };
};

export default useVideoLabelingSession;