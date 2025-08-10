import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Container, Card, Button, Tabs, Alert } from '../components/ui';
import ProjectDetailsTab from '../components/features/projects/tabs/ProjectDetailsTab.jsx';
import ProjectSubjectsTab from '../components/features/projects/tabs/ProjectSubjectsTab.jsx';
import ProjectVideosTab from '../components/features/projects/tabs/ProjectVideosTab.jsx';
import ProjectAssignmentsTab from '../components/features/projects/tabs/ProjectAssignmentsTab.jsx';
import ProjectLabelersTab from '../components/features/projects/tabs/ProjectLabelersTab.jsx';
import ProjectAccessCodesTab from '../components/features/projects/tabs/ProjectAccessCodesTab.jsx';
import { FAKE_PROJECTS, FAKE_SUBJECTS, FAKE_VIDEO_GROUPS, FAKE_ASSIGNMENTS, FAKE_USERS, FAKE_ACCESS_CODES, findById, findByProperty, removeFromCollection } from '../data/fakeData.js';

const ProjectDetails = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { t } = useTranslation(['common', 'projects']);
  
  // Stan komponentu
  const [projects, setProjects] = useState([...FAKE_PROJECTS]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [project, setProject] = useState(null);
  const [activeTab, setActiveTab] = useState('details');

  const getProject = async (projectId) => {
    // Pobieranie z kolekcji
    await new Promise(resolve => setTimeout(resolve, 300));
    return findById(FAKE_PROJECTS, projectId);
  };

  const deleteProject = async (projectId) => {
    // Usuwanie z kolekcji
    await new Promise(resolve => setTimeout(resolve, 500));
    const deletedProject = removeFromCollection(FAKE_PROJECTS, projectId);
    if (deletedProject) {
      console.log('Deleted project:', projectId);
      console.log('Current projects collection:', FAKE_PROJECTS);
    }
  };

  // Save active tab to localStorage whenever it changes
  const handleTabChange = (tabId) => {
    setActiveTab(tabId);
    localStorage.setItem(`projectDetails_${id}_activeTab`, tabId);
  };

  // Load saved tab when component mounts or id changes
  useEffect(() => {
    const savedTab = localStorage.getItem(`projectDetails_${id}_activeTab`);
    if (savedTab) {
      setActiveTab(savedTab);
    }
  }, [id]);

  useEffect(() => {
    const fetchProject = async () => {
      try {
        const projectData = await getProject(id);
        setProject(projectData);
      } catch (err) {
        console.error('Error fetching project:', err);
      }
    };
    if (id) {
      fetchProject();
    }
  }, [id, getProject]);

  const handleBackToList = () => {
    navigate('/projects');
  };

  const handleDeleteProject = async (projectId) => {
    try {
      await deleteProject(projectId);
      navigate('/projects');
    } catch (error) {
      console.error('Failed to delete project:', error);
    }
  };

  if (loading) {
    return (
      <Container>
        <div className="d-flex justify-content-center py-5">
          <div className="spinner-border text-primary" role="status">
            <span className="visually-hidden">{t('common:states.loading')}</span>
          </div>
        </div>
      </Container>
    );
  }

  if (error) {
    return (
      <Container>
        <Alert variant="danger">
          <i className="fas fa-exclamation-triangle me-2"></i>
          {error}
        </Alert>
      </Container>
    );
  }

  if (!project) {
    return (
      <Container>
        <Alert variant="warning">
          <i className="fas fa-exclamation-triangle me-2"></i>
          {t('projects:messages.not_found')}
        </Alert>
      </Container>
    );
  }

  const tabs = [
    {
      id: 'details',
      label: t('projects:tabs.details'),
      icon: 'fas fa-info-circle',
      content: <ProjectDetailsTab project={project} onDeleteProject={handleDeleteProject} />
    },
    {
      id: 'subjects',
      label: t('projects:tabs.subjects'),
      icon: 'fas fa-book',
      badge: findByProperty(FAKE_SUBJECTS, 'projectId', project.id).length,
      content: <ProjectSubjectsTab projectId={project.id} subjects={findByProperty(FAKE_SUBJECTS, 'projectId', project.id)} />
    },
    {
      id: 'videos',
      label: t('projects:tabs.videos'),
      icon: 'fas fa-video',
      badge: findByProperty(FAKE_VIDEO_GROUPS, 'projectId', project.id).length,
      content: <ProjectVideosTab projectId={project.id} videoGroups={findByProperty(FAKE_VIDEO_GROUPS, 'projectId', project.id)} />
    },
    {
      id: 'assignments',
      label: t('projects:tabs.assignments'),
      icon: 'fas fa-tasks',
      badge: findByProperty(FAKE_ASSIGNMENTS, 'projectId', project.id).length,
      content: <ProjectAssignmentsTab projectId={project.id} assignments={findByProperty(FAKE_ASSIGNMENTS, 'projectId', project.id)} />
    },
    {
      id: 'labelers',
      label: t('projects:tabs.labelers'),
      icon: 'fas fa-users',
      badge: findByProperty(FAKE_USERS, 'role', 'labeler').length,
      content: <ProjectLabelersTab projectId={project.id} labelers={findByProperty(FAKE_USERS, 'role', 'labeler')} />
    },
    {
      id: 'access-codes',
      label: t('projects:tabs.access_codes'),
      icon: 'fas fa-key',
      badge: findByProperty(FAKE_ACCESS_CODES, 'projectId', project.id).length,
      content: <ProjectAccessCodesTab projectId={project.id} accessCodes={findByProperty(FAKE_ACCESS_CODES, 'projectId', project.id)} />
    }
  ];

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
          onClick={handleBackToList}
        >
          {t('common:buttons.back')}
        </Button>
      </div>

      <Card>
        <Card.Body className="p-0">
          <Tabs
            tabs={tabs}
            activeTab={activeTab}
            onTabChange={handleTabChange}
          />
        </Card.Body>
      </Card>
    </Container>
  );
};

export default ProjectDetails;
