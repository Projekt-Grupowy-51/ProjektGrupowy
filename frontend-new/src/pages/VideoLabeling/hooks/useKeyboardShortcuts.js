import { useEffect, useRef } from 'react';

export const useKeyboardShortcuts = (assignment, playerState, labels, handlePlayPause, handleSeek, handleLabelAction) => {
  const keyboardHandlerRef = useRef(null);

  const handleKeyPress = (event) => {
    if (!assignment) return;
    
    const { key } = event;
    
    switch (key) {
      case ' ':
        event.preventDefault();
        handlePlayPause();
        break;
      case 'ArrowLeft':
        event.preventDefault();
        handleSeek(playerState.currentTime - 1);
        break;
      case 'ArrowRight':
        event.preventDefault();
        handleSeek(playerState.currentTime + 1);
        break;
      default:
        const label = labels?.find(l => l.shortcut?.toLowerCase() === key.toLowerCase());
        if (label) {
          event.preventDefault();
          handleLabelAction(label.id);
        }
        break;
    }
  };

  useEffect(() => {
    keyboardHandlerRef.current = handleKeyPress;
    
    const keyHandler = (e) => keyboardHandlerRef.current?.(e);
    
    window.addEventListener('keydown', keyHandler);
    return () => window.removeEventListener('keydown', keyHandler);
  });

  return {};
};