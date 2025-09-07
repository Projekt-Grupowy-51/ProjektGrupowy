import React from 'react';
import { useTranslation } from 'react-i18next';
import { Container, Card, Button } from '../components/ui';
import { LoadingSpinner, ErrorAlert } from '../components/common';
import ProjectList from '../components/features/projects/ProjectList.jsx';
import { useProjects } from '../hooks/useProjects.js';
import { useNavigate } from 'react-router-dom';

const ProjectsPage = () => {
  const { t } = useTranslation(['projects', 'common']);
  const navigate = useNavigate();
  
  const {
    projects,
    loading,
    error,
    handleDelete
  } = useProjects();

  if (loading) {
    return (
      <Container className="py-4">
        <LoadingSpinner />
      </Container>
    );
  }

  if (error) {
    return (
      <Container className="py-4">
        <ErrorAlert error={error} />
      </Container>
    );
  }

  return (
    <Container className="py-4">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h1 className="heading">{t('projects:list_title')}</h1>
        <Button
          variant="primary"
          icon="fas fa-plus-circle"
          onClick={() => navigate('/projects/add')}
        >
          {t('common:buttons.add')}
        </Button>
      </div>

      <ProjectList 
        projects={projects || []}
        onDelete={handleDelete}
      />
    </Container>
  );
};

export default ProjectsPage;
