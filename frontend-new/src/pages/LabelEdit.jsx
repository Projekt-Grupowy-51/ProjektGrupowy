import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Container, Card } from '../components/ui';
import LabelForm from '../components/forms/LabelForm';
import { FAKE_SUBJECTS, FAKE_LABELS, findById, updateInCollection } from '../data/fakeData.js';

const LabelEdit = () => {
  const { t } = useTranslation(['labels', 'common']);
  const { id } = useParams();
  const navigate = useNavigate();
  
  // Stan komponentu
  const [updateLoading, setUpdateLoading] = useState(false);
  const [label, setLabel] = useState(null);
  const [subject, setSubject] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const getLabel = async (labelId) => {
    // Pobieranie z kolekcji
    await new Promise(resolve => setTimeout(resolve, 500));
    return findById(FAKE_LABELS, labelId);
  };

  const getSubject = async (subjectId) => {
    // Pobieranie z kolekcji
    await new Promise(resolve => setTimeout(resolve, 300));
    return findById(FAKE_SUBJECTS, subjectId);
  };

  const updateLabel = async (labelId, formData) => {
    setUpdateLoading(true);
    // Symulacja aktualizacji w kolekcji
    await new Promise(resolve => setTimeout(resolve, 1000));
    
    const updatedLabel = updateInCollection(FAKE_LABELS, labelId, formData);
    if (updatedLabel) {
      console.log('Updated label:', updatedLabel);
      console.log('Current labels collection:', FAKE_LABELS);
    }
    setUpdateLoading(false);
  };

  useEffect(() => {
    const loadData = async () => {
      try {
        setLoading(true);
        
        // First load label data to get subjectId
        const labelData = await getLabel(parseInt(id));
        setLabel(labelData);
        
        // Then load subject data using subjectId from label
        const subjectData = await getSubject(labelData.subjectId);
        setSubject(subjectData);
        
      } catch (error) {
        console.error('Error loading data:', error);
        setError(t('labels:errors.loadFailed'));
      } finally {
        setLoading(false);
      }
    };

    if (id) {
      loadData();
    }
  }, [id, t]);

  const handleSubmit = async (formData) => {
    try {
      await updateLabel(parseInt(id), formData);
      navigate(`/subjects/${label.subjectId}`);
    } catch (error) {
      console.error('Error updating label:', error);
      // Here you could add a toast notification
    }
  };

  const handleCancel = () => {
    navigate(`/subjects/${label?.subjectId}`);
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

  if (!label) {
    return (
      <Container className="py-4">
        <Card>
          <Card.Body>
            <div className="alert alert-warning" role="alert">
              {t('labels:messages.label_not_found')}
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
                {t('labels:edit_title')}
              </Card.Title>
              {subject && (
                <p className="text-muted mb-0">
                  {t('labels:for_subject')}: {subject.name}
                </p>
              )}
            </Card.Header>
            <Card.Body>
              <LabelForm
                initialData={label}
                onSubmit={handleSubmit}
                onCancel={handleCancel}
                loading={updateLoading}
                submitText={t('labels:buttons.save')}
              />
            </Card.Body>
          </Card>
        </Container.Col>
      </Container.Row>
    </Container>
  );
};

export default LabelEdit;
