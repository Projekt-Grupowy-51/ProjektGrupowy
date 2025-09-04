import React, { useEffect, useCallback } from 'react';
import { useParams } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Container, Card, Alert } from '../../components/ui';
import { LoadingSpinner } from '../../components/common';
import { useDataFetching } from '../../hooks/common';
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
  
  // Fetch assignment and labels directly
  const fetchAssignment = useCallback(() => {
    return assignmentId ? SubjectVideoGroupAssignmentService.getById(parseInt(assignmentId)) : Promise.resolve(null);
  }, [assignmentId]);

  const { data: assignment, loading: assignmentLoading, error: assignmentError } = useDataFetching(
    fetchAssignment,
    [assignmentId]
  );

  const fetchLabels = useCallback(() => {
    return assignment?.subjectId ? SubjectService.getLabels(assignment.subjectId) : Promise.resolve([]);
  }, [assignment?.subjectId]);

  const { data: labels, loading: labelsLoading, error: labelsError } = useDataFetching(
    fetchLabels,
    [assignment?.subjectId]
  );

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
        <LoadingSpinner message={t('videos:loading_session')} />
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
              <h5 className="alert-heading mb-1">Failed to Load Session</h5>
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
              {assignment.subjectName} - {assignment.videoGroupName}
            </h1>
          </div>
          <div className="d-flex align-items-center gap-3">
            <span className="badge bg-secondary">
              Assignment #{assignment.id}
            </span>
            <span className="badge bg-secondary">
              Batch {batchManagement.currentBatch}/{batchManagement.totalBatches}
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
                Available Labels
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