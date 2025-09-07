import { useState, useEffect } from 'react';
import { useParams, useLocation, useNavigate } from 'react-router-dom';
import ProjectService from '../services/ProjectService.js';

export const useProjectEdit = () => {
  const { id } = useParams();
  const location = useLocation();
  const navigate = useNavigate();
  const projectId = parseInt(id, 10);

  const [project, setProject] = useState(null);
  const [loading, setLoading] = useState(false);
  const [submitLoading, setSubmitLoading] = useState(false);
  const [error, setError] = useState(null);

  const fetchProject = () => {
    setLoading(true);
    setError(null);
    return ProjectService.getById(projectId)
      .then(setProject)
      .catch(err => setError(err.message))
      .finally(() => setLoading(false));
  };

  const handleSubmit = (formData) => {
    setSubmitLoading(true);
    setError(null);
    const redirectPath = location.state?.from || `/projects/${projectId}`;
    return ProjectService.update(projectId, formData)
      .then(() => navigate(redirectPath))
      .catch(err => setError(err.message))
      .finally(() => setSubmitLoading(false));
  };

  const handleCancel = () => window.history.back();

  useEffect(() => {
    fetchProject();
  }, [projectId]);

  return {
    id: projectId,
    project,
    loading,
    submitLoading,
    error,
    handleSubmit,
    handleCancel
  };
};
