import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Container, Card, Alert, Button } from '../components/ui';
import SubjectForm from '../components/forms/SubjectForm.jsx';
import { FAKE_SUBJECTS, findById, updateInCollection } from '../data/fakeData.js';

const SubjectEditPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { t } = useTranslation(['subjects', 'common']);
  
  // Stan komponentu
  const [subject, setSubject] = useState(null);
  const [subjectLoading, setSubjectLoading] = useState(true);
  const [subjectError, setSubjectError] = useState(null);
  const [updateLoading, setUpdateLoading] = useState(false);

  useEffect(() => {
    // Ładowanie subject z kolekcji
    const loadSubject = async () => {
      setSubjectLoading(true);
      await new Promise(resolve => setTimeout(resolve, 500));
      
      const foundSubject = findById(FAKE_SUBJECTS, id);
      if (foundSubject) {
        setSubject(foundSubject);
      } else {
        setSubjectError('Subject not found');
      }
      setSubjectLoading(false);
    };
    
    if (id) {
      loadSubject();
    }
  }, [id]);

  const updateSubject = async (subjectId, subjectData) => {
    setUpdateLoading(true);
    // Symulacja aktualizacji w kolekcji
    await new Promise(resolve => setTimeout(resolve, 1000));
    
    const updatedSubject = updateInCollection(FAKE_SUBJECTS, subjectId, subjectData);
    if (updatedSubject) {
      console.log('Updated subject:', updatedSubject);
      console.log('Current subjects collection:', FAKE_SUBJECTS);
    }
    setUpdateLoading(false);
  };

  const handleSubmit = async (subjectData) => {
    try {
      await updateSubject(id, subjectData);
      navigate(`/subjects/${id}`);
    } catch (error) {
      console.error('Failed to update subject:', error);
      // Here you could add a toast notification or error handling
    }
  };

  const handleCancel = () => {
    navigate(`/subjects/${id}`);
  };

  const handleBackToDetails = () => {
    navigate(`/subjects/${id}`);
  };

  if (subjectLoading) {
    return (
      <Container className="py-4">
        <div className="d-flex justify-content-center">
          <div className="spinner-border text-primary" role="status">
            <span className="visually-hidden">{t('common:states.loading')}</span>
          </div>
        </div>
      </Container>
    );
  }

  if (subjectError) {
    return (
      <Container className="py-4">
        <Alert variant="danger">
          <i className="fas fa-exclamation-triangle me-2"></i>
          {subjectError}
        </Alert>
        <div className="mt-3">
          <Button
            variant="outline-secondary"
            icon="fas fa-arrow-left"
            onClick={handleBackToDetails}
          >
            {t('common:buttons.back')}
          </Button>
        </div>
      </Container>
    );
  }

  if (!subject) {
    return (
      <Container className="py-4">
        <Alert variant="warning">
          <i className="fas fa-exclamation-triangle me-2"></i>
          {t('subjects:messages.not_found')}
        </Alert>
        <div className="mt-3">
          <Button
            variant="outline-secondary"
            icon="fas fa-arrow-left"
            onClick={handleBackToDetails}
          >
            {t('common:buttons.back')}
          </Button>
        </div>
      </Container>
    );
  }

  return (
    <Container className="py-4">
      <Container.Row className="justify-content-center">
        <Container.Col lg={8}>
          <div className="d-flex justify-content-between align-items-center mb-4">
            <h1 className="mb-0">
              <i className="fas fa-edit me-2"></i>
              {t('subjects:edit_title')}
            </h1>
            <Button
              variant="outline-secondary"
              icon="fas fa-arrow-left"
              onClick={handleBackToDetails}
            >
              {t('common:buttons.back')}
            </Button>
          </div>

          <Card className="shadow-sm">
            <Card.Header variant="warning">
              <Card.Title level={2} className="mb-0">
                <i className="fas fa-book me-2"></i>
                {subject.name}
              </Card.Title>
            </Card.Header>
            <Card.Body>
              <SubjectForm
                projectId={subject.projectId}
                initialData={subject}
                onSubmit={handleSubmit}
                onCancel={handleCancel}
                loading={updateLoading}
                submitText={t('common:buttons.save')}
                isEdit={true}
              />
            </Card.Body>
          </Card>
        </Container.Col>
      </Container.Row>
    </Container>
  );
};

export default SubjectEditPage;
