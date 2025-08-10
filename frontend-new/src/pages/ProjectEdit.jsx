import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Container, Card } from '../components/ui';
import ProjectForm from '../components/forms/ProjectForm.jsx';
import { FAKE_PROJECTS, findById, updateInCollection } from '../data/fakeData.js';

const ProjectEditPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { t } = useTranslation(['projects', 'common']);
  
  // Stan komponentu
  const [project, setProject] = useState(null);
  const [fetchLoading, setFetchLoading] = useState(true);
  const [updateLoading, setUpdateLoading] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    // Ładowanie projektu z kolekcji
    const loadProject = async () => {
      setFetchLoading(true);
      await new Promise(resolve => setTimeout(resolve, 500));
      
      const foundProject = findById(FAKE_PROJECTS, id);
      if (foundProject) {
        setProject(foundProject);
      } else {
        setError('Project not found');
      }
      setFetchLoading(false);
    };
    
    if (id) {
      loadProject();
    }
  }, [id]);

  const updateProject = async (projectId, projectData) => {
    setUpdateLoading(true);
    // Symulacja aktualizacji w kolekcji
    await new Promise(resolve => setTimeout(resolve, 1000));
    
    const updatedProject = updateInCollection(FAKE_PROJECTS, projectId, projectData);
    if (updatedProject) {
      console.log('Updated project:', updatedProject);
      console.log('Current projects collection:', FAKE_PROJECTS);
    }
    setUpdateLoading(false);
  };

  const handleSubmit = async (projectData) => {
    try {
      await updateProject(id, projectData);
      navigate(`/projects/${id}`);
    } catch (error) {
      console.error('Failed to update project:', error);
      // Here you could add a toast notification
    }
  };

  const handleCancel = () => {
    navigate(`/projects/${id}`);
  };

  if (fetchLoading) {
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

  if (!project) {
    return (
      <Container className="py-4">
        <Card>
          <Card.Body>
            <div className="alert alert-warning" role="alert">
              {t('projects:messages.project_not_found')}
            </div>
          </Card.Body>
        </Card>
      </Container>
    );
  }

  return (
    <Container className="py-4">
      <Container.Row className="justify-content-center">
        <Container.Col lg={8}>
          <Card className="shadow-sm">
            <Card.Header variant="primary">
              <Card.Title level={1} className="mb-0">
                {t('projects:edit_title')}
              </Card.Title>
            </Card.Header>
            <Card.Body>
              <ProjectForm
                initialData={project}
                onSubmit={handleSubmit}
                onCancel={handleCancel}
                loading={updateLoading}
                submitText={t('projects:buttons.save')}
              />
            </Card.Body>
          </Card>
        </Container.Col>
      </Container.Row>
    </Container>
  );
};

export default ProjectEditPage;
