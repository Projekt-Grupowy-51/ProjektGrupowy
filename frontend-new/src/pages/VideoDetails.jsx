import React from 'react';
import { useTranslation } from 'react-i18next';
import { Container, Card, Button, Table, Alert } from '../components/ui';
import { LoadingSpinner, ErrorAlert } from '../components/common';
import { useVideoDetails } from '../hooks/useVideoDetails.js';

const VideoDetails = () => {
  const { t } = useTranslation(['videos', 'common']);
  const {
    video,
    videoLoading,
    videoError,
    assignedLabels,
    labelsLoading,
    labelsError,
    handleBackToVideoGroup,
    formatDuration
  } = useVideoDetails();

  if (videoLoading) {
    return (
      <Container>
        <LoadingSpinner message={t('common:states.loading')} />
      </Container>
    );
  }

  if (videoError) {
    return (
      <Container>
        <ErrorAlert error={videoError} />
      </Container>
    );
  }

  if (!video) {
    return (
      <Container>
        <ErrorAlert error={t('videos:messages.not_found')} />
      </Container>
    );
  }

  return (
    <Container className="py-4">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h1>
            <i className="fas fa-video me-2"></i>
            {video.title}
          </h1>
          <p className="text-muted mb-0">
            {t('videos:details.duration')}: {formatDuration(video.duration || 0)} • 
            {t('videos:details.filename')}: {video.filename}
          </p>
        </div>
        <Button
          variant="outline-secondary"
          icon="fas fa-arrow-left"
          onClick={handleBackToVideoGroup}
        >
          {t('common:buttons.back')}
        </Button>
      </div>

      <Card className="mb-4">
        <Card.Header>
          <Card.Title level={5}>
            <i className="fas fa-play-circle me-2"></i>
            {t('videos:details.video_player')}
          </Card.Title>
        </Card.Header>
        <Card.Body>
          <div
            className="video-container"
            style={{ 
              position: "relative", 
              paddingTop: "56.25%",
              backgroundColor: "#000",
              borderRadius: "8px",
              overflow: "hidden"
            }}
          >
            <video
              style={{
                position: "absolute",
                top: 0,
                left: 0,
                width: "100%",
                height: "100%",
                objectFit: "contain",
              }}
              controls
              src={`/api/Video/${video.id}/stream`}
              type="video/mp4"
            >
              {t('videos:details.video_not_supported')}
            </video>
          </div>
        </Card.Body>
      </Card>

      <Card>
        <Card.Header>
          <Card.Title level={5}>
            <i className="fas fa-tags me-2"></i>
            {t('videos:details.assigned_labels')}
          </Card.Title>
        </Card.Header>
        <Card.Body>
          {labelsLoading ? (
            <LoadingSpinner message="Loading labels..." size="small" />
          ) : labelsError ? (
            <ErrorAlert error={labelsError} />
          ) : assignedLabels.length > 0 ? (
            <Table striped hover responsive>
              <Table.Head>
                <Table.Row>
                  <Table.Cell header={true}>#</Table.Cell>
                  <Table.Cell header={true}>{t('videos:table.label')}</Table.Cell>
                  <Table.Cell header={true}>{t('videos:table.labeler')}</Table.Cell>
                  <Table.Cell header={true}>{t('videos:table.subject')}</Table.Cell>
                  <Table.Cell header={true}>{t('videos:table.timestamp')}</Table.Cell>
                  <Table.Cell header={true}>{t('videos:table.ins_date')}</Table.Cell>
                </Table.Row>
              </Table.Head>
              <Table.Body>
                {assignedLabels.map((label, index) => (
                  <Table.Row key={label.id}>
                    <Table.Cell>{index + 1}</Table.Cell>
                    <Table.Cell>
                      <span className="badge bg-primary">{label.labelName || 'Unknown'}</span>
                    </Table.Cell>
                    <Table.Cell>{label.labelerName || 'Unknown'}</Table.Cell>
                    <Table.Cell>{label.subjectName || 'Unknown'}</Table.Cell>
                    <Table.Cell>
                      {label.timestamp ? `${label.timestamp}s` : 'N/A'}
                    </Table.Cell>
                    <Table.Cell>
                      {label.insDate ? new Date(label.insDate).toLocaleString() : 'N/A'}
                    </Table.Cell>
                  </Table.Row>
                ))}
              </Table.Body>
            </Table>
          ) : (
            <Alert variant="info">
              <i className="fas fa-info-circle me-2"></i>
              {t('videos:details.no_labels')}
            </Alert>
          )}
        </Card.Body>
      </Card>
    </Container>
  );
};

export default VideoDetails;
