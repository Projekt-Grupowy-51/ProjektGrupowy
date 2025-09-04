import React from 'react';
import { useTranslation } from 'react-i18next';
import { Container, Card, Button, Tabs } from '../components/ui';
import { LoadingSpinner, ErrorAlert } from '../components/common';
import ProjectDetailsTab from '../components/features/projects/tabs/ProjectDetailsTab.jsx';
import ProjectSubjectsTab from '../components/features/projects/tabs/ProjectSubjectsTab.jsx';
import ProjectVideosTab from '../components/features/projects/tabs/ProjectVideosTab.jsx';
import ProjectAssignmentsTab from '../components/features/projects/tabs/ProjectAssignmentsTab.jsx';
import ProjectLabelersTab from '../components/features/projects/tabs/ProjectLabelersTab.jsx';
import ProjectAccessCodesTab from '../components/features/projects/tabs/ProjectAccessCodesTab.jsx';
import { useProjectDetails } from '../hooks/useProjectDetails.js';

const ProjectDetails = () => {
  const { t } = useTranslation(['common', 'projects']);
  const {
    project,
    loading,
    error,
    activeTab,
    tabs,
    handleTabChange,
    handleBackToList,
  } = useProjectDetails();

  // Tab component mapping
  const tabComponents = {
    'details': ProjectDetailsTab,
    'subjects': ProjectSubjectsTab,
    'videos': ProjectVideosTab,
    'assignments': ProjectAssignmentsTab,
    'labelers': ProjectLabelersTab,
    'access_codes': ProjectAccessCodesTab
  };

  if (loading) {
    return (
      <Container>
        <LoadingSpinner message={t('common:states.loading')} />
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

  if (!project) {
    return (
      <Container>
        <ErrorAlert error={t('projects:messages.not_found')} />
      </Container>
    );
  }

  const tabsWithContent = tabs.map(tab => {
    const Component = tabComponents[tab.id];
    return {
      ...tab,
      label: t(`projects:tabs.${tab.id}`),
      content: Component ? <Component {...tab.data} /> : null
    };
  });

  return (
    <Container>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h2>
            <i className="fas fa-project-diagram me-2"></i>
            {project.name}
          </h2>
          <p className="text-muted mb-0">{project.description}</p>
        </div>
        <Button
          variant="outline-secondary"
          icon="fas fa-arrow-left"
          onClick={() => handleBackToList()}
        >
          {t('common:buttons.back')}
        </Button>
      </div>

      <Card>
        <Card.Body className="p-0">
          <Tabs
            tabs={tabsWithContent}
            activeTab={activeTab}
            onTabChange={handleTabChange}
          />
        </Card.Body>
      </Card>
    </Container>
  );
};

export default ProjectDetails;
