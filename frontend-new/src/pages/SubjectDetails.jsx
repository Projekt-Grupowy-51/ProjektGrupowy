import React from 'react';
import { useTranslation } from 'react-i18next';
import { useSubjectDetails } from '../hooks/useSubjectDetails.js';
import { Card, Button, Table } from '../components/ui';
import { LoadingSpinner, ErrorAlert, EmptyState } from '../components/common';

const SubjectDetails = () => {
  const { t } = useTranslation(['subjects', 'common']);
  const {
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

  if (subjectLoading) return <LoadingSpinner />;
  if (subjectError) return <ErrorAlert error={subjectError} />;
  if (!subject)
    return (
      <EmptyState
        icon="fas fa-book"
        message={t('subjects:messages.not_found')}
        actionText={t('common:buttons.back')}
        onAction={handleBackToProject}
      />
    );

  return (
    <div className="row justify-content-center">
      <div className="col-lg-10">
        {/* SUBJECT DETAILS */}
        <Card className="mb-4">
          <Card.Header>
            <Card.Title level={5}>
              <i className="fas fa-book me-2"></i>
              {t('subjects:details.title')}
            </Card.Title>
          </Card.Header>
          <Card.Body>
            <div className="row mb-3">
              <div className="col-sm-3">
                <strong>{t('subjects:form.name')}:</strong>
              </div>
              <div className="col-sm-9">{subject.name}</div>
            </div>

            <div className="row mb-3">
              <div className="col-sm-3">
                <strong>{t('subjects:details.description')}:</strong>
              </div>
              <div className="col-sm-9">{subject.description || '-'}</div>
            </div>

            <div className="row mb-3">
              <div className="col-sm-3">
                <strong>ID:</strong>
              </div>
              <div className="col-sm-9">{subject.id}</div>
            </div>
          </Card.Body>
          <Card.Footer>
            <div className="d-flex gap-2">
              <Button
                size="sm"
                variant="outline-secondary"
                icon="fas fa-arrow-left"
                onClick={handleBackToProject}
              >
                {t('common:buttons.back')}
              </Button>

              <Button
                size="sm"
                variant="outline-warning"
                icon="fas fa-edit"
                onClick={handleEditSubject}
              >
                {t('common:buttons.edit')}
              </Button>

              <Button
                size="sm"
                variant="outline-danger"
                icon="fas fa-trash"
                confirmAction
                confirmTitle={t('common:deleteConfirmation.title')}
                confirmMessage={t('subjects:confirm_delete_subject', { name: subject.name })}
                confirmText={t('common:deleteConfirmation.confirm')}
                cancelText={t('common:deleteConfirmation.cancel')}
                onConfirm={deleteSubject}
              >
                {t('common:buttons.delete')}
              </Button>
            </div>
          </Card.Footer>
        </Card>

        {/* LABELS UNDER DETAILS */}
        <Card>
          <Card.Header>
            <div className="d-flex justify-content-between align-items-center">
              <Card.Title level={5}>
                <i className="fas fa-tags me-2"></i>
                {t('subjects:labels.title')}
              </Card.Title>
              <Button
                variant="primary"
                size="sm"
                icon="fas fa-plus"
                onClick={handleAddLabel}
              >
                {t('subjects:labels.add')}
              </Button>
            </div>
          </Card.Header>

          <Card.Body>
            {labelsLoading ? (
              <LoadingSpinner message={t('subjects:labels.loading')} />
            ) : labelsError ? (
              <ErrorAlert error={labelsError} />
            ) : labels.length === 0 ? (
              <EmptyState
                icon="fas fa-tags"
                message={t('subjects:labels.empty')}
                actionText={t('subjects:labels.add')}
                onAction={handleAddLabel}
              />
            ) : (
              <div className="table-responsive">
                <Table striped hover>
                  <Table.Head>
                    <Table.Row>
                      <Table.Cell header>#</Table.Cell>
                      <Table.Cell header>{t('subjects:columns.name')}</Table.Cell>
                      <Table.Cell header>{t('subjects:columns.shortcut')}</Table.Cell>
                      <Table.Cell header>{t('subjects:columns.color')}</Table.Cell>
                      <Table.Cell header>{t('common:actions')}</Table.Cell>
                    </Table.Row>
                  </Table.Head>
                  <Table.Body>
                    {labels.map((label, index) => (
                      <Table.Row key={label.id}>
                        <Table.Cell>{index + 1}</Table.Cell>
                        <Table.Cell>{label.name}</Table.Cell>
                        <Table.Cell>
                          <span className="badge bg-primary rounded-pill">
                            {label.shortcut}
                          </span>
                        </Table.Cell>
                        <Table.Cell>
                          <div
                            style={{
                              backgroundColor: label.colorHex,
                              width: '20px',
                              height: '20px',
                              borderRadius: '3px',
                              border: '1px solid var(--color-border)',
                              display: 'inline-block'
                            }}
                          ></div>
                        </Table.Cell>
                        <Table.Cell>
                          <div className="d-flex gap-2">
                            <Button
                              size="sm"
                              variant="outline-warning"
                              icon="fas fa-edit"
                              onClick={() => handleEditLabel(label.id)}
                            >
                              {t('common:buttons.edit')}
                            </Button>
                            <Button
                              size="sm"
                              variant="outline-danger"
                              icon="fas fa-trash"
                              confirmAction
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
              </div>
            )}
          </Card.Body>
        </Card>
      </div>
    </div>
  );
};

export default SubjectDetails;
