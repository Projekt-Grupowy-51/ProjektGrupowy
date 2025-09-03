import { useState, useEffect, useCallback, useMemo } from 'react';
import { useParams } from 'react-router-dom';
import { useDataFetching, useAsyncOperation, useNavigation } from './common';
import ProjectService from '../services/ProjectService.js';

export const useProjectDetails = () => {
  const { id } = useParams();
  const projectId = parseInt(id, 10);
  const { goTo } = useNavigation();
  const [activeTab, setActiveTab] = useState('details');

  const fetchProject = useCallback(
    () => ProjectService.getById(projectId),
    [projectId]
  );
  const { data: project, loading, error } = useDataFetching(fetchProject, [projectId]);

  useEffect(() => {
    const savedTab = localStorage.getItem(`projectDetails_${projectId}_activeTab`);
    if (savedTab) setActiveTab(savedTab);
  }, [projectId]);

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
    { id: 'details', label: 'Details', icon: 'fas fa-info-circle', data: { project, onDeleteProject: handleDeleteProject } },
    { id: 'subjects', label: 'Subjects', icon: 'fas fa-book', badge: 0, data: { projectId } },
    { id: 'videos', label: 'Videos', icon: 'fas fa-video', badge: 0, data: { projectId } },
    { id: 'assignments', label: 'Assignments', icon: 'fas fa-tasks', badge: 0, data: { projectId } },
    { id: 'labelers', label: 'Labelers', icon: 'fas fa-users', badge: 0, data: { projectId } },
    { id: 'access_codes', label: 'Access Codes', icon: 'fas fa-key', badge: 0, data: { projectId } }
  ] : [], [project, projectId, handleDeleteProject]);

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
    handleDeleteProject
  };
};
