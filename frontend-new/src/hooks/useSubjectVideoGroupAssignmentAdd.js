import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import SubjectVideoGroupAssignmentService from '../services/SubjectVideoGroupAssignmentService.js';
import ProjectService from '../services/ProjectService.js';

export const useSubjectVideoGroupAssignmentAdd = () => {
  const { projectId: projectIdParam } = useParams();
  const navigate = useNavigate();
  const projectId = projectIdParam ? parseInt(projectIdParam, 10) : null;

  const [subjects, setSubjects] = useState([]);
  const [videoGroups, setVideoGroups] = useState([]);
  const [loading, setLoading] = useState(false);
  const [submitLoading, setSubmitLoading] = useState(false);
  const [error, setError] = useState(null);

  const fetchData = () => {
    if (!projectId) return Promise.resolve();
    
    setLoading(true);
    setError(null);
    
    return Promise.all([
      ProjectService.getSubjects(projectId),
      ProjectService.getVideoGroups(projectId)
    ])
      .then(([subjectsData, videoGroupsData]) => {
        setSubjects(subjectsData || []);
        setVideoGroups(videoGroupsData || []);
      })
      .catch(err => setError(err.message))
      .finally(() => setLoading(false));
  };

  const handleSubmit = (assignmentData) => {
    if (!projectId) {
      setError('Project ID not found');
      return Promise.reject(new Error('Project ID not found'));
    }

    setSubmitLoading(true);
    setError(null);
    return SubjectVideoGroupAssignmentService.create(assignmentData)
      .then(() => navigate(`/projects/${projectId}`))
      .catch(err => setError(err.message))
      .finally(() => setSubmitLoading(false));
  };

  const handleCancel = () => navigate(`/projects/${projectId}`);

  useEffect(() => {
    fetchData();
  }, [projectId]);

  return {
    projectId,
    subjects,
    videoGroups,
    loading,
    error,
    submitLoading,
    handleSubmit,
    handleCancel
  };
};
