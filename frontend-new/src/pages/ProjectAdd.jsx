import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Container, Card } from '../components/ui';
import ProjectForm from '../components/forms/ProjectForm.jsx';
import { FAKE_PROJECTS, addToCollection } from '../data/fakeData.js';

const ProjectAddPage = () => {
  const navigate = useNavigate();
  const { t } = useTranslation(['projects', 'common']);
  
  // Stan komponentu
  const [loading, setLoading] = useState(false);
  
  const createProject = async (projectData) => {
    setLoading(true);
    // Symulacja dodawania do kolekcji
    await new Promise(resolve => setTimeout(resolve, 1000));
    
    const newProject = addToCollection(FAKE_PROJECTS, {
      ...projectData,
      createdAt: new Date().toISOString(),
      status: 'active'
    });
    
    console.log('Created project:', newProject);
    console.log('Current projects collection:', FAKE_PROJECTS);
    setLoading(false);
  };

  const handleSubmit = async (projectData) => {
    try {
      await createProject(projectData);
      navigate('/projects');
    } catch (error) {
      console.error('Failed to create project:', error);
      // Here you could add a toast notification or error handling
    }
  };

  const handleCancel = () => {
    navigate('/projects');
  };

  return (
    <Container className="py-4">
      <Container.Row className="justify-content-center">
        <Container.Col lg={8}>
          <Card className="shadow-sm">
            <Card.Header variant="primary">
              <Card.Title level={1} className="mb-0">
                {t('projects:add_title')}
              </Card.Title>
            </Card.Header>
            <Card.Body>
              <ProjectForm
                onSubmit={handleSubmit}
                onCancel={handleCancel}
                loading={loading}
                submitText={t('projects:buttons.create')}
              />
            </Card.Body>
          </Card>
        </Container.Col>
      </Container.Row>
    </Container>
  );
};

export default ProjectAddPage;
