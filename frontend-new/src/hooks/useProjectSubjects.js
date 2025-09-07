import { useState, useEffect } from 'react';
import ProjectService from '../services/ProjectService.js';

export const useProjectSubjects = (projectId) => {
  const [subjects, setSubjects] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const fetchSubjects = () => {
    if (!projectId) return Promise.resolve();
    
    setLoading(true);
    setError(null);
    return ProjectService.getSubjects(projectId)
      .then(setSubjects)
      .catch(err => setError(err.message))
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    fetchSubjects();
  }, [projectId]);

  return {
    subjects,
    loading,
    error,
    refetch: fetchSubjects
  };
};