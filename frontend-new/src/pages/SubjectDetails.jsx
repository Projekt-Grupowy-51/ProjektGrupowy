import React from 'react';
import { useTranslation } from 'react-i18next';
import { Container, Card, Button, Table } from '../components/ui';
import { LoadingSpinner, ErrorAlert, EmptyState, PageHeader, DetailPageActions, DetailSection } from '../components/common';
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
    deleteSubject,
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
      <PageHeader
        title={subject.name}
        subtitle={subject.description}
        icon="fas fa-book"
        actions={
          <DetailPageActions
            onEdit={handleEditSubject}
            onDelete={deleteSubject}
            onBack={handleBackToProject}
            editText={t('common:buttons.edit')}
            deleteText={t('common:buttons.delete')}
            backText={t('common:buttons.back')}
            deleteConfirmTitle={t('common:deleteConfirmation.title')}
            deleteConfirmMessage={t('subjects:confirm_delete_subject', { name: subject.name })}
          />
        }
      />

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

      <DetailSection
        title={t('subjects:labels.title')}
        icon="fas fa-tags"
        actions={
          <Button
            variant="primary"
            icon="fas fa-plus"
            onClick={handleAddLabel}
          >
            {t('subjects:labels.add')}
          </Button>
        }
      />

      {labelsLoading ? (
        <LoadingSpinner message={t('subjects:labels.loading')} size="small" />
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
                        confirmAction={true}
                        confirmTitle={t('common:deleteConfirmation.title')}
                        confirmMessage={t('subjects:confirm_delete_label', { name: label.name })}
                        confirmText={t('common:deleteConfirmation.confirm')}
                        cancelText={t('common:deleteConfirmation.cancel')}
                        onConfirm={() => deleteLabel(label.id)}
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
          title={t('subjects:labels.no_labels')}
          message={t('subjects:labels.empty')}
          actionText={t('subjects:labels.add')}
          onAction={handleAddLabel}
        />
      )}
    </Container>
  );
};

export default SubjectDetails;
