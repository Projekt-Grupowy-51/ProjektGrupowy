import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Container, Card, Button, Table, Alert } from '../components/ui';
import { FAKE_VIDEOS, FAKE_VIDEO_ASSIGNED_LABELS, findById, findByProperty } from '../data/fakeData.js';

const VideoDetails = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { t } = useTranslation(['videos', 'common']);
  
  // Stan komponentu
  const [video, setVideo] = useState(null);
  const [videoLoading, setVideoLoading] = useState(true);
  const [videoError, setVideoError] = useState(null);
  const [assignedLabels, setAssignedLabels] = useState([]);
  const [labelsLoading, setLabelsLoading] = useState(true);
  const [labelsError, setLabelsError] = useState(null);

  useEffect(() => {
    // Ładowanie video z kolekcji
    const loadVideo = async () => {
      setVideoLoading(true);
      await new Promise(resolve => setTimeout(resolve, 500));
      
      const foundVideo = findById(FAKE_VIDEOS, parseInt(id));
      if (foundVideo) {
        setVideo(foundVideo);
      } else {
        setVideoError('Video not found');
      }
      setVideoLoading(false);
    };
    
    // Ładowanie assigned labels dla tego video
    const loadVideoAssignedLabels = async () => {
      setLabelsLoading(true);
      await new Promise(resolve => setTimeout(resolve, 300));
      
      const videoLabels = findByProperty(FAKE_VIDEO_ASSIGNED_LABELS, 'videoId', parseInt(id));
      setAssignedLabels(videoLabels);
      setLabelsLoading(false);
    };
    
    if (id) {
      loadVideo();
      loadVideoAssignedLabels();
    }
  }, [id]);

  const handleBackToVideoGroup = () => {
    if (video?.videoGroupId) {
      navigate(`/video-groups/${video.videoGroupId}`);
    }
  };

  const formatDuration = (seconds) => {
    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = seconds % 60;
    return `${minutes}:${remainingSeconds.toString().padStart(2, '0')}`;
  };

  if (videoLoading) {
    return (
      <Container className="py-4">
        <div className="d-flex justify-content-center">
          <div className="spinner-border text-primary" role="status">
            <span className="visually-hidden">{t('common:states.loading')}</span>
          </div>
        </div>
      </Container>
    );
  }

  if (videoError) {
    return (
      <Container className="py-4">
        <Alert variant="danger">
          <i className="fas fa-exclamation-triangle me-2"></i>
          {videoError}
        </Alert>
      </Container>
    );
  }

  if (!video) {
    return (
      <Container className="py-4">
        <Alert variant="warning">
          <i className="fas fa-exclamation-triangle me-2"></i>
          {t('videos:messages.not_found')}
        </Alert>
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
              src={`/api/Video/${id}/stream`}
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
            <div className="text-center py-3">
              <div className="spinner-border text-primary" role="status">
                <span className="visually-hidden">{t('common:states.loading')}</span>
              </div>
            </div>
          ) : labelsError ? (
            <Alert variant="danger">
              <i className="fas fa-exclamation-triangle me-2"></i>
              {labelsError}
            </Alert>
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
