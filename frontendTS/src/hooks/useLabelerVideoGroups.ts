import { useCallback, useEffect, useState } from 'react';
import { Project } from '../models/Project';
import { Assignment } from '../models/Assignment';
import { getProjects, joinProject } from '../services/api/projectService';
import { getAssignments } from '../services/api/assignmentService';
import { useNotification } from '../context/NotificationContext';
import { useTranslation } from 'react-i18next';

export default function useLabelerVideoGroups() {
  const [projects, setProjects] = useState<Project[]>([]);
  const [assignments, setAssignments] = useState<Assignment[]>([]);
  const [loading, setLoading] = useState(true);
  const [accessCode, setAccessCode] = useState('');
  const [expandedProjects, setExpandedProjects] = useState<Record<number, boolean>>({});
  const { addNotification } = useNotification();
  const { t } = useTranslation(['labeler', 'common']);

  const fetchAssignments = useCallback(async () => {
    try {
      const data = await getAssignments();
      setAssignments(data);
    } catch (error: any) {
      addNotification(error.response?.data?.message || t('labeler:errors.load_assignments'), 'error');
    } finally {
      setLoading(false);
    }
  }, [addNotification, t]);

  const fetchProjects = useCallback(async () => {
    setLoading(true);
    try {
      const data = await getProjects();
      setProjects(data);
      const expanded: Record<number, boolean> = {};
      data.forEach(p => { expanded[p.id!] = false; });
      setExpandedProjects(expanded);
      await fetchAssignments();
    } catch (error: any) {
      addNotification(error.response?.data?.message || t('labeler:errors.load_projects'), 'error');
      setLoading(false);
    }
  }, [addNotification, t, fetchAssignments]);

  useEffect(() => {
    fetchProjects();
  }, [fetchProjects]);

  const handleJoinProject = async () => {
    if (!accessCode.trim()) {
      addNotification(t('labeler:join_project.required_code'), 'error');
      return;
    }
    try {
      await joinProject(accessCode.trim());
      addNotification(t('labeler:join_project.success'), 'success');
      setAccessCode('');
      fetchProjects();
    } catch (error: any) {
      addNotification(error.response?.data?.message || t('labeler:join_project.invalid_code'), 'error');
    }
  };

  const toggleProjectExpand = (projectId: number) => {
    setExpandedProjects(prev => ({ ...prev, [projectId]: !prev[projectId] }));
  };

  const getProjectAssignments = (projectId: number) => {
    return assignments.filter(a => a.projectId === projectId);
  };

  return {
    projects,
    assignments,
    loading,
    accessCode,
    setAccessCode,
    expandedProjects,
    toggleProjectExpand,
    getProjectAssignments,
    handleJoinProject,
  };
}
