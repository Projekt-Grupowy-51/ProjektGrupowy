import React, { useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Container, Card } from '../components/ui';
import SubjectForm from '../components/forms/SubjectForm.jsx';
import { FAKE_SUBJECTS, addToCollection } from '../data/fakeData.js';

const SubjectAddPage = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const { t } = useTranslation(['subjects', 'common']);
  
  // Stan komponentu
  const [loading, setLoading] = useState(false);
  
  // Get projectId from URL params
  const projectId = new URLSearchParams(location.search).get("projectId");

  const createSubject = async (subjectData) => {
    setLoading(true);
    // Symulacja dodawania do kolekcji
    await new Promise(resolve => setTimeout(resolve, 1000));
    
    const newSubject = addToCollection(FAKE_SUBJECTS, {
      ...subjectData,
      projectId: parseInt(projectId)
    });
    
    console.log('Created subject:', newSubject);
    console.log('Current subjects collection:', FAKE_SUBJECTS);
    setLoading(false);
  };

  const handleSubmit = async (subjectData) => {
    try {
      await createSubject(subjectData);
      navigate(`/projects/${projectId}`);
    } catch (error) {
      console.error('Failed to create subject:', error);
      // Here you could add a toast notification or error handling
    }
  };

  const handleCancel = () => {
    navigate(`/projects/${projectId}`);
  };

  return (
    <Container className="py-4">
      <Container.Row className="justify-content-center">
        <Container.Col lg={8}>
          <Card className="shadow-sm">
            <Card.Header variant="primary">
              <Card.Title level={1} className="mb-0">
                {t('subjects:add_title')}
              </Card.Title>
            </Card.Header>
            <Card.Body>
              <SubjectForm
                projectId={projectId}
                onSubmit={handleSubmit}
                onCancel={handleCancel}
                loading={loading}
                submitText={t('subjects:buttons.add')}
              />
            </Card.Body>
          </Card>
        </Container.Col>
      </Container.Row>
    </Container>
  );
};

export default SubjectAddPage;
