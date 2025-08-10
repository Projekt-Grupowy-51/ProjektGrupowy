import React from 'react';
import PropTypes from 'prop-types';
import { useTranslation } from 'react-i18next';
import { Card, Alert } from '../../../ui';

const ProjectLabelersTab = ({ projectId, labelers = [], labelersCount = 0 }) => {
  const { t } = useTranslation(['common', 'projects']);

  return (
    <Card>
      <Card.Header>
        <Card.Title level={5}>
          <i className="fas fa-users me-2"></i>
          {t('projects:tabs.labelers')}
          <span className="badge bg-primary rounded-pill ms-2">
            {labelersCount}
          </span>
        </Card.Title>
      </Card.Header>
      <Card.Body>
        {labelers.length === 0 ? (
          <Alert variant="info">
            <i className="fas fa-info-circle me-2"></i>
            {t('projects:not_found.unassigned_labelers')}
          </Alert>
        ) : (
          <div className="list-group">
            {labelers.map((labeler) => (
              <div key={labeler.id} className="list-group-item d-flex justify-content-between align-items-center">
                <div>
                  <div className="fw-bold">{labeler.name}</div>
                  <small className="text-muted">{labeler.email}</small>
                </div>
                <div className="d-flex gap-1">
                  <span className="badge bg-success">
                    <i className="fas fa-check me-1"></i>
                    Active
                  </span>
                </div>
              </div>
            ))}
          </div>
        )}
      </Card.Body>
    </Card>
  );
};

ProjectLabelersTab.propTypes = {
  projectId: PropTypes.oneOfType([PropTypes.string, PropTypes.number]).isRequired,
  labelers: PropTypes.array,
  labelersCount: PropTypes.number
};

export default ProjectLabelersTab;
