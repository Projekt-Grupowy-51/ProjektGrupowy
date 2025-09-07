import React from 'react';
import PropTypes from 'prop-types';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';
import { Card, Button } from '../../../ui';
import { EmptyState, TabHeader, TabListGroup, LoadingSpinner, ErrorAlert } from '../../../common';
import { useProjectVideoGroups } from '../../../../hooks/useProjectVideoGroups.js';

const ProjectVideosTab = ({ projectId }) => {
  const { t } = useTranslation(['common', 'projects']);
  const navigate = useNavigate();
  
  const {
    videoGroups,
    loading,
    error,
    deleteVideoGroup
  } = useProjectVideoGroups(projectId);

  if (loading) return <LoadingSpinner />;
  if (error) return <ErrorAlert error={error} />;

  const renderVideoGroupItem = (videoGroup) => (
    <>
      <div>
        <div className="fw-bold">{videoGroup.name}</div>
        <small className="text-muted">{videoGroup.description}</small>
      </div>
      <div className="d-flex gap-1">
        <Button
          size="sm"
          variant="info"
          icon="fas fa-eye"
          onClick={() => navigate(`/videogroups/${videoGroup.id}`)}
        >
          {t('common:buttons.details')}
        </Button>
        <Button
          size="sm"
          variant="outline-warning"
          icon="fas fa-edit"
          onClick={() => navigate(`/videogroups/${videoGroup.id}/edit`)}
        >
          {t('common:buttons.edit')}
        </Button>
        <Button
          size="sm"
          variant="outline-danger"
          icon="fas fa-trash"
          confirmAction={true}
          confirmTitle="Potwierdź usunięcie"
          confirmMessage={`Czy na pewno chcesz usunąć grupę wideo "${videoGroup.name}"? Ta operacja jest nieodwracalna.`}
          confirmText="Usuń"
          cancelText="Anuluj"
          onConfirm={() => deleteVideoGroup(videoGroup.id)}
        >
          {t('common:buttons.delete')}
        </Button>
      </div>
    </>
  );

  return (
    <Card>
      <TabHeader
        icon="fas fa-film"
        title={t('projects:tabs.videos')}
        actionText={t('projects:add.video_group')}
        onAction={() => navigate(`/videogroups/add?projectId=${projectId}`)}
      />
      <Card.Body>
        {videoGroups.length === 0 ? (
          <EmptyState
            icon="fas fa-film"
            message={t('projects:not_found.video_group')}
            actionText={t('projects:add.video_group')}
            onAction={() => navigate(`/videogroups/add?projectId=${projectId}`)}
          />
        ) : (
          <TabListGroup items={videoGroups} renderItem={renderVideoGroupItem} />
        )}
      </Card.Body>
    </Card>
  );
};

ProjectVideosTab.propTypes = {
  projectId: PropTypes.oneOfType([PropTypes.string, PropTypes.number]).isRequired
};

export default ProjectVideosTab;
