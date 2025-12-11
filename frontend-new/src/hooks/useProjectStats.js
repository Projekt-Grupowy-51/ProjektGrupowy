import { useState, useEffect } from 'react';
import ProjectService from '../services/ProjectService.js';

export const useProjectStats = (projectId) => {
  const [stats, setStats] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const fetchStats = () => {
    if (!projectId) return Promise.resolve();
    
    setLoading(true);
    setError(null);
    return ProjectService.getStats(projectId)
      .then(setStats)
      .catch(err => setError(err.message))
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    fetchStats();
  }, [projectId]);

  return {
    stats,
    loading,
    error,
    refetchStats: fetchStats
  };
};