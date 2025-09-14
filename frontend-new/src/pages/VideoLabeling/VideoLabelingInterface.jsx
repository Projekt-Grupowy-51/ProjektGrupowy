import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Container, Card, Alert } from '../../components/ui';
import { LoadingSpinner } from '../../components/common';
import { useVideoControls } from './hooks/useVideoControls.js';
import { useAssignedLabelsState } from './hooks/useAssignedLabelsState.js';
import { useBatchManagement } from './hooks/useBatchManagement.js';
import { useKeyboardShortcuts } from './hooks/useKeyboardShortcuts.js';
import { useRangeLabels } from './hooks/useRangeLabels.js';
import VideoGrid from './components/VideoGrid.jsx';
import MediaControls from './components/MediaControls.jsx';
import LabelingPanel from './components/LabelingPanel.jsx';
import LabelButtons from './components/LabelButtons.jsx';
import SubjectVideoGroupAssignmentService from '../../services/SubjectVideoGroupAssignmentService.js';
import SubjectService from '../../services/SubjectService.js';

const VideoLabelingInterface = () => {
  const { assignmentId } = useParams();
  const { t } = useTranslation(['videos', 'common']);
  
  const [assignment, setAssignment] = useState(null);
  const [labels, setLabels] = useState([]);
  const [assignmentLoading, setAssignmentLoading] = useState(false);
  const [labelsLoading, setLabelsLoading] = useState(false);
  const [assignmentError, setAssignmentError] = useState(null);
  const [labelsError, setLabelsError] = useState(null);

  useEffect(() => {
    if (!assignmentId) return;
    
    setAssignmentLoading(true);
    setAssignmentError(null);
    
    SubjectVideoGroupAssignmentService.getById(parseInt(assignmentId))
      .then(setAssignment)
      .catch(err => setAssignmentError(err.message))
      .finally(() => setAssignmentLoading(false));
  }, [assignmentId]);

  useEffect(() => {
    if (!assignment?.subjectId) return;
    
    setLabelsLoading(true);
    setLabelsError(null);
    
    SubjectService.getLabels(assignment.subjectId)
      .then(setLabels)
      .catch(err => setLabelsError(err.message))
      .finally(() => setLabelsLoading(false));
  }, [assignment?.subjectId]);

  // Use all hooks independently
  const videoControls = useVideoControls();
  const batchManagement = useBatchManagement(assignment);
  const assignedLabelsState = useAssignedLabelsState(batchManagement.videos, assignment);
  
  // Range labels logic
  const rangeLabels = useRangeLabels(
    labels,
    assignedLabelsState,
    videoControls.playerState.currentTime,
    assignment
  );

  // Keyboard shortcuts
  useKeyboardShortcuts(
    assignment,
    videoControls.playerState,
    labels,
    videoControls.handlePlayPause,
    videoControls.handleSeek,
    rangeLabels.handleLabelAction
  );

  const loading = assignmentLoading || labelsLoading || batchManagement.loading;
  const error = assignmentError || labelsError || batchManagement.error;

  useEffect(() => {
    videoControls.resetPlayerState();
    videoControls.syncVideos('reset');
  }, [batchManagement.currentBatch]);

  // Loading state
  if (loading) {
    return (
      <Container>
        <LoadingSpinner message={t('videos:labeling.loading_session')} />
      </Container>
    );
  }

  // Error state
  if (error) {
    return (
      <Container>
        <Alert variant="danger" className="mt-4">
          <div className="d-flex align-items-center">
            <i className="fas fa-exclamation-triangle me-3"></i>
            <div>
              <h5 className="alert-heading mb-1">{t('videos:labeling.error.failed_to_load_session')}</h5>
              <p className="mb-0">{error}</p>
            </div>
          </div>
        </Alert>
      </Container>
    );
  }

  // No assignment state
  if (!assignment) {
    return (
      <Container>
        <Alert variant="warning" className="mt-4">
          <div className="d-flex align-items-center">
            <i className="fas fa-exclamation-triangle me-3"></i>
            <div>
              <h5 className="alert-heading mb-1">{t('videos:labeling.error.assignment_not_found')}</h5>
              <p className="mb-0">{t('videos:labeling.error.assignment_not_found_message')}</p>
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
              {assignment.subjectName} - {assignment.videoGroupName}
            </h1>
          </div>
          <div className="d-flex align-items-center gap-3">
            <span className="badge bg-secondary">
              {t('videos:labeling.assignment')} #{assignment.id}
            </span>
            <span className="badge bg-secondary">
              {t('videos:labeling.batch')} {batchManagement.currentBatch}/{batchManagement.totalBatches}
            </span>
          </div>
        </div>
      </div>

      {/* Main Content Area */}
      <div className="row">
        {/* Left Column - Video Only */}
        <div className="col-lg-9 mb-3">
          <VideoGrid
            videos={batchManagement.videos}
            videoStreamUrls={batchManagement.videoStreamUrls}
            videoRefs={videoControls.videoRefs}
            onTimeUpdate={videoControls.handleTimeUpdate}
            onLoadedMetadata={videoControls.handleTimeUpdate}
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
                playerState={videoControls.playerState}
                batchInfo={batchManagement.getBatchInfo()}
                onPlayPause={videoControls.handlePlayPause}
                onSeek={videoControls.handleSeek}
                onSpeedChange={videoControls.handleSpeedChange}
                onBatchChange={batchManagement.loadBatch}
              />
            </Card.Body>
          </Card>

          {/* Available Labels */}
          <Card className="mb-3">
            <Card.Header>
              <Card.Title level={6}>
                <i className="fas fa-tags me-2"></i>
                {t('videos:labeling.available_labels')}
              </Card.Title>
            </Card.Header>
            <Card.Body>
              <LabelButtons
                labels={labels}
                onLabelAction={rangeLabels.handleLabelAction}
                getLabelState={rangeLabels.getLabelState}
              />
            </Card.Body>
          </Card>

          <LabelingPanel
            labels={labels}
            assignedLabels={assignedLabelsState.assignedLabels}
            onDeleteLabel={assignedLabelsState.handleDeleteLabel}
            loading={assignedLabelsState.assignedLabelsLoading}
            operationLoading={assignedLabelsState.labelOperationLoading}
          />
        </div>
      </div>
    </Container>
  );
};

export default VideoLabelingInterface;