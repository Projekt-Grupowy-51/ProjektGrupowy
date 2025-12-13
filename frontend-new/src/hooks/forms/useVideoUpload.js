import { useState } from 'react';
import { useTranslation } from 'react-i18next';

/**
 * Custom hook for video upload functionality
 * Handles file validation, drag & drop, and video list management
 */
export const useVideoUpload = () => {
  const { t } = useTranslation(['videos', 'common']);
  const [videos, setVideos] = useState([]);
  const [error, setError] = useState('');

  /**
   * Validate a single file
   */
  const validateFile = (file) => {
    if (!file.type.startsWith('video/')) {
      return t('upload.error_not_video', { filename: file.name });
    }
    if (file.size > 100 * 1024 * 1024) {
      return t('upload.error_size', { filename: file.name });
    }
    return null;
  };

  /**
   * Process multiple files and add valid ones to videos list
   */
  const processFiles = (files) => {
    const accepted = [];
    const rejected = [];

    Array.from(files).forEach((file) => {
      const validationError = validateFile(file);
      if (validationError) {
        rejected.push(validationError);
      } else {
        accepted.push({
          file,
          title: file.name.replace(/\.[^/.]+$/, ''),
          positionInQueue: videos.length + accepted.length + 1,
        });
      }
    });

    setVideos((prev) => [...prev, ...accepted]);
    if (rejected.length > 0) {
      setError(rejected.join('\n'));
    } else {
      setError('');
    }

    return {
      accepted: accepted.length,
      rejected: rejected.length
    };
  };

  /**
   * Handle file input change
   */
  const handleFileSelect = (e) => {
    return processFiles(e.target.files);
  };

  /**
   * Handle drag and drop
   */
  const handleDrop = (e) => {
    e.preventDefault();
    return processFiles(e.dataTransfer.files);
  };

  /**
   * Handle drag over (prevent default to allow drop)
   */
  const handleDragOver = (e) => {
    e.preventDefault();
  };

  /**
   * Update video properties (title, position, etc.)
   */
  const updateVideo = (index, field, value) => {
    setVideos((prev) => {
      const updated = [...prev];
      updated[index][field] = field === 'positionInQueue' ? parseInt(value) : value;
      return updated;
    });
  };

  /**
   * Remove video from the list
   */
  const removeVideo = (indexToRemove) => {
    setVideos((prev) => prev.filter((_, i) => i !== indexToRemove));
  };

  /**
   * Clear all videos
   */
  const clearVideos = () => {
    setVideos([]);
    setError('');
  };

  /**
   * Set error message
   */
  const setErrorMessage = (message) => {
    setError(message);
  };

  /**
   * Clear error
   */
  const clearError = () => {
    setError('');
  };

  /**
   * Validate that at least one video is selected
   */
  const validateVideos = () => {
    if (videos.length === 0) {
      const errorMsg = t('upload.error_empty');
      setError(errorMsg);
      return { isValid: false, error: errorMsg };
    }
    setError('');
    return { isValid: true, error: null };
  };

  return {
    // State
    videos,
    error,
    hasVideos: videos.length > 0,
    videoCount: videos.length,
    
    // File handling
    handleFileSelect,
    handleDrop,
    handleDragOver,
    processFiles,
    
    // Video management
    updateVideo,
    removeVideo,
    clearVideos,
    
    // Validation
    validateVideos,
    validateFile,
    
    // Error handling
    setErrorMessage,
    clearError
  };
};