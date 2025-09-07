import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import ProjectService from '../services/ProjectService.js';

export const useProjectAdd = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const handleSubmit = (projectData) => {
    setLoading(true);
    setError(null);
    return ProjectService.create(projectData)
      .then(() => navigate('/projects'))
      .catch(err => setError(err.message))
      .finally(() => setLoading(false));
  };

  const handleCancel = () => navigate('/projects');

  return {
    handleSubmit,
    handleCancel,
    loading,
    error
  };
};
