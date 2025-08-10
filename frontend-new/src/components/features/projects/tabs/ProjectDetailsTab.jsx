import React, { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Button, Card, Alert } from '../../../ui';
import { FAKE_PROJECT_REPORTS, findByProperty, addToCollection, removeFromCollection } from '../../../../data/fakeData.js';

const ProjectDetailsTab = ({ project, onDeleteProject }) => {
  const navigate = useNavigate();
  const { t } = useTranslation(['common', 'projects']);
  
  // Stan dla raportów
  const [reports, setReports] = useState([]);
  const [reportsLoading, setReportsLoading] = useState(false);
  const [reportsError, setReportsError] = useState(null);

  // Ładowanie raportów dla projektu
  useEffect(() => {
    const loadReports = async () => {
      setReportsLoading(true);
      await new Promise(resolve => setTimeout(resolve, 500)); // Symulacja opóźnienia
      
      try {
        const projectReports = findByProperty(FAKE_PROJECT_REPORTS, 'projectId', project.id);
        setReports(projectReports);
        setReportsError(null);
      } catch (error) {
        setReportsError('Failed to load reports');
      } finally {
        setReportsLoading(false);
      }
    };

    if (project.id) {
      loadReports();
    }
  }, [project.id]);

  const generateReport = async (reportData) => {
    setReportsLoading(true);
    await new Promise(resolve => setTimeout(resolve, 1000)); // Symulacja opóźnienia
    
    try {
      const newReport = {
        id: Date.now(), // Prosty generator ID
        ...reportData,
        projectId: project.id,
        createdAt: new Date().toISOString(),
        url: `/api/projectreport/download/${Date.now()}`
      };
      
      addToCollection(FAKE_PROJECT_REPORTS, newReport);
      
      // Aktualizacja lokalnego stanu
      setReports(prev => [newReport, ...prev]);
      
      console.log('Generated report:', newReport);
    } catch (error) {
      setReportsError('Failed to generate report');
      throw error;
    } finally {
      setReportsLoading(false);
    }
  };

  const deleteReport = async (reportId) => {
    setReportsLoading(true);
    await new Promise(resolve => setTimeout(resolve, 500)); // Symulacja opóźnienia
    
    try {
      const deletedReport = removeFromCollection(FAKE_PROJECT_REPORTS, reportId);
      if (deletedReport) {
        // Aktualizacja lokalnego stanu
        setReports(prev => prev.filter(r => r.id !== reportId));
        console.log('Deleted report:', reportId);
      }
    } catch (error) {
      setReportsError('Failed to delete report');
      throw error;
    } finally {
      setReportsLoading(false);
    }
  };

  const handleDeleteProject = () => {
    if (window.confirm(t('projects:messages.confirm_delete'))) {
      if (onDeleteProject) {
        onDeleteProject(project.id);
      }
      navigate('/projects');
    }
  };

  const handleGenerateReport = async () => {
    try {
      await generateReport({ 
        name: `Project Report ${new Date().toLocaleDateString()}`,
        type: 'progress'
      });
    } catch (error) {
      console.error('Failed to generate report:', error);
    }
  };

  const handleDeleteReport = async (reportId) => {
    if (window.confirm(t('projects:reports.confirm_delete'))) {
      try {
        await deleteReport(reportId);
      } catch (error) {
        console.error('Failed to delete report:', error);
      }
    }
  };

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
                {formatDate(project.createdAt)}
              </div>
            </div>

            <div className="row mb-3">
              <div className="col-sm-3">
                <strong>{t('common:updated_at')}:</strong>
              </div>
              <div className="col-sm-9">
                {formatDate(project.updatedAt)}
              </div>
            </div>
          </Card.Body>
          <Card.Footer>
            <div className="d-flex gap-2">
              <Button
                variant="warning"
                icon="fas fa-edit"
                onClick={() => navigate(`/projects/${project.id}/edit`)}
              >
                {t('common:buttons.edit')}
              </Button>
              <Button
                variant="outline-danger"
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
                <div className="spinner-border spinner-border-sm text-primary" role="status">
                  <span className="visually-hidden">{t('common:states.loading')}</span>
                </div>
              </div>
            ) : reportsError ? (
              <Alert variant="danger">
                <i className="fas fa-exclamation-triangle me-2"></i>
                {reportsError}
              </Alert>
            ) : reports.length === 0 ? (
              <Alert variant="info">
                <i className="fas fa-info-circle me-2"></i>
                {t('projects:reports.no_reports')}
              </Alert>
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
                        variant="outline-primary"
                        icon="fas fa-download"
                        onClick={() => window.open(`/api/projectreport/download/${report.id}`)}
                      >
                        {t('common:buttons.download')}
                      </Button>
                      <Button
                        size="sm"
                        variant="outline-danger"
                        icon="fas fa-trash"
                        onClick={() => handleDeleteReport(report.id)}
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
              onClick={handleGenerateReport}
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
  project: PropTypes.shape({
    id: PropTypes.number.isRequired,
    name: PropTypes.string.isRequired,
    description: PropTypes.string.isRequired,
    finished: PropTypes.bool,
    createdAt: PropTypes.string,
    updatedAt: PropTypes.string
  }).isRequired,
  onDeleteProject: PropTypes.func
};

export default ProjectDetailsTab;
