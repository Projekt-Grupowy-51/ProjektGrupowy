import React, { useState, useEffect } from "react";
import httpClient from "../httpclient";
import DeleteButton from "../components/DeleteButton";
import DataTable from "../components/DataTable";
import "./css/ScientistProjects.css";
import NavigateButton from "../components/NavigateButton";
import { useTranslation } from 'react-i18next';

const Projects = () => {
  const [projects, setProjects] = useState([]);
  const { t } = useTranslation(['projects', 'common']);

  const fetchProjects = async () => {
    const response = await httpClient.get("/Project");
    setProjects(response.data.sort((a, b) => a.id - b.id));
  };

  const handleDeleteProject = async (projectId) => {
    await httpClient.delete(`/Project/${projectId}`);
    setProjects(prev => prev.filter(project => project.id !== projectId));
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

          {projects.length > 0 ? (
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
                          onClick={() => handleDeleteProject(project.id)}
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
