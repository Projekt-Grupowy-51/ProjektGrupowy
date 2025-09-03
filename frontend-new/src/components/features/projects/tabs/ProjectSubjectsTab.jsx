import React from 'react';
import PropTypes from 'prop-types';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';
import { Card, Button } from '../../../ui';
import { EmptyState, TabHeader, TabListGroup } from '../../../common';

const ProjectSubjectsTab = ({ projectId, subjects = [] }) => {
  const { t } = useTranslation(['common', 'projects']);
  const navigate = useNavigate();

  const renderSubjectItem = (subject) => (
    <>
      <div>
        <div className="fw-bold">{subject.name}</div>
        <small className="text-muted">{subject.email}</small>
      </div>
      <div className="d-flex gap-1">
        <Button
          size="sm"
          variant="info"
          icon="fas fa-eye"
          onClick={() => navigate(`/subjects/${subject.id}`)}
        >
          {t('common:buttons.details')}
        </Button>
      </div>
    </>
  );

  return (
    <Card>
      <TabHeader
        icon="fas fa-folder"
        title={t('projects:tabs.subjects')}
        actionText={t('projects:add.subject')}
        onAction={() => navigate(`/subjects/add?projectId=${projectId}`)}
      />
      <Card.Body>
        {subjects.length === 0 ? (
          <EmptyState
            icon="fas fa-book"
            message={t('projects:not_found.subject')}
            actionText={t('projects:add.subject')}
            onAction={() => navigate(`/subjects/add?projectId=${projectId}`)}
          />
        ) : (
          <TabListGroup items={subjects} renderItem={renderSubjectItem} />
        )}
      </Card.Body>
    </Card>
  );
};

ProjectSubjectsTab.propTypes = {
  projectId: PropTypes.oneOfType([PropTypes.string, PropTypes.number]).isRequired,
  subjects: PropTypes.array
};

export default ProjectSubjectsTab;
