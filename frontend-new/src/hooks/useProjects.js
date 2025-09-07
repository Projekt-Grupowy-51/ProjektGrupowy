import { useState, useEffect } from 'react';
import ProjectService from '../services/ProjectService.js';

export const useProjects = () => {
  const [projects, setProjects] = useState([]);
  const [loading, setLoading] = useState(false);
  const [deleteLoading, setDeleteLoading] = useState(false);
  const [error, setError] = useState(null);

  const fetchProjects = () => {
    setLoading(true);
    setError(null);
    return ProjectService.getAll()
      .then(setProjects)
      .catch(err => setError(err.message))
      .finally(() => setLoading(false));
  };

  const handleDelete = (id) => {
    setDeleteLoading(true);
    setError(null);
    return ProjectService.delete(id)
      .then(() => fetchProjects())
      .catch(err => setError(err.message))
      .finally(() => setDeleteLoading(false));
  };

  useEffect(() => {
    fetchProjects();
  }, []);

  return {
    projects,
    loading,
    error,
    deleteLoading,
    handleDelete,
    refetch: fetchProjects,
  };
};
