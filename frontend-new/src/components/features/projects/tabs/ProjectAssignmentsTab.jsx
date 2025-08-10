import React from 'react';
import PropTypes from 'prop-types';
import { useTranslation } from 'react-i18next';
import { Card, Button, Alert } from '../../../ui';

const ProjectAssignmentsTab = ({ projectId, assignments = [] }) => {
  const { t } = useTranslation(['common', 'projects']);

  return (
    <Card>
      <Card.Header>
        <div className="d-flex justify-content-between align-items-center">
          <Card.Title level={5}>
            <i className="fas fa-tasks me-2"></i>
            {t('projects:tabs.assignments')}
          </Card.Title>
          <Button
            variant="primary"
            size="sm"
            icon="fas fa-plus"
            onClick={() => window.location.href = `/projects/${projectId}/assignments/add`}
          >
            {t('projects:add.assignment')}
          </Button>
        </div>
      </Card.Header>
      <Card.Body>
        {assignments.length === 0 ? (
          <Alert variant="info">
            <i className="fas fa-info-circle me-2"></i>
            {t('projects:not_found.assignment')}
          </Alert>
        ) : (
          <div className="list-group">
            {assignments.map((assignment) => (
              <div key={assignment.id} className="list-group-item d-flex justify-content-between align-items-center">
                <div>
                  <div className="fw-bold">Assignment #{assignment.id}</div>
                  <small className="text-muted">
                    Subject: {assignment.subjectId} | Video: {assignment.videoId}
                  </small>
                </div>
                <div className="d-flex gap-1">
                  {assignment.completed ? (
                    <span className="badge bg-success">
                      <i className="fas fa-check me-1"></i>
                      Completed
                    </span>
                  ) : (
                    <span className="badge bg-warning">
                      <i className="fas fa-clock me-1"></i>
                      Pending
                    </span>
                  )}
                  <Button
                    size="sm"
                    variant="info"
                    icon="fas fa-eye"
                    onClick={() => window.location.href = `/assignments/${assignment.id}`}
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

ProjectAssignmentsTab.propTypes = {
  projectId: PropTypes.oneOfType([PropTypes.string, PropTypes.number]).isRequired,
  assignments: PropTypes.array
};

export default ProjectAssignmentsTab;
