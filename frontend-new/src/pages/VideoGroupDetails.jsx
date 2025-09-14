import React from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Container, Card, Button, Table } from '../components/ui';
import { LoadingSpinner, ErrorAlert, EmptyState } from '../components/common';
import { useVideoGroupDetails } from '../hooks/useVideoGroupDetails.js';

const VideoGroupDetails = () => {
  const { t } = useTranslation(['videoGroups', 'videos', 'common']);
  const { id } = useParams();
  const navigate = useNavigate();
  
  const {
    videoGroup,
    videos,
    loading,
    error,
    deleteVideo,
    deleteVideoGroup,
    handleBack,
    handleAddVideo,
    handleEditVideoGroup
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
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h1>
            <i className="fas fa-film me-2"></i>
            {videoGroup.name}
          </h1>
          <p className="text-muted mb-0">{videoGroup.description}</p>
        </div>
        <div className="d-flex gap-2">
          <Button
            variant="warning"
            icon="fas fa-edit"
            onClick={handleEditVideoGroup}
          >
            {t('common:buttons.edit')}
          </Button>
          <Button
            variant="outline-danger"
            icon="fas fa-trash"
            confirmAction={true}
            confirmTitle={t('common:deleteConfirmation.title')}
            confirmMessage={t('videoGroups:confirm_delete_group', { name: videoGroup.name })}
            confirmText={t('common:deleteConfirmation.confirm')}
            cancelText={t('common:deleteConfirmation.cancel')}
            onConfirm={deleteVideoGroup}
          >
            {t('common:buttons.delete')}
          </Button>
          <Button
            variant="outline-secondary"
            icon="fas fa-arrow-left"
            onClick={handleBack}
          >
            {t('common:buttons.back')}
          </Button>
        </div>
      </div>

      <Card className="mb-4">
        <Card.Header variant="info">
          <Card.Title level={5} className="mb-0">
            <i className="fas fa-info-circle me-2"></i>
            {t('videoGroups:details.title')}
          </Card.Title>
        </Card.Header>
        <Card.Body>
          <Card.Text>
            <strong>{t('videoGroups:details.description')}:</strong> {videoGroup.description}
          </Card.Text>
        </Card.Body>
      </Card>

      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2 className="section-title mb-0">
          <i className="fas fa-video me-2"></i>
          {t('videoGroups:videos.title')}
        </h2>
        <Button
          variant="primary"
          icon="fas fa-plus"
          onClick={handleAddVideo}
        >
          {t('videoGroups:videos.add')}
        </Button>
      </div>

      {videos.length > 0 ? (
        <Card>
          <Table striped hover responsive>
            <Table.Head>
              <Table.Row>
                <Table.Cell header={true}>#</Table.Cell>
                <Table.Cell header={true}>{t('videos:columns.title')}</Table.Cell>
                <Table.Cell header={true}>{t('videos:columns.position')}</Table.Cell>
                <Table.Cell header={true}>{t('common:actions')}</Table.Cell>
              </Table.Row>
            </Table.Head>
            <Table.Body>
              {videos.map((video, index) => (
                <Table.Row key={video.id}>
                  <Table.Cell>{index + 1}</Table.Cell>
                  <Table.Cell>{video.title}</Table.Cell>
                  <Table.Cell>
                    <span className="badge bg-secondary">{video.positionInQueue}</span>
                  </Table.Cell>
                  <Table.Cell>
                    <div className="d-flex gap-2">
                      <Button
                        size="sm"
                        variant="info"
                        icon="fas fa-eye"
                        onClick={() => navigate(`/videos/${video.id}`)}
                      >
                        {t('common:buttons.details')}
                      </Button>
                      <Button
                        size="sm"
                        variant="outline-warning"
                        icon="fas fa-edit"
                        onClick={() => navigate(`/videos/${video.id}/edit`)}
                      >
                        {t('common:buttons.edit')}
                      </Button>
                      <Button
                        size="sm"
                        variant="outline-danger"
                        icon="fas fa-trash"
                        confirmAction={true}
                        confirmTitle={t('common:deleteConfirmation.title')}
                        confirmMessage={t('videoGroups:confirm_delete_video', { title: video.title })}
                        confirmText={t('common:deleteConfirmation.confirm')}
                        cancelText={t('common:deleteConfirmation.cancel')}
                        onConfirm={() => deleteVideo(video.id)}
                      >
                        {t('common:buttons.delete')}
                      </Button>
                    </div>
                  </Table.Cell>
                </Table.Row>
              ))}
            </Table.Body>
          </Table>
        </Card>
      ) : (
        <EmptyState
          icon="fas fa-video"
          title={t('videoGroups:videos.no_videos')}
          message={t('videoGroups:videos.empty')}
          actionText={t('videoGroups:videos.add')}
          onAction={handleAddVideo}
        />
      )}
    </Container>
  );
};

export default VideoGroupDetails;
