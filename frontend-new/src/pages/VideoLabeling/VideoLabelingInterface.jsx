import React, { useState, useEffect, useRef } from "react";
import { useParams } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { Container, Card, Alert } from "../../components/ui";
import { LoadingSpinner } from "../../components/common";
import { useVideoControls } from "./hooks/useVideoControls.js";
import { useAssignedLabelsState } from "./hooks/useAssignedLabelsState.js";
import { useBatchManagement } from "./hooks/useBatchManagement.js";
import { useKeyboardShortcuts } from "./hooks/useKeyboardShortcuts.js";
import { useRangeLabels } from "./hooks/useRangeLabels.js";
import VideoGrid from "./components/VideoGrid.jsx";
import MediaControls from "./components/MediaControls.jsx";
import LabelingPanel from "./components/LabelingPanel.jsx";
import LabelButtons from "./components/LabelButtons.jsx";
import QualitySelector from "./components/QualitySelector.jsx";
import SubjectVideoGroupAssignmentService from "../../services/SubjectVideoGroupAssignmentService.js";
import SubjectService from "../../services/SubjectService.js";

const VideoLabelingInterface = () => {
  const { assignmentId } = useParams();
  const { t } = useTranslation(["videos", "common"]);

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
      .catch((err) => setAssignmentError(err.message))
      .finally(() => setAssignmentLoading(false));
  }, [assignmentId]);

  useEffect(() => {
    if (!assignment?.subjectId) return;

    setLabelsLoading(true);
    setLabelsError(null);

    SubjectService.getLabels(assignment.subjectId)
      .then(setLabels)
      .catch((err) => setLabelsError(err.message))
      .finally(() => setLabelsLoading(false));
  }, [assignment?.subjectId]);

  // Use all hooks independently
  const videoControls = useVideoControls();
  const batchManagement = useBatchManagement(assignment);
  const assignedLabelsState = useAssignedLabelsState(
    batchManagement.videos,
    assignment
  );

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

  // Store playback state reference for quality changes
  const playbackStateRef = useRef({
    wasPlaying: false,
    currentTime: 0,
    playbackSpeed: 1.0,
    shouldRestore: false,
  });

  // Handle quality change with playback state preservation
  const handleQualityChange = async (quality) => {
    // Capture current playback state
    playbackStateRef.current = {
      wasPlaying: videoControls.playerState.isPlaying,
      currentTime: videoControls.playerState.currentTime,
      playbackSpeed: videoControls.playerState.playbackSpeed,
      shouldRestore: true,
    };

    // Pause videos before quality change
    if (videoControls.playerState.isPlaying) {
      videoControls.handlePlayPause();
    }

    // Change quality (this will reload videos)
    await batchManagement.handleQualityChange(quality);
  };

  // Restore playback state when videos are reloaded after quality change
  const handleVideoLoaded = () => {
    if (playbackStateRef.current.shouldRestore) {
      const { currentTime, playbackSpeed, wasPlaying } =
        playbackStateRef.current;

      // Use setTimeout to ensure video metadata is fully loaded
      setTimeout(() => {
        // Restore time position
        videoControls.handleSeek(currentTime);

        // Restore playback speed
        if (playbackSpeed !== 1.0) {
          videoControls.handleSpeedChange(playbackSpeed);
        }

        // Resume playback if it was playing
        if (wasPlaying) {
          setTimeout(() => {
            videoControls.handlePlayPause();
          }, 100);
        }

        // Reset flag
        playbackStateRef.current.shouldRestore = false;
      }, 200);
    }
  };

  useEffect(() => {
    videoControls.resetPlayerState();
    videoControls.syncVideos("reset");
  }, [batchManagement.currentBatch]);

  // Loading state
  if (loading) {
    return (
      <Container>
        <LoadingSpinner message={t("videos:labeling.loading_session")} />
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
              <h5 className="alert-heading mb-1">
                {t("videos:labeling.error.failed_to_load_session")}
              </h5>
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
              <h5 className="alert-heading mb-1">
                {t("videos:labeling.error.assignment_not_found")}
              </h5>
              <p className="mb-0">
                {t("videos:labeling.error.assignment_not_found_message")}
              </p>
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
            <span className="badge bg-primary">
              {t("videos:labeling.assignment")} #{assignment.id}
            </span>
            <span className="badge bg-primary">
              {t("videos:labeling.batch")} {batchManagement.currentBatch}/
              {batchManagement.totalBatches}
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
            onLoadedMetadata={() => {
              videoControls.handleTimeUpdate();
              handleVideoLoaded();
            }}
            videoHeight={300}
            fillSpace={true}
            displayMode={"auto"}
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

              {/* Quality Selector */}
              <QualitySelector
                availableQualities={batchManagement.availableQualities}
                originalQuality={batchManagement.originalQuality}
                selectedQualityIndex={batchManagement.selectedQualityIndex}
                onQualityChange={handleQualityChange}
              />
            </Card.Body>
          </Card>

          {/* Available Labels */}
          <Card className="mb-3">
            <Card.Header>
              <Card.Title level={6}>
                <i className="fas fa-tags me-2"></i>
                {t("videos:labeling.available_labels")}
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
            onTimeClick={videoControls.handleSeek}
            loading={assignedLabelsState.assignedLabelsLoading}
            operationLoading={assignedLabelsState.labelOperationLoading}
            currentPage={assignedLabelsState.currentPage}
            pageSize={assignedLabelsState.pageSize}
            totalPages={assignedLabelsState.totalPages}
            totalItems={assignedLabelsState.totalItems}
            onPageChange={assignedLabelsState.handlePageChange}
            onPageSizeChange={assignedLabelsState.handlePageSizeChange}
          />
        </div>
      </div>
    </Container>
  );
};

export default VideoLabelingInterface;
