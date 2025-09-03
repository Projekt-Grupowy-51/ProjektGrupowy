import { useState, useEffect, useCallback } from 'react';
import { useTranslation } from 'react-i18next';
import { useMultipleDataFetching, useNavigation, useAsyncOperation } from './common';
import ProjectService from '../services/ProjectService.js';
import AccessCodeService from '../services/AccessCodeService.js';
import SubjectVideoGroupAssignmentService from '../services/SubjectVideoGroupAssignmentService.js';

export const useLabelerVideoGroups = () => {
  const [accessCode, setAccessCode] = useState('');
  const [expandedProjects, setExpandedProjects] = useState({});
  const [success, setSuccess] = useState('');

  const { goTo } = useNavigation();
  const { t } = useTranslation(['labeler', 'common']);

  const operations = {
    projects: useCallback(() => ProjectService.getAll(), []),
    assignments: useCallback(() => SubjectVideoGroupAssignmentService.getAll(), [])
  };

  const { data, loading, error, refetchAll } = useMultipleDataFetching(operations);

  const { execute: validateAccessCode, loading: validatingCode, error: validationError } = useAsyncOperation();

  useEffect(() => {
    if (!data.projects) return;
    setExpandedProjects(Object.fromEntries(data.projects.map(p => [p.id, false])));
  }, [data.projects]);

  const handleJoinProject = useCallback(async () => {
    const code = accessCode.trim();
    if (!code) return;

    try {
      const isValid = await validateAccessCode(() => AccessCodeService.validateCode({ code }));
      if (!isValid) return;

      setSuccess(t('labeler:join_project.success'));
      setAccessCode('');
      await refetchAll();
    } catch (err) {
      console.error('Failed to join project:', err);
    }
  }, [accessCode, validateAccessCode, refetchAll, t]);

  const toggleProjectExpand = useCallback((projectId) => {
    setExpandedProjects(prev => ({ ...prev, [projectId]: !prev[projectId] }));
  }, []);

  const getProjectAssignments = useCallback(
    (projectId) => (data.assignments || []).filter(a => a.projectId === projectId),
    [data.assignments]
  );

  const handleAssignmentClick = useCallback(
    (assignment) => goTo(`/video-labeling/${assignment.id}`),
    [goTo]
  );

  const isLoading = loading.projects || loading.assignments || validatingCode;

  const errorMessage = validationError
    ? t('labeler:join_project.invalid_code')
    : error.projects || error.assignments || (!accessCode.trim() && !validationError ? t('labeler:join_project.required_code') : '');

  return {
    projects: data.projects || [],
    assignments: data.assignments || [],
    loading: isLoading,
    accessCode,
    setAccessCode,
    expandedProjects,
    error: errorMessage,
    success,
    handleJoinProject,
    toggleProjectExpand,
    getProjectAssignments,
    handleAssignmentClick
  };
};
