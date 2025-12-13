import { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';
import ProjectService from '../services/ProjectService.js';
import AccessCodeService from '../services/AccessCodeService.js';
import SubjectVideoGroupAssignmentService from '../services/SubjectVideoGroupAssignmentService.js';

export const useLabelerVideoGroups = () => {
  const [accessCode, setAccessCode] = useState('');
  const [expandedProjects, setExpandedProjects] = useState({});
  const [success, setSuccess] = useState('');
  
  const [projects, setProjects] = useState([]);
  const [assignments, setAssignments] = useState([]);
  const [loading, setLoading] = useState(false);
  const [joiningProject, setJoiningProject] = useState(false);
  const [error, setError] = useState(null);

  const navigate = useNavigate();
  const { t } = useTranslation(['labeler', 'common']);

  const fetchData = () => {
    setLoading(true);
    setError(null);

    return Promise.all([
      ProjectService.getAll(),
      SubjectVideoGroupAssignmentService.getLabelerAssignments()
    ])
      .then(([projectsData, assignmentsData]) => {
        setProjects(projectsData || []);
        setAssignments(assignmentsData || []);
      })
      .catch(err => setError(err.message))
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    fetchData();
  }, []);

  useEffect(() => {
    if (!projects.length) return;
    setExpandedProjects(Object.fromEntries(projects.map(p => [p.id, false])));
  }, [projects]);

  const handleJoinProject = async () => {
    const code = accessCode.trim();
    if (!code) return;

    setJoiningProject(true);
    setError(null);
    try {
      await AccessCodeService.joinProject({ code });
      setSuccess(t('labeler:join_project.success'));
      setAccessCode('');
      await fetchData();
    } catch (err) {
      setError(t('labeler:join_project.invalid_code'));
    } finally {
      setJoiningProject(false);
    }
  };

  const toggleProjectExpand = (projectId) => {
    setExpandedProjects(prev => ({ ...prev, [projectId]: !prev[projectId] }));
  };

  const getProjectAssignments = (projectId) =>
    assignments.filter(a => a.projectId === projectId);

  const handleAssignmentClick = (assignment) =>
    navigate(`/video-labeling/${assignment.id}`);

  const handleToggleCompletion = async (assignmentId, isCompleted) => {
    try {
      await SubjectVideoGroupAssignmentService.toggleCompletion(assignmentId, isCompleted);

      // Update local state
      setAssignments(prev =>
        prev.map(a =>
          a.id === assignmentId
            ? { ...a, isCompleted }
            : a
        )
      );
    } catch (err) {
      setError(err.message);
    }
  };

  return {
    projects,
    assignments,
    loading: loading || joiningProject,
    accessCode,
    setAccessCode,
    expandedProjects,
    error,
    success,
    handleJoinProject,
    toggleProjectExpand,
    getProjectAssignments,
    handleAssignmentClick,
    handleToggleCompletion
  };
};
