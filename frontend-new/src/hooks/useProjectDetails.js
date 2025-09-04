import { useState, useEffect, useCallback, useMemo } from 'react';
import { useParams, useLocation } from 'react-router-dom';
import { useDataFetching, useAsyncOperation, useNavigation } from './common';
import ProjectService from '../services/ProjectService.js';
import { useProjectStats } from './useProjectStats.js';

export const useProjectDetails = (passedProjectId) => {
  const { id } = useParams();
  const location = useLocation();
  const projectId = passedProjectId ? parseInt(passedProjectId, 10) : parseInt(id, 10);
  const { goTo } = useNavigation();
  const [activeTab, setActiveTab] = useState('details');

  const fetchProject = useCallback(
    () => ProjectService.getById(projectId),
    [projectId]
  );
  const { data: project, loading, error } = useDataFetching(fetchProject, [projectId]);
  const { stats, refetchStats } = useProjectStats(projectId);

  useEffect(() => {
    // If user came from projects list, always start with 'details' tab
    // Otherwise, restore the last active tab from localStorage
    const cameFromProjectsList = location.state?.from === '/projects';
    
    if (cameFromProjectsList) {
      setActiveTab('details');
    } else {
      const savedTab = localStorage.getItem(`projectDetails_${projectId}_activeTab`);
      if (savedTab) setActiveTab(savedTab);
    }
  }, [projectId, location.state]);

  const handleTabChange = useCallback(
    (tabId) => {
      setActiveTab(tabId);
      localStorage.setItem(`projectDetails_${projectId}_activeTab`, tabId);
    },
    [projectId]
  );

  const { execute: deleteProject, loading: deleting, error: deleteError } = useAsyncOperation();

  const handleDeleteProject = useCallback(async () => {
    try {
      await deleteProject(() => ProjectService.delete(projectId));
      goTo('/projects');
    } catch (err) {
      console.error('Failed to delete project:', err);
    }
  }, [deleteProject, goTo, projectId]);

  const handleBackToList = useCallback(() => goTo('/projects'), [goTo]);

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
    error: error || deleteError,
    deleting,
    activeTab,
    tabs,
    handleTabChange,
    handleBackToList,
    handleDeleteProject,
    refetchStats
  };
};
