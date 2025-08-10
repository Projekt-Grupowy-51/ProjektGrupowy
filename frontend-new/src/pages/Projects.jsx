import React, { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { Container, Card, Button } from '../components/ui';
import ProjectList from '../components/features/projects/ProjectList.jsx';
import { FAKE_PROJECTS, removeFromCollection } from '../data/fakeData.js';

const ProjectsPage = () => {
  const { t } = useTranslation(['projects', 'common']);
  
  // Stan komponentu
  const [projects, setProjects] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    // Symulacja ładowania projektów z kolekcji
    const loadProjects = async () => {
      setLoading(true);
      await new Promise(resolve => setTimeout(resolve, 1000));
      setProjects([...FAKE_PROJECTS]); // Kopiujemy kolekcję do stanu
      setLoading(false);
    };
    
    loadProjects();
  }, []);

  const deleteProject = async (projectId) => {
    // Usuwanie z kolekcji
    await new Promise(resolve => setTimeout(resolve, 500));
    
    const deletedProject = removeFromCollection(FAKE_PROJECTS, projectId);
    if (deletedProject) {
      setProjects([...FAKE_PROJECTS]); // Aktualizujemy stan
      console.log('Deleted project:', projectId);
      console.log('Current projects collection:', FAKE_PROJECTS);
    }
  };

  const handleDeleteProject = async (projectId) => {
    try {
      await deleteProject(projectId);
      // Success handled by the hook
    } catch (error) {
      console.error('Failed to delete project:', error);
      // Here you could add a toast notification
    }
  };

  if (loading) {
    return (
      <Container className="py-4">
        <div className="text-center">
          <div className="spinner-border" role="status">
            <span className="visually-hidden">Loading...</span>
          </div>
        </div>
      </Container>
    );
  }

  if (error) {
    return (
      <Container className="py-4">
        <Card>
          <Card.Body>
            <div className="alert alert-danger" role="alert">
              {error}
            </div>
          </Card.Body>
        </Card>
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
          onClick={() => window.location.href = '/projects/add'}
        >
          {t('common:buttons.add')}
        </Button>
      </div>

      <ProjectList 
        projects={projects}
        onDelete={handleDeleteProject}
      />
    </Container>
  );
};

export default ProjectsPage;
