import React from 'react';
import PropTypes from 'prop-types';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Button, Card } from '../../../ui';
import { LoadingSpinner, ErrorAlert, EmptyState } from '../../../common';
import { useProjectReports } from '../../../../hooks/useProjectReports.js';
import { useProjectDetails } from '../../../../hooks/useProjectDetails.js';
import { useConfirmDialog } from '../../../../hooks/common';

const ProjectDetailsTab = ({ projectId, onDeleteProject }) => {
  const navigate = useNavigate();
  const { t } = useTranslation(['common', 'projects']);
  const { confirmWithPrompt } = useConfirmDialog();
  
  const {
    project,
    loading: projectLoading,
    error: projectError
  } = useProjectDetails(projectId);
  
  const {
    reports,
    loading: reportsLoading,
    error: reportsError,
    generateReport,
    deleteReport,
    downloadReport
  } = useProjectReports(projectId);

  const handleDeleteProject = () => {
    confirmWithPrompt(t('projects:messages.confirm_delete'), () => {
      if (onDeleteProject) {
        onDeleteProject(projectId);
      }
      navigate('/projects');
    });
  };
  
  if (projectLoading) return <LoadingSpinner />;
  if (projectError) return <ErrorAlert error={projectError} />;
  if (!project) return <EmptyState icon="fas fa-folder" message="Project not found" />;


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
                variant="warning"
                icon="fas fa-edit"
                onClick={() => navigate(`/projects/${projectId}/edit`, { 
                  state: { from: `/projects/${projectId}` } 
                })}
              >
                {t('common:buttons.edit')}
              </Button>
              <Button
                variant="outline"
                icon="fas fa-trash"
                onClick={handleDeleteProject}
              >
                {t('common:buttons.delete')}
              </Button>
            </div>
          </Card.Footer>
        </Card>
      </div>

      <div className="col-lg-4">
        <Card>
          <Card.Header>
            <Card.Title level={5}>
              <i className="fas fa-chart-bar me-2"></i>
              {t('projects:reports.title')}
            </Card.Title>
          </Card.Header>
          <Card.Body>
            {reportsLoading ? (
              <div className="text-center py-3">
                <LoadingSpinner size="small" />
              </div>
            ) : reportsError ? (
              <ErrorAlert error={reportsError} />
            ) : reports.length === 0 ? (
              <EmptyState
                icon="fas fa-chart-bar"
                message={t('projects:reports.no_reports')}
              />
            ) : (
              <div className="list-group" style={{ maxHeight: '300px', overflowY: 'auto' }}>
                {reports.map((report) => (
                  <div key={report.id} className="list-group-item d-flex justify-content-between align-items-center">
                    <div>
                      <div className="fw-bold">{report.name || `Report #${report.id}`}</div>
                      <small className="text-muted">{formatDate(report.createdAt)}</small>
                    </div>
                    <div className="d-flex gap-1">
                      <Button
                        size="sm"
                        variant="outline"
                        icon="fas fa-download"
                        onClick={() => downloadReport(report.id)}
                      >
                        {t('common:buttons.download')}
                      </Button>
                      <Button
                        size="sm"
                        variant="outline"
                        icon="fas fa-trash"
                        onClick={() => deleteReport(report.id, t('projects:reports.confirm_delete'))}
                      >
                        {t('common:buttons.delete')}
                      </Button>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </Card.Body>
          <Card.Footer>
            <Button
              variant="primary"
              icon="fas fa-plus"
              onClick={() => generateReport()}
              className="w-100"
              disabled={reportsLoading}
            >
              {reportsLoading ? t('common:states.loading') : t('projects:reports.generate')}
            </Button>
          </Card.Footer>
        </Card>
      </div>
    </div>
  );
};

ProjectDetailsTab.propTypes = {
  projectId: PropTypes.oneOfType([PropTypes.string, PropTypes.number]).isRequired,
  onDeleteProject: PropTypes.func
};

export default ProjectDetailsTab;
