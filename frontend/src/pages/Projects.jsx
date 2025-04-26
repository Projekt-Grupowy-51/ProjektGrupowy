import React, { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import httpClient from "../httpclient";
import DeleteButton from "../components/DeleteButton";
import DataTable from "../components/DataTable";
import "./css/ScientistProjects.css";
import NavigateButton from "../components/NavigateButton";
import { useNotification } from "../context/NotificationContext";
import { useTranslation } from 'react-i18next';

const Projects = () => {
  const [projects, setProjects] = useState([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();
  const location = useLocation();
  const { addNotification } = useNotification();
  const { t } = useTranslation(['projects', 'common']);

  useEffect(() => {
    if (location.state?.success) {
      addNotification(location.state.success, "success");
    }
  }, [location.state]);

  const fetchProjects = async () => {
    try {
      const response = await httpClient.get("/Project");
      setProjects(response.data.sort((a, b) => a.id - b.id));
    } catch (error) {
      addNotification(t('projects:notifications.load_error'), "error");
    } finally {
      setLoading(false);
    }
  };

  const handleDeleteProject = async (projectId) => {
    try {
      await httpClient.delete(`/Project/${projectId}`);
      setProjects(prev => prev.filter(project => project.id !== projectId));
      addNotification(t('projects:notifications.delete_success'), "success");
    } catch (error) {
      addNotification(t('projects:notifications.load_error'), "error");
    }
  };

  useEffect(() => {
    fetchProjects();
  }, []);

  const columns = [
    { field: "name", header: t('projects:form.name') },
    { field: "description", header: t('projects:form.description') }
  ];

  return (
      <div className="container">
        <div className="content">
          <div className="d-flex justify-content-between align-items-center mb-4">
            <h1 className="heading">{t('projects:list_title')}</h1>
            <NavigateButton actionType = 'Add' path="/projects/add" title={t('common:buttons.add')} />
          </div>

          {loading ? (
              <div className="text-center py-5">
                <div className="spinner-border text-primary" role="status">
                  <span className="visually-hidden">{t('common:loading')}</span>
                </div>
                <p className="mt-3">{t('projects:buttons.loading')}</p>
              </div>
          ) : projects.length > 0 ? (
              <DataTable
                  showRowNumbers={true}
                  columns={columns}
                  data={projects}
                  navigateButton={(project) => (
                      <NavigateButton
                          path={`/projects/${project.id}`}
                          actionType='Details'
                          value={t('common:buttons.details')}
                      />
                  )}
                  deleteButton={(project) => (
                      <DeleteButton
                          onConfirm={() => handleDeleteProject(project.id)}
                          itemName={project.name}
                      />
                  )}
              />
          ) : (
              <div className="alert alert-info text-center">
                <i className="fas fa-info-circle me-2"></i>
                {t('projects:no_projects')}
              </div>
          )}
        </div>
      </div>
  );
};

export default Projects;