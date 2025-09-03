import React from 'react';
import { useTranslation } from 'react-i18next';
import { Container, Card, Button, Table } from '../components/ui';
import { LoadingSpinner, ErrorAlert, EmptyState } from '../components/common';
import { useSubjectDetails } from '../hooks/useSubjectDetails.js';

const SubjectDetails = () => {
  const { t } = useTranslation(['subjects', 'common']);
  const {
    id,
    subject,
    labels,
    subjectLoading,
    labelsLoading,
    subjectError,
    labelsError,
    deleteLabel,
    handleBackToProject,
    handleAddLabel,
    handleEditSubject,
    handleEditLabel
  } = useSubjectDetails();

  if (subjectLoading) {
    return (
      <Container className="py-4">
        <LoadingSpinner message={t('common:states.loading')} />
      </Container>
    );
  }

  if (subjectError) {
    return (
      <Container className="py-4">
        <ErrorAlert error={subjectError} />
      </Container>
    );
  }

  if (!subject) {
    return (
      <Container className="py-4">
        <ErrorAlert error={t('subjects:messages.not_found')} />
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
            onClick={handleEditSubject}
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
        <LoadingSpinner message="Loading labels..." size="small" />
      ) : labelsError ? (
        <ErrorAlert error={labelsError} />
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
                        onClick={() => handleEditLabel(label.id)}
                      >
                        {t('common:buttons.edit')}
                      </Button>
                      <Button
                        size="sm"
                        variant="outline-danger"
                        icon="fas fa-trash"
                        onClick={() => deleteLabel(label.id, t('subjects:labels.confirm_delete'))}
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
        <EmptyState
          icon="fas fa-tags"
          title="No labels found"
          message={t('subjects:labels.empty')}
          actionText={t('subjects:labels.add')}
          onAction={handleAddLabel}
        />
      )}
    </Container>
  );
};

export default SubjectDetails;
