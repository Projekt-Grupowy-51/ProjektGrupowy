import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Container, Card, Table, Button, Alert } from '../components/ui';
import { FAKE_VIDEO_GROUPS, FAKE_VIDEOS, findById, findByProperty, removeFromCollection } from '../data/fakeData.js';

const VideoGroupDetails = () => {
  const { t } = useTranslation();
  const { id } = useParams();
  const navigate = useNavigate();
  
  // Stan komponentu
  const [videoGroup, setVideoGroup] = useState(null);
  const [videos, setVideos] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);

  const getVideoGroup = async (videoGroupId) => {
    // Pobieranie z kolekcji
    await new Promise(resolve => setTimeout(resolve, 500));
    return findById(FAKE_VIDEO_GROUPS, videoGroupId);
  };

  const getVideoGroupVideos = async (videoGroupId) => {
    // Pobieranie videos dla konkretnej grupy
    await new Promise(resolve => setTimeout(resolve, 300));
    return findByProperty(FAKE_VIDEOS, 'videoGroupId', parseInt(videoGroupId));
  };

  const deleteVideo = async (videoId) => {
    // Usuwanie z kolekcji
    await new Promise(resolve => setTimeout(resolve, 500));
    
    const deletedVideo = removeFromCollection(FAKE_VIDEOS, videoId);
    if (deletedVideo) {
      console.log('Deleted video:', videoId);
      console.log('Current videos collection:', FAKE_VIDEOS);
      
      // Aktualizacja lokalnego stanu
      setVideos(prev => prev.filter(v => v.id !== videoId));
    }
  };

  useEffect(() => {
    const loadData = async () => {
      try {
        setIsLoading(true);
        
        // Load video group details and videos
        const [videoGroupData, videosData] = await Promise.all([
          getVideoGroup(parseInt(id)),
          getVideoGroupVideos(parseInt(id))
        ]);

        setVideoGroup(videoGroupData);
        setVideos(videosData);
      } catch (error) {
        console.error('Error loading video group data:', error);
        setError(t('videoGroups.errors.loadFailed'));
      } finally {
        setIsLoading(false);
      }
    };

    if (id) {
      loadData();
    }
  }, [id, t]);

  const handleDeleteVideo = async (videoId, videoTitle) => {
    if (window.confirm(t('videos.confirmDelete', { title: videoTitle }))) {
      try {
        await deleteVideo(videoId);
        setVideos(prev => prev.filter(video => video.id !== videoId));
      } catch (error) {
        console.error('Error deleting video:', error);
        setError(t('videos.errors.deleteFailed'));
      }
    }
  };

  const handleAddVideo = () => {
    navigate(`/videos/add?videogroupId=${id}`);
  };

  const handleBack = () => {
    if (videoGroup?.projectId) {
      navigate(`/projects/${videoGroup.projectId}`);
    } else {
      navigate('/projects');
    }
  };

  if (isLoading) {
    return (
      <Container>
        <div className="d-flex justify-content-center py-5">
          <div className="spinner-border text-primary" role="status">
            <span className="visually-hidden">{t('common:loading')}</span>
          </div>
        </div>
      </Container>
    );
  }

  if (error) {
    return (
      <Container>
        <Alert variant="danger">
          <i className="fas fa-exclamation-triangle me-2"></i>
          {error}
        </Alert>
      </Container>
    );
  }

  if (!videoGroup) {
    return (
      <Container>
        <Alert variant="warning">
          <i className="fas fa-exclamation-triangle me-2"></i>
          {t('videoGroups:not_found')}
        </Alert>
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
                    variant="outline-secondary"
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
                              variant="outline-primary"
                              onClick={() => navigate(`/videos/${video.id}`)}
                            >
                              <i className="fas fa-eye me-1"></i>
                              {t('common:details')}
                            </Button>
                            <Button
                              size="sm"
                              variant="outline-danger"
                              onClick={() => handleDeleteVideo(video.id, video.title)}
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
