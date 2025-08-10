import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Container, Card, Button, Table, Alert } from '../components/ui';
import { FAKE_SUBJECTS, FAKE_LABELS, findById, findByProperty, removeFromCollection } from '../data/fakeData.js';

const SubjectDetails = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { t } = useTranslation(['subjects', 'common']);
  
  // Stan komponentu
  const [subject, setSubject] = useState(null);
  const [subjectLoading, setSubjectLoading] = useState(true);
  const [subjectError, setSubjectError] = useState(null);
  const [labels, setLabels] = useState([]);
  const [labelsLoading, setLabelsLoading] = useState(true);
  const [labelsError, setLabelsError] = useState(null);

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
    
    // Ładowanie labels dla tego subject
    const loadSubjectLabels = async () => {
      setLabelsLoading(true);
      await new Promise(resolve => setTimeout(resolve, 300));
      
      const subjectLabels = findByProperty(FAKE_LABELS, 'subjectId', parseInt(id));
      setLabels(subjectLabels);
      setLabelsLoading(false);
    };
    
    if (id) {
      loadSubject();
      loadSubjectLabels();
    }
  }, [id]);

  const deleteLabel = async (labelId) => {
    // Usuwanie z kolekcji
    await new Promise(resolve => setTimeout(resolve, 500));
    
    const deletedLabel = removeFromCollection(FAKE_LABELS, labelId);
    if (deletedLabel) {
      // Aktualizacja lokalnego stanu
      setLabels(prev => prev.filter(l => l.id !== labelId));
      console.log('Deleted label:', labelId);
      console.log('Current labels collection:', FAKE_LABELS);
    }
  };

  const handleDeleteLabel = async (labelId) => {
    if (window.confirm(t('subjects:labels.confirm_delete'))) {
      try {
        await deleteLabel(labelId);
      } catch (error) {
        console.error('Failed to delete label:', error);
      }
    }
  };

  const handleBackToProject = () => {
    if (subject?.projectId) {
      navigate(`/projects/${subject.projectId}`);
    }
  };

  const handleAddLabel = () => {
    navigate(`/labels/add?subjectId=${id}`);
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
      </Container>
    );
  }

  return (
    <Container className="py-4">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h1>
            <i className="fas fa-book me-2"></i>
            {subject.name}
          </h1>
          <p className="text-muted mb-0">{subject.description}</p>
        </div>
        <div className="d-flex gap-2">
          <Button
            variant="warning"
            icon="fas fa-edit"
            onClick={() => navigate(`/subjects/${id}/edit`)}
          >
            {t('common:buttons.edit')}
          </Button>
          <Button
            variant="outline-secondary"
            icon="fas fa-arrow-left"
            onClick={handleBackToProject}
          >
            {t('common:buttons.back')}
          </Button>
        </div>
      </div>

      <Card className="mb-4">
        <Card.Header variant="info">
          <Card.Title level={5} className="mb-0">
            <i className="fas fa-info-circle me-2"></i>
            {t('subjects:details.title')}
          </Card.Title>
        </Card.Header>
        <Card.Body>
          <Card.Text>
            <strong>{t('subjects:details.description')}:</strong> {subject.description}
          </Card.Text>
        </Card.Body>
      </Card>

      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2 className="section-title mb-0">
          <i className="fas fa-tags me-2"></i>
          {t('subjects:labels.title')}
        </h2>
        <Button
          variant="primary"
          icon="fas fa-plus"
          onClick={handleAddLabel}
        >
          {t('subjects:labels.add')}
        </Button>
      </div>

      {labelsLoading ? (
        <div className="text-center py-3">
          <div className="spinner-border text-primary" role="status">
            <span className="visually-hidden">{t('common:states.loading')}</span>
          </div>
        </div>
      ) : labelsError ? (
        <Alert variant="danger">
          <i className="fas fa-exclamation-triangle me-2"></i>
          {labelsError}
        </Alert>
      ) : labels.length > 0 ? (
        <Card>
          <Table striped hover responsive>
            <Table.Head>
              <Table.Row>
                <Table.Cell header={true}>#</Table.Cell>
                <Table.Cell header={true}>{t('subjects:columns.name')}</Table.Cell>
                <Table.Cell header={true}>{t('subjects:columns.shortcut')}</Table.Cell>
                <Table.Cell header={true}>{t('subjects:columns.color')}</Table.Cell>
                <Table.Cell header={true}>{t('common:actions')}</Table.Cell>
              </Table.Row>
            </Table.Head>
            <Table.Body>
              {labels.map((label, index) => (
                <Table.Row key={label.id}>
                  <Table.Cell>{index + 1}</Table.Cell>
                  <Table.Cell>{label.name}</Table.Cell>
                  <Table.Cell>
                    <span className="badge bg-secondary">{label.shortcut}</span>
                  </Table.Cell>
                  <Table.Cell>
                    <div
                      style={{
                        backgroundColor: label.colorHex,
                        width: "20px",
                        height: "20px",
                        display: "inline-block",
                        borderRadius: "3px",
                        border: "1px solid #ddd"
                      }}
                    ></div>
                  </Table.Cell>
                  <Table.Cell>
                    <div className="d-flex gap-2">
                      <Button
                        size="sm"
                        variant="warning"
                        icon="fas fa-edit"
                        onClick={() => navigate(`/labels/${label.id}/edit`)}
                      >
                        {t('common:buttons.edit')}
                      </Button>
                      <Button
                        size="sm"
                        variant="outline-danger"
                        icon="fas fa-trash"
                        onClick={() => handleDeleteLabel(label.id)}
                      >
                        {t('common:buttons.delete')}
                      </Button>
                    </div>
                  </Table.Cell>
                </Table.Row>
              ))}
            </Table.Body>
          </Table>
        </Card>
      ) : (
        <Alert variant="info">
          <i className="fas fa-info-circle me-2"></i>
          {t('subjects:labels.empty')}
        </Alert>
      )}
    </Container>
  );
};

export default SubjectDetails;
