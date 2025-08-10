import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, useSearchParams } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Container, Card } from '../components/ui';
import LabelForm from '../components/forms/LabelForm';
import { FAKE_SUBJECTS, FAKE_LABELS, findById, addToCollection } from '../data/fakeData.js';

const LabelAdd = () => {
  const { t } = useTranslation(['labels', 'common']);
  const [searchParams] = useSearchParams();
  const subjectId = searchParams.get('subjectId');
  const navigate = useNavigate();
  
  // Stan komponentu
  const [loading, setLoading] = useState(false);
  const [subject, setSubject] = useState(null);
  const [subjectLoading, setSubjectLoading] = useState(true);
  const [subjectError, setSubjectError] = useState(null);
  const [error, setError] = useState(null);

  useEffect(() => {
    // Symulacja ładowania subject z kolekcji
    const loadSubject = async () => {
      setSubjectLoading(true);
      await new Promise(resolve => setTimeout(resolve, 500));
      
      const foundSubject = findById(FAKE_SUBJECTS, subjectId);
      if (foundSubject) {
        setSubject(foundSubject);
      } else {
        setSubjectError('Subject not found');
      }
      setSubjectLoading(false);
    };
    
    if (subjectId) {
      loadSubject();
    }
  }, [subjectId]);

  const createLabel = async (labelData) => {
    setLoading(true);
    // Symulacja dodawania do kolekcji
    await new Promise(resolve => setTimeout(resolve, 1000));
    
    const newLabel = addToCollection(FAKE_LABELS, {
      ...labelData,
      subjectId: parseInt(subjectId),
      createdAt: new Date().toISOString()
    });
    
    console.log('Created label:', newLabel);
    console.log('Current labels collection:', FAKE_LABELS);
    setLoading(false);
  };

  const handleSubmit = async (formData) => {
    try {
      const labelData = {
        ...formData,
        subjectId: parseInt(subjectId)
      };
      await createLabel(labelData);
      navigate(`/subjects/${subjectId}`);
    } catch (error) {
      console.error('Error creating label:', error);
      // Here you could add a toast notification or error handling
    }
  };

  const handleCancel = () => {
    navigate(`/subjects/${subjectId}`);
  };

  if (error || subjectError) {
    return (
      <Container className="py-4">
        <Card>
          <Card.Body>
            <div className="alert alert-danger" role="alert">
              {error || subjectError}
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
                {t('labels:add_title')}
              </Card.Title>
              {subject && (
                <p className="text-muted mb-0">
                  {t('labels:for_subject')}: {subject.name}
                </p>
              )}
            </Card.Header>
            <Card.Body>
              <LabelForm
                subjectId={parseInt(subjectId)}
                subjectName={subject?.name}
                onSubmit={handleSubmit}
                onCancel={handleCancel}
                loading={loading}
                submitText={t('labels:buttons.create')}
              />
            </Card.Body>
          </Card>
        </Container.Col>
      </Container.Row>
    </Container>
  );
};

export default LabelAdd;
