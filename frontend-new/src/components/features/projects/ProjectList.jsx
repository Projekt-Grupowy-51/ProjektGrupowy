import React, { useState } from 'react';
import PropTypes from 'prop-types';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';
import { Card, Button } from '../../ui';

const ProjectList = ({ projects, onDelete }) => {
  const { t } = useTranslation(['projects', 'common']);
  const navigate = useNavigate();

  if (!projects || projects.length === 0) {
    return (
      <Card>
        <Card.Body className="text-center py-5">
          <i className="fas fa-folder-open fa-3x text-muted mb-3"></i>
          <h5 className="text-muted">{t('projects:messages.no_projects')}</h5>
          <p className="text-muted">{t('projects:messages.create_first_project')}</p>
        </Card.Body>
      </Card>
    );
  }

  return (
    <div className="row">
      {projects.map((project) => (
        <div key={project.id} className="col-lg-6 mb-4">
          <Card className="h-100">
            <Card.Body>
              <div className="d-flex justify-content-between align-items-start mb-3">
                <Card.Title level={5} className="mb-0">
                  {project.name}
                </Card.Title>
                {project.finished ? (
                  <span className="badge bg-success">
                    <i className="fas fa-check me-1"></i>
                    {t('projects:status.finished')}
                  </span>
                ) : (
                  <span className="badge bg-primary">
                    <i className="fas fa-clock me-1"></i>
                    {t('projects:status.in_progress')}
                  </span>
                )}
              </div>
              
              <p className="text-muted mb-3">
                {project.description}
              </p>
              
              <div className="row text-muted small mb-3">
                <div className="col-6">
                  <strong>ID:</strong> {project.id}
                </div>
                <div className="col-6">
                  <strong>{t('common:created_at')}:</strong>{' '}
                  {project.createdAt ? new Date(project.createdAt).toLocaleDateString() : '-'}
                </div>
              </div>
            </Card.Body>
            
            <Card.Footer>
              <div className="d-flex gap-2">
                <Button
                  size="sm"
                  variant="outline-primary"
                  icon="fas fa-eye"
                  onClick={() => navigate(`/projects/${project.id}`)}
                >
                  {t('common:buttons.details')}
                </Button>
                <Button
                  size="sm"
                  variant="outline-warning"
                  icon="fas fa-edit"
                  onClick={() => navigate(`/projects/${project.id}/edit`)}
                >
                  {t('common:buttons.edit')}
                </Button>
                <Button
                  size="sm"
                  variant="outline-danger"
                  icon="fas fa-trash"
                  onClick={() => {
                    if (window.confirm(t('projects:messages.confirm_delete'))) {
                      onDelete(project.id);
                    }
                  }}
                >
                  {t('common:buttons.delete')}
                </Button>
              </div>
            </Card.Footer>
          </Card>
        </div>
      ))}
    </div>
  );
};

ProjectList.propTypes = {
  projects: PropTypes.arrayOf(PropTypes.shape({
    id: PropTypes.number.isRequired,
    name: PropTypes.string.isRequired,
    description: PropTypes.string,
    finished: PropTypes.bool,
    createdAt: PropTypes.string,
    updatedAt: PropTypes.string
  })).isRequired,
  onDelete: PropTypes.func.isRequired
};

export default ProjectList;
