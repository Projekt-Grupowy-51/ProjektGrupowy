import React from 'react';
import { useParams } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Container, Card, Alert } from '../../components/ui';
import useVideoLabelingSession from './hooks/useVideoLabelingSession.js';
import VideoGrid from './components/VideoGrid.jsx';
import MediaControls from './components/MediaControls.jsx';
import LabelingPanel from './components/LabelingPanel.jsx';

const VideoLabelingInterface = () => {
  const { assignmentId } = useParams();
  const { t } = useTranslation(['videos', 'common']);
  
  const session = useVideoLabelingSession(assignmentId);

  // Loading state
  if (session.loading) {
    return (
      <Container>
        <div className="d-flex justify-content-center py-5">
          <div className="text-center">
            <div className="spinner-border text-primary mb-3" role="status">
              <span className="visually-hidden">{t('common:loading')}</span>
            </div>
            <p className="text-muted">{t('videos:loading_session')}</p>
          </div>
        </div>
      </Container>
    );
  }

  // Error state
  if (session.error) {
    return (
      <Container>
        <Alert variant="danger" className="mt-4">
          <div className="d-flex align-items-center">
            <i className="fas fa-exclamation-triangle me-3"></i>
            <div>
              <h5 className="alert-heading mb-1">Failed to Load Session</h5>
              <p className="mb-0">{session.error}</p>
            </div>
          </div>
        </Alert>
      </Container>
    );
  }

  // No assignment state
  if (!session.assignment) {
    return (
      <Container>
        <Alert variant="warning" className="mt-4">
          <div className="d-flex align-items-center">
            <i className="fas fa-exclamation-triangle me-3"></i>
            <div>
              <h5 className="alert-heading mb-1">Assignment Not Found</h5>
              <p className="mb-0">The requested labeling assignment could not be found.</p>
            </div>
          </div>
        </Alert>
      </Container>
    );
  }

  return (
    <Container fluid>
      {/* Header */}
      <div className="mb-4">
        <div className="d-flex justify-content-between align-items-center">
          <div>
            <h1 className="h4 mb-1">
              <i className="fas fa-video me-2"></i>
              {session.assignment.subjectName} - {session.assignment.videoGroupName}
            </h1>
          </div>
          <div className="d-flex align-items-center gap-3">
            <span className="badge bg-secondary">
              Assignment #{session.assignment.id}
            </span>
            <span className="badge bg-secondary">
              Batch {session.currentBatch}/{session.totalBatches}
            </span>
          </div>
        </div>
      </div>

      {/* Main Content Area */}
      <div className="row">
        {/* Left Column - Video Only */}
        <div className="col-lg-9 mb-3">
          <VideoGrid
            videos={session.videos}
            videoRefs={session.videoRefs}
            onTimeUpdate={session.handleTimeUpdate}
            onLoadedMetadata={session.handleTimeUpdate}
            videoHeight={300}
            fillSpace={true}
            displayMode={'auto'}
          />
        </div>

        {/* Right Column - Controls & Labels Panel */}
        <div className="col-lg-3">
          {/* Media Controls */}
          <Card className="mb-3">
            <Card.Body>
              <MediaControls
                playerState={session.playerState}
                batchInfo={session.getBatchInfo()}
                onPlayPause={session.handlePlayPause}
                onSeek={session.handleSeek}
                onSpeedChange={session.handleSpeedChange}
                onBatchChange={session.handleBatchChange}
              />
            </Card.Body>
          </Card>

          {/* Available Labels */}
          <Card className="mb-3">
            <Card.Header>
              <Card.Title level={6}>
                <i className="fas fa-tags me-2"></i>
                Available Labels
              </Card.Title>
            </Card.Header>
            <Card.Body>
              <div className="d-flex flex-wrap gap-1">
                {session.labels.map((label) => {
                  const labelState = session.labelingState[label.id] || { isActive: false };
                  const buttonText = `${label.name} [${label.shortcut}]`;
                  const tooltipText = `Shortcut: ${label.shortcut} | ${label.description}`;
                  
                  return (
                    <button
                      key={label.id}
                      className={`btn btn-sm ${labelState.isActive ? 'btn-warning' : 'btn-outline-primary'}`}
                      onClick={() => session.handleLabelAction(label.id)}
                      title={tooltipText}
                      style={{
                        backgroundColor: labelState.isActive ? label.colorHex : 'transparent',
                        borderColor: label.colorHex,
                        color: labelState.isActive ? '#fff' : label.colorHex,
                        fontSize: '0.75rem'
                      }}
                    >
                      {buttonText}
                    </button>
                  );
                })}
              </div>
            </Card.Body>
          </Card>

          <LabelingPanel
            labels={session.labels}
            assignedLabels={session.assignedLabels}
            onDeleteLabel={session.handleDeleteLabel}
          />
        </div>
      </div>
    </Container>
  );
};

export default VideoLabelingInterface;