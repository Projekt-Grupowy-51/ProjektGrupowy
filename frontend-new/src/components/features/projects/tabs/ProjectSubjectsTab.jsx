import React from 'react';
import PropTypes from 'prop-types';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';
import { Card, Button, Alert } from '../../../ui';

const ProjectSubjectsTab = ({ projectId, subjects = [] }) => {
  const { t } = useTranslation(['common', 'projects']);
  const navigate = useNavigate();

  return (
    <Card>
      <Card.Header>
        <div className="d-flex justify-content-between align-items-center">
          <Card.Title level={5}>
            <i className="fas fa-folder me-2"></i>
            {t('projects:tabs.subjects')}
          </Card.Title>
          <Button
            variant="primary"
            size="sm"
            icon="fas fa-plus"
            onClick={() => navigate(`/subjects/add?projectId=${projectId}`)}
          >
            {t('projects:add.subject')}
          </Button>
        </div>
      </Card.Header>
      <Card.Body>
        {subjects.length === 0 ? (
          <Alert variant="info">
            <i className="fas fa-info-circle me-2"></i>
            {t('projects:not_found.subject')}
          </Alert>
        ) : (
          <div className="list-group">
            {subjects.map((subject) => (
              <div key={subject.id} className="list-group-item d-flex justify-content-between align-items-center">
                <div>
                  <div className="fw-bold">{subject.name}</div>
                  <small className="text-muted">{subject.email}</small>
                </div>
                <div className="d-flex gap-1">
                  <Button
                    size="sm"
                    variant="info"
                    icon="fas fa-eye"
                    onClick={() => navigate(`/subjects/${subject.id}`)}
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

ProjectSubjectsTab.propTypes = {
  projectId: PropTypes.oneOfType([PropTypes.string, PropTypes.number]).isRequired,
  subjects: PropTypes.array
};

export default ProjectSubjectsTab;
