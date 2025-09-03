import React from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Container, Card, Button } from '../components/ui';
import { LoadingSpinner, ErrorAlert, EmptyState } from '../components/common';
import { useVideoGroupDetails } from '../hooks/useVideoGroupDetails.js';

const VideoGroupDetails = () => {
  const { t } = useTranslation();
  const { id } = useParams();
  const navigate = useNavigate();
  
  const {
    videoGroup,
    videos,
    loading,
    error,
    deleteVideo,
    handleBack,
    handleAddVideo
  } = useVideoGroupDetails(id);

  if (loading) {
    return (
      <Container>
        <LoadingSpinner />
      </Container>
    );
  }

  if (error) {
    return (
      <Container>
        <ErrorAlert error={error} />
      </Container>
    );
  }

  if (!videoGroup) {
    return (
      <Container>
        <ErrorAlert error={t('videoGroups:not_found')} />
      </Container>
    );
  }

  return (
    <Container className="py-4">
      <Container.Row>
        <Container.Col>
          <Card>
            <Card.Header>
              <div className="d-flex justify-content-between align-items-center">
                <div>
                  <Card.Title level={3} className="mb-1">{videoGroup.name}</Card.Title>
                  <p className="text-muted mb-0">{videoGroup.description}</p>
                </div>
                <div className="d-flex gap-2">
                  <Button
                    variant="primary"
                    onClick={handleAddVideo}
                  >
                    <i className="fas fa-plus me-2"></i>
                    {t('videos:add_video')}
                  </Button>
                  <Button
                    variant="outline"
                    onClick={handleBack}
                  >
                    <i className="fas fa-arrow-left me-2"></i>
                    {t('common:back')}
                  </Button>
                </div>
              </div>
            </Card.Header>
            <Card.Body>
              {videos.length > 0 ? (
                <Container.Row>
                  {videos.map((video) => (
                    <Container.Col key={video.id} md={6} lg={4} className="mb-4">
                      <Card className="h-100 video-card">
                        <Card.Body>
                          <Card.Title level={6} className="card-title">{video.title}</Card.Title>
                          <p className="text-muted small">
                            {t('videos:position')}: {video.positionInQueue}
                          </p>
                          <div className="d-flex gap-2 mt-auto">
                            <Button
                              size="sm"
                              variant="outline"
                              onClick={() => navigate(`/videos/${video.id}`)}
                            >
                              <i className="fas fa-eye me-1"></i>
                              {t('common:details')}
                            </Button>
                            <Button
                              size="sm"
                              variant="outline"
                              onClick={() => deleteVideo(video.id, video.title)}
                            >
                              <i className="fas fa-trash me-1"></i>
                              {t('common:delete')}
                            </Button>
                          </div>
                        </Card.Body>
                      </Card>
                    </Container.Col>
                  ))}
                </Container.Row>
              ) : (
                <div className="text-center py-5">
                  <i className="fas fa-film fs-1 text-muted opacity-50"></i>
                  <p className="text-muted mt-3 mb-0">
                    {t('videoGroups:no_videos')}
                  </p>
                  <Button
                    variant="primary"
                    className="mt-3"
                    onClick={handleAddVideo}
                  >
                    <i className="fas fa-plus me-2"></i>
                    {t('videos:add_first_video')}
                  </Button>
                </div>
              )}
            </Card.Body>
          </Card>
        </Container.Col>
      </Container.Row>
    </Container>
  );
};

export default VideoGroupDetails;
