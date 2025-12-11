import { useState, useEffect, useMemo } from 'react';
import { useParams, useLocation, useNavigate } from 'react-router-dom';
import ProjectService from '../services/ProjectService.js';
import { useProjectStats } from './useProjectStats.js';

export const useProjectDetails = (passedProjectId) => {
  const { id } = useParams();
  const location = useLocation();
  const navigate = useNavigate();
  const projectId = passedProjectId ? parseInt(passedProjectId, 10) : parseInt(id, 10);
  
  const [project, setProject] = useState(null);
  const [loading, setLoading] = useState(false);
  const [deleting, setDeleting] = useState(false);
  const [error, setError] = useState(null);
  const [activeTab, setActiveTab] = useState('details');
  
  const { stats, refetchStats } = useProjectStats(projectId);

  const fetchProject = () => {
    setLoading(true);
    setError(null);
    return ProjectService.getById(projectId)
      .then(setProject)
      .catch(err => setError(err.message))
      .finally(() => setLoading(false));
  };

  const handleDeleteProject = () => {
    setDeleting(true);
    setError(null);
    return ProjectService.delete(projectId)
      .then(() => navigate('/projects'))
      .catch(err => setError(err.message))
      .finally(() => setDeleting(false));
  };

  const handleTabChange = (tabId) => {
    setActiveTab(tabId);
    localStorage.setItem(`projectDetails_${projectId}_activeTab`, tabId);
  };

  useEffect(() => {
    fetchProject();
  }, [projectId]);

  useEffect(() => {
    const cameFromProjectsList = location.state?.from === '/projects';
    
    if (cameFromProjectsList) {
      setActiveTab('details');
    } else {
      const savedTab = localStorage.getItem(`projectDetails_${projectId}_activeTab`);
      if (savedTab) setActiveTab(savedTab);
    }
  }, [projectId, location.state]);

  const tabs = useMemo(() => project ? [
    { id: 'details', label: 'Details', icon: 'fas fa-info-circle', data: { projectId, onDeleteProject: handleDeleteProject } },
    { id: 'subjects', label: 'Subjects', icon: 'fas fa-book', badge: stats?.subjects || 0, data: { projectId } },
    { id: 'videos', label: 'Videos', icon: 'fas fa-video', badge: stats?.videos || 0, data: { projectId } },
    { id: 'assignments', label: 'Assignments', icon: 'fas fa-tasks', badge: stats?.assignments || 0, data: { projectId } },
    { id: 'labelers', label: 'Labelers', icon: 'fas fa-users', badge: stats?.labelers || 0, data: { projectId } },
    { id: 'access_codes', label: 'Access Codes', icon: 'fas fa-key', badge: stats?.access_codes || 0, data: { projectId } }
  ] : [], [project, projectId, handleDeleteProject, stats]);

  return {
    id: projectId,
    project,
    loading,
    error,
    deleting,
    activeTab,
    tabs,
    handleTabChange,
    handleBackToList: () => navigate('/projects'),
    handleDeleteProject,
    refetchStats
  };
};
