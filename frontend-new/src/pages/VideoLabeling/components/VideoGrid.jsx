import React, { useEffect, useState } from 'react';
import PropTypes from 'prop-types';
import { useTranslation } from 'react-i18next';
import { getVideoGridLayout } from '../utils/videoUtils.js';

const VideoGrid = ({ videos, videoStreamUrls = {}, videoRefs, onTimeUpdate, onLoadedMetadata = () => {}, videoHeight = 300, fillSpace = false, displayMode = 'auto' }) => {
  const { t } = useTranslation(['videos']);
  const [endedVideos, setEndedVideos] = useState(new Set());
  
  const getGridConfig = (mode, videoCount) => {
    const configs = {
      '1': { layout: 'single-video', videos: 1 },
      '2': { layout: 'two-videos', videos: 2 },
      '4': { layout: 'four-videos', videos: 4 },
      'auto': { layout: getVideoGridLayout(videoCount), videos: videoCount }
    };
    return configs[mode] || configs['auto'];
  };
  
  const { layout: gridLayout, videos: maxVideos } = getGridConfig(displayMode, videos.length);
  const videosToShow = videos.slice(0, maxVideos);
  

  const handleVideoEnded = (index) => {
    setEndedVideos(prev => new Set(prev).add(index));
  };

  useEffect(() => {
    setEndedVideos(new Set());
  }, [videos, displayMode]);

  const getGridStyles = (videoHeight) => {
    const baseStyles = {
      display: 'grid',
      gap: '6px',
      width: '100%',
      height: fillSpace ? '100%' : 'auto'
    };

    const gridConfigs = {
      'single-video': {
        columns: '1fr',
        rows: fillSpace ? '1fr' : `${videoHeight * 1.3}px`
      },
      'two-videos': {
        columns: '1fr 1fr',
        rows: fillSpace ? '1fr 1fr' : `${(videoHeight * 1.3) / 2}px ${(videoHeight * 1.3) / 2}px`
      },
      'four-videos': {
        columns: '1fr 1fr',
        rows: fillSpace ? '1fr 1fr' : `${(videoHeight * 1.3) / 2}px ${(videoHeight * 1.3) / 2}px`
      }
    };

    const config = gridConfigs[gridLayout];
    if (config) {
      return {
        ...baseStyles,
        gridTemplateColumns: config.columns,
        gridTemplateRows: config.rows
      };
    }

    return {
      ...baseStyles,
      gridTemplateColumns: 'repeat(auto-fit, minmax(300px, 1fr))',
      gridTemplateRows: fillSpace ? '1fr' : `repeat(auto-fit, ${videoHeight * 1.3}px)`
    };
  };

  const getVideoStyles = () => ({
    width: '100%',
    height: '100%',
    objectFit: 'cover',
    backgroundColor: '#000',
    borderRadius: '4px'
  });

  if (!videosToShow || videosToShow.length === 0) {
    return (
      <div className="d-flex justify-content-center align-items-center" style={{ height: '400px' }}>
        <div className="text-center">
          <i className="fas fa-film fs-1 text-muted opacity-50"></i>
          <p className="text-muted mt-3">{t('videos:video_group_details.no_videos')}</p>
        </div>
      </div>
    );
  }

  const gridStyles = getGridStyles(videoHeight);
  
  return (
    <div className={`video-grid-container ${fillSpace ? 'h-100' : ''}`}>
      <div style={gridStyles}>
        {videosToShow.map((video, index) => (
          <div key={video.id || index} className="video-container">
            <div className="position-relative h-100">
              <video
                ref={(el) => {
                  if (videoRefs?.current) {
                    videoRefs.current[index] = el;
                  }
                }}
                style={getVideoStyles()}
                onTimeUpdate={onTimeUpdate}
                onLoadedMetadata={onLoadedMetadata}
                onEnded={() => handleVideoEnded(index)}
                preload="metadata"
                src={videoStreamUrls[video.id] || video.url}
              >
                Your browser does not support the video tag.
              </video>
              
              {/* Video title overlay */}
              {video.title && (
                <div 
                  className="position-absolute top-0 start-0 bg-dark bg-opacity-75 text-white px-2 py-1"
                  style={{ fontSize: '0.8rem', borderRadius: '0 0 4px 0' }}
                >
                  {video.title}
                </div>
              )}
              
              {/* Video ended indicator */}
              {endedVideos.has(index) && (
                <div className="position-absolute top-50 start-50 translate-middle">
                  <div className="bg-dark bg-opacity-75 text-white px-3 py-2 rounded">
                    <i className="fas fa-check-circle me-2"></i>
                    Video Ended
                  </div>
                </div>
              )}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

VideoGrid.propTypes = {
  videos: PropTypes.arrayOf(PropTypes.shape({
    id: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
    url: PropTypes.string,
    title: PropTypes.string,
    duration: PropTypes.number
  })).isRequired,
  videoStreamUrls: PropTypes.object,
  videoRefs: PropTypes.object.isRequired,
  onTimeUpdate: PropTypes.func.isRequired,
  onLoadedMetadata: PropTypes.func,
  videoHeight: PropTypes.number,
  fillSpace: PropTypes.bool,
  displayMode: PropTypes.oneOf(['auto', '1', '2', '4'])
};


export default VideoGrid;