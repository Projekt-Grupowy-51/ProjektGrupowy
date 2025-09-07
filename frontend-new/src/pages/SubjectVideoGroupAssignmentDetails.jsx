import React from 'react';
import { useTranslation } from 'react-i18next';
import { Container, Card, Button, Alert, Table, TableHead, TableBody, TableRow, TableCell } from '../components/ui';
import { LoadingSpinner, ErrorAlert } from '../components/common';
import { useSubjectVideoGroupAssignmentDetails } from '../hooks/useSubjectVideoGroupAssignmentDetails.js';

const SubjectVideoGroupAssignmentDetails = () => {
  const { t } = useTranslation(['assignments', 'common']);
  const {
    assignmentDetails,
    subject,
    videoGroup,
    labelers,
    loading,
    error,
    handleDelete,
    handleRefresh,
    handleBack
  } = useSubjectVideoGroupAssignmentDetails();

  if (loading) {
    return (
      <Container>
        <LoadingSpinner message={t('common:loading')} />
      </Container>
    );
  }

  if (error) {
    return (
      <Container>
        <ErrorAlert error={error} />
      </Container>
    );
  }

  if (!assignmentDetails) {
    return (
      <Container>
        <ErrorAlert error={t('assignments:errors.assignment_not_found')} />
      </Container>
    );
  }

  return (
    <Container className="py-4">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h1 className="h2 mb-0">
          {t('assignments:details.title', { id: assignmentDetails.id })}
        </h1>
        <div className="d-flex gap-2">
          <Button 
            variant="outline-danger" 
            icon="fas fa-trash"
            confirmAction={true}
            confirmTitle="Potwierdź usunięcie"
            confirmMessage={`Czy na pewno chcesz usunąć przypisanie #${assignmentDetails.id}? Ta operacja jest nieodwracalna.`}
            confirmText="Usuń"
            cancelText="Anuluj"
            onConfirm={handleDelete}
          >
            {t('common:buttons.delete')}
          </Button>
          <Button variant="outline-secondary" onClick={handleRefresh} icon="fas fa-sync-alt">
            {t('common:buttons.refresh')}
          </Button>
          <Button variant="secondary" onClick={handleBack} icon="fas fa-arrow-left">
            {t('common:buttons.back')}
          </Button>
        </div>
      </div>

      <Card className="mb-4">
        <Card.Header>
          <Card.Title level={4} className="mb-0">
            {t('assignments:details.subject')}
          </Card.Title>
        </Card.Header>
        <Card.Body>
          {subject ? (
            <Table>
              <TableBody>
                <TableRow>
                  <TableCell><strong>{t('assignments:details.name')}</strong></TableCell>
                  <TableCell>{subject.name}</TableCell>
                </TableRow>
                <TableRow>
                  <TableCell><strong>{t('assignments:details.description')}</strong></TableCell>
                  <TableCell>{subject.description}</TableCell>
                </TableRow>
              </TableBody>
            </Table>
          ) : (
            <Alert variant="warning">
              {t('assignments:errors.load_subject')}
            </Alert>
          )}
        </Card.Body>
      </Card>

      <Card className="mb-4">
        <Card.Header>
          <Card.Title level={4} className="mb-0">
            {t('assignments:details.video_group')}
          </Card.Title>
        </Card.Header>
        <Card.Body>
          {videoGroup ? (
            <Table>
              <TableBody>
                <TableRow>
                  <TableCell><strong>{t('assignments:details.name')}</strong></TableCell>
                  <TableCell>{videoGroup.name}</TableCell>
                </TableRow>
                <TableRow>
                  <TableCell><strong>{t('assignments:details.description')}</strong></TableCell>
                  <TableCell>{videoGroup.description}</TableCell>
                </TableRow>
              </TableBody>
            </Table>
          ) : (
            <Alert variant="warning">
              {t('assignments:errors.load_video_group')}
            </Alert>
          )}
        </Card.Body>
      </Card>

      <Card>
        <Card.Header>
          <Card.Title level={4} className="mb-0">
            {t('assignments:details.assigned_labelers')}
          </Card.Title>
        </Card.Header>
        <Card.Body>
          {labelers.length > 0 ? (
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>#</TableCell>
                  <TableCell>{t('assignments:columns.name')}</TableCell>
                  <TableCell>{t('assignments:columns.email')}</TableCell>
                  <TableCell>{t('assignments:columns.role')}</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {labelers.map((labeler, index) => (
                  <TableRow key={labeler.id}>
                    <TableCell>{index + 1}</TableCell>
                    <TableCell>{labeler.name}</TableCell>
                    <TableCell>{labeler.email}</TableCell>
                    <TableCell>
                      <span className="badge bg-success">{labeler.role}</span>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          ) : (
            <div className="text-center py-5">
              <i className="fas fa-user-slash fs-1 text-muted opacity-50"></i>
              <p className="text-muted mt-3 mb-0">
                {t('assignments:details.no_labelers')}
              </p>
            </div>
          )}
        </Card.Body>
      </Card>
    </Container>
  );
};

export default SubjectVideoGroupAssignmentDetails;