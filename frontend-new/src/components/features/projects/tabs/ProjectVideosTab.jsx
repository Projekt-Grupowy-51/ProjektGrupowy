import React from 'react';
import PropTypes from 'prop-types';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';
import { Card, Button, Alert } from '../../../ui';

const ProjectVideosTab = ({ projectId, videoGroups = [] }) => {
  const { t } = useTranslation(['common', 'projects']);
  const navigate = useNavigate();

  return (
    <Card>
      <Card.Header>
        <div className="d-flex justify-content-between align-items-center">
          <Card.Title level={5}>
            <i className="fas fa-film me-2"></i>
            {t('projects:tabs.videos')}
          </Card.Title>
          <Button
            variant="primary"
            size="sm"
            icon="fas fa-plus"
            onClick={() => navigate(`/videogroups/add?projectId=${projectId}`)}
          >
            {t('projects:add.video_group')}
          </Button>
        </div>
      </Card.Header>
      <Card.Body>
        {videoGroups.length === 0 ? (
          <Alert variant="info">
            <i className="fas fa-info-circle me-2"></i>
            {t('projects:not_found.video_group')}
          </Alert>
        ) : (
          <div className="list-group">
            {videoGroups.map((videoGroup) => (
              <div key={videoGroup.id} className="list-group-item d-flex justify-content-between align-items-center">
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
                </div>
              </div>
            ))}
          </div>
        )}
      </Card.Body>
    </Card>
  );
};

ProjectVideosTab.propTypes = {
  projectId: PropTypes.oneOfType([PropTypes.string, PropTypes.number]).isRequired,
  videoGroups: PropTypes.array
};

export default ProjectVideosTab;
