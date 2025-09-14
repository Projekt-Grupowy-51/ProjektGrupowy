import React from 'react';
import PropTypes from 'prop-types';
import { useTranslation } from 'react-i18next';
import { Button } from '../../../components/ui';
import { formatDuration, getProgressPercentage } from '../utils/timeUtils.js';

const PLAYBACK_SPEEDS = [0.25, 0.5, 0.75, 1.0, 1.25, 1.5, 1.75, 2.0];

const MediaControls = ({ 
  playerState, 
  batchInfo, 
  onPlayPause, 
  onSeek, 
  onSpeedChange, 
  onBatchChange 
}) => {
  const { t } = useTranslation('videos');
  const { isPlaying, currentTime, duration, playbackSpeed } = playerState;
  const { current, total, canGoPrevious, canGoNext } = batchInfo;

  const handleSeekBarChange = (e) => {
    onSeek(parseFloat(e.target.value));
  };

  const handleQuickSeek = (seconds) => {
    onSeek(Math.max(0, Math.min(currentTime + seconds, duration || 0)));
  };

  const progressPercentage = getProgressPercentage(currentTime, duration);

  return (
    <div className="media-controls p-2">
      {/* Progress Bar Row */}
      <div className="mb-2">
        <div className="d-flex align-items-center gap-2">
          <span className="small text-dark fw-bold" style={{ fontSize: '0.75rem', minWidth: '45px' }}>
            {formatDuration(currentTime)}
          </span>
          <div className="flex-grow-1 position-relative">
            <input
              type="range"
              className="form-range w-100"
              min="0"
              max={duration || 100}
              value={currentTime}
              step="0.01"
              onChange={handleSeekBarChange}
              style={{ 
                height: '8px', 
                cursor: 'pointer',
                background: 'linear-gradient(to right, #0d6efd 0%, #0d6efd ' + progressPercentage + '%, #e9ecef ' + progressPercentage + '%, #e9ecef 100%)'
              }}
            />
          </div>
          <span className="small text-dark fw-bold" style={{ fontSize: '0.75rem', minWidth: '45px' }}>
            {formatDuration(duration || 0)}
          </span>
        </div>
      </div>
      
      {/* Controls Row */}
      <div className="d-flex justify-content-between align-items-center gap-1">
        {/* Batch Navigation */}
        <div className="d-flex align-items-center gap-1">
          <Button
            variant="outline"
            size="sm"
            onClick={() => onBatchChange(current - 1)}
            disabled={!canGoPrevious}
            style={{ fontSize: '0.7rem', padding: '4px 6px', minWidth: '24px' }}
          >
            ‹
          </Button>
          <span className="badge bg-primary" style={{ fontSize: '0.7rem', padding: '2px 4px' }}>
            {current}/{total}
          </span>
          <Button
            variant="outline"
            size="sm"
            onClick={() => onBatchChange(current + 1)}
            disabled={!canGoNext}
            style={{ fontSize: '0.7rem', padding: '4px 6px', minWidth: '24px' }}
          >
            ›
          </Button>
        </div>

        {/* Video Controls */}
        <div className="d-flex align-items-center gap-1">
          <Button
            variant="outline-secondary"
            size="sm"
            onClick={() => handleQuickSeek(-10)}
            disabled={currentTime <= 0}
            title={t('videos:labeling.seek_backward_10s')}
            style={{ fontSize: '0.7rem', padding: '4px 6px', minWidth: '28px' }}
          >
            ‹‹
          </Button>

          <Button
            variant={isPlaying ? "warning" : "success"}
            size="sm"
            onClick={() => onPlayPause()}
            title={t('videos:labeling.play_pause_shortcut')}
            style={{ fontSize: '0.9rem', padding: '6px 8px', minWidth: '32px', fontWeight: 'bold' }}
          >
            {isPlaying ? '⏸' : '▶'}
          </Button>
          
          <Button
            variant="outline-secondary"
            size="sm"
            onClick={() => handleQuickSeek(10)}
            disabled={currentTime >= (duration || 0)}
            title={t('videos:labeling.seek_forward_10s')}
            style={{ fontSize: '0.7rem', padding: '4px 6px', minWidth: '28px' }}
          >
            ››
          </Button>
        </div>

        {/* Playback Speed */}
        <div className="d-flex align-items-center">
          <select
            className="form-select form-select-sm"
            style={{ width: '50px', fontSize: '0.7rem', padding: '4px' }}
            value={playbackSpeed}
            onChange={(e) => onSpeedChange(parseFloat(e.target.value))}
            title={t('videos:labeling.playback_speed')}
          >
            {PLAYBACK_SPEEDS.map((speed) => (
              <option key={speed} value={speed}>
                {speed}x
              </option>
            ))}
          </select>
        </div>
      </div>
    </div>
  );
};

MediaControls.propTypes = {
  playerState: PropTypes.shape({
    isPlaying: PropTypes.bool.isRequired,
    currentTime: PropTypes.number.isRequired,
    duration: PropTypes.number.isRequired,
    playbackSpeed: PropTypes.number.isRequired
  }).isRequired,
  batchInfo: PropTypes.shape({
    current: PropTypes.number.isRequired,
    total: PropTypes.number.isRequired,
    canGoPrevious: PropTypes.bool.isRequired,
    canGoNext: PropTypes.bool.isRequired
  }).isRequired,
  onPlayPause: PropTypes.func.isRequired,
  onSeek: PropTypes.func.isRequired,
  onSpeedChange: PropTypes.func.isRequired,
  onBatchChange: PropTypes.func.isRequired
};

export default MediaControls;