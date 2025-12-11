import { useState, useMemo } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import SubjectService from '../services/SubjectService.js';

export const useSubjectAdd = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const projectId = useMemo(() => {
    const id = new URLSearchParams(location.search).get('projectId');
    return id ? parseInt(id, 10) : null;
  }, [location.search]);

  const handleSubmit = (subjectData) => {
    if (!projectId) {
      setError('Project ID not specified');
      return Promise.reject(new Error('Project ID not specified'));
    }
    
    setLoading(true);
    setError(null);
    const subjectRequest = { ...subjectData, projectId };
    
    return SubjectService.create(subjectRequest)
      .then(() => navigate(`/projects/${projectId}`))
      .catch(err => setError(err.message))
      .finally(() => setLoading(false));
  };

  const handleCancel = () => window.history.back();

  return {
    projectId,
    handleSubmit,
    handleCancel,
    loading,
    error
  };
};
