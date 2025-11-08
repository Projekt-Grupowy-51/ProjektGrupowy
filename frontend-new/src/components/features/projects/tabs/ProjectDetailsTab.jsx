import React from 'react';
import PropTypes from 'prop-types';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Button, Card } from '../../../ui';
import { LoadingSpinner, ErrorAlert, EmptyState } from '../../../common';
import { useProjectDetails } from '../../../../hooks/useProjectDetails.js';
import ProjectReportsSection from '../ProjectReportsSection.jsx';

const ProjectDetailsTab = ({ projectId, onDeleteProject }) => {
  const navigate = useNavigate();
  const { t } = useTranslation(['common', 'projects']);

  const {
    project,
    loading: projectLoading,
    error: projectError
  } = useProjectDetails(projectId);

  const handleDeleteProject = () => {
    if (onDeleteProject) {
      onDeleteProject(projectId);
    }
    navigate('/projects');
  };
  
  if (projectLoading) return <LoadingSpinner />;
  if (projectError) return <ErrorAlert error={projectError} />;
  if (!project) return <EmptyState icon="fas fa-folder" message={t('projects:messages.project_not_found')} />;


  const formatDate = (dateString) => {
    if (!dateString) return '-';
    return new Date(dateString).toLocaleDateString();
  };

  return (
    <div className="row">
      <div className="col-lg-8">
        <Card>
          <Card.Header>
            <Card.Title level={5}>
              <i className="fas fa-info-circle me-2"></i>
              {t('projects:project_details')}
            </Card.Title>
          </Card.Header>
          <Card.Body>
            <div className="row mb-3">
              <div className="col-sm-3">
                <strong>{t('projects:form.name')}:</strong>
              </div>
              <div className="col-sm-9">
                {project.name}
              </div>
            </div>
            
            <div className="row mb-3">
              <div className="col-sm-3">
                <strong>{t('projects:form.description')}:</strong>
              </div>
              <div className="col-sm-9">
                {project.description}
              </div>
            </div>
            
            <div className="row mb-3">
              <div className="col-sm-3">
                <strong>{t('projects:form.status')}:</strong>
              </div>
              <div className="col-sm-9">
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
            </div>
            
            <div className="row mb-3">
              <div className="col-sm-3">
                <strong>ID:</strong>
              </div>
              <div className="col-sm-9">
                {project.id}
              </div>
            </div>

            <div className="row mb-3">
              <div className="col-sm-3">
                <strong>{t('common:created_at')}:</strong>
              </div>
              <div className="col-sm-9">
                {formatDate(project.creationDate)}
              </div>
            </div>

            <div className="row mb-3">
              <div className="col-sm-3">
                <strong>{t('common:updated_at')}:</strong>
              </div>
              <div className="col-sm-9">
                {formatDate(project.modificationDate)}
              </div>
            </div>
          </Card.Body>
          <Card.Footer>
              <div className="d-flex gap-2">
              <Button
                size="sm"
                variant="outline-warning"
                icon="fas fa-edit"
                onClick={() => navigate(`/projects/${projectId}/edit`, { 
                  state: { from: `/projects/${projectId}` } 
                })}
              >
                {t('common:buttons.edit')}
              </Button>
              <Button
                size="sm"
                variant="outline-danger"
                icon="fas fa-trash"
                confirmAction={true}
                confirmTitle={t('common:deleteConfirmation.title')}
                confirmMessage={t('projects:messages.confirm_delete')}
                confirmText={t('common:deleteConfirmation.confirm')}
                cancelText={t('common:deleteConfirmation.cancel')}
                onConfirm={handleDeleteProject}
              >
                {t('common:buttons.delete')}
              </Button>
            </div>
          </Card.Footer>
        </Card>
      </div>

      <div className="col-lg-4">
        <ProjectReportsSection projectId={projectId} />
      </div>
    </div>
  );
};

ProjectDetailsTab.propTypes = {
  projectId: PropTypes.oneOfType([PropTypes.string, PropTypes.number]).isRequired,
  onDeleteProject: PropTypes.func
};

export default ProjectDetailsTab;
