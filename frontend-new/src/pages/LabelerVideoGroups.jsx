import React from 'react';
import { useTranslation } from 'react-i18next';
import { Container, Card, Button, Input, Alert, Table, TableHead, TableBody, TableRow, TableCell } from '../components/ui';
import { LoadingSpinner } from '../components/common';
import { useLabelerVideoGroups } from '../hooks/useLabelerVideoGroups.js';

const LabelerVideoGroups = () => {
  const { t } = useTranslation(['labeler', 'common']);
  const {
    projects,
    assignments,
    loading,
    accessCode,
    setAccessCode,
    expandedProjects,
    error,
    success,
    handleJoinProject,
    toggleProjectExpand,
    getProjectAssignments,
    handleAssignmentClick
  } = useLabelerVideoGroups();

  if (loading) {
    return (
      <Container>
        <LoadingSpinner message={t('common:loading')} />
      </Container>
    );
  }

  return (
    <Container className="py-4">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h1 className="h2">{t('labeler:title')}</h1>
        <div className="d-flex gap-2" style={{ maxWidth: '400px' }}>
          <Input
            name="accessCode"
            type="text"
            placeholder={t('labeler:join_project.placeholder')}
            value={accessCode}
            onChange={(e) => setAccessCode(e.target.value)}
            onKeyPress={(e) => e.key === 'Enter' && handleJoinProject()}
          />
          <Button
            onClick={handleJoinProject}
            variant="primary"
            disabled={loading}
            icon="fas fa-plus-circle"
          >
            {t('labeler:join_project.button')}
          </Button>
        </div>
      </div>

      {error && (
        <Alert variant="danger" className="mb-4">
          <i className="fas fa-exclamation-triangle me-2"></i>
          {error}
        </Alert>
      )}

      {success && (
        <Alert variant="success" className="mb-4">
          <i className="fas fa-check-circle me-2"></i>
          {success}
        </Alert>
      )}

      {projects.length > 0 ? (
        <div className="projects-container">
          {projects.map((project) => (
            <Card key={project.id} className="mb-4">
              <Card.Header>
                <div
                  className="d-flex justify-content-between align-items-center"
                  style={{ cursor: 'pointer' }}
                  onClick={() => toggleProjectExpand(project.id)}
                >
                  <Card.Title level={5} className="mb-0">
                    {project.name}
                  </Card.Title>
                  <Button variant="light" size="sm">
                    <i
                      className={`fas fa-chevron-${
                        expandedProjects[project.id] ? 'up' : 'down'
                      }`}
                    ></i>
                  </Button>
                </div>
              </Card.Header>
              {expandedProjects[project.id] && (
                <Card.Body>
                  <h6 className="mt-3 mb-3">
                    {t('labeler:projects.assignments')}:
                  </h6>
                  {getProjectAssignments(project.id).length > 0 ? (
                    <Table>
                      <TableHead>
                        <TableRow>
                          <TableCell>#</TableCell>
                          <TableCell>{t('labeler:projects.columns.subject')}</TableCell>
                          <TableCell>{t('labeler:projects.columns.video_group')}</TableCell>
                          <TableCell>{t('common:actions')}</TableCell>
                        </TableRow>
                      </TableHead>
                      <TableBody>
                        {getProjectAssignments(project.id).map((assignment, index) => (
                          <TableRow key={assignment.id}>
                            <TableCell>{index + 1}</TableCell>
                            <TableCell>{assignment.subjectName || t('common:states.unknown')}</TableCell>
                            <TableCell>{assignment.videoGroupName || t('common:states.unknown')}</TableCell>
                            <TableCell>
                              <Button
                                size="sm"
                                variant="outline"
                                onClick={() => handleAssignmentClick(assignment)}
                                icon="fas fa-play"
                              >
                                {t('labeler:start_labeling')}
                              </Button>
                            </TableCell>
                          </TableRow>
                        ))}
                      </TableBody>
                    </Table>
                  ) : (
                    <Alert variant="info">
                      <i className="fas fa-info-circle me-2"></i>
                      {t('labeler:projects.no_assignments')}
                    </Alert>
                  )}
                </Card.Body>
              )}
            </Card>
          ))}
        </div>
      ) : (
        <Alert variant="info" className="text-center">
          <i className="fas fa-info-circle me-2"></i>
          {t('labeler:projects.no_projects')}
        </Alert>
      )}
    </Container>
  );
};

export default LabelerVideoGroups;