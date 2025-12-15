import { useCallback, useEffect, useState } from 'react';
import { Project } from '../models/Project';
import { getProjects } from '../services/api/projectService';

const useProjects = () => {
  const [projects, setProjects] = useState<Project[]>([]);

  const fetchProjects = useCallback(async () => {
    const data = await getProjects();
    setProjects(data.sort((a, b) => a.id! - b.id!));
  }, []);

  useEffect(() => {
    fetchProjects();
  }, [fetchProjects]);

  return { projects, setProjects, fetchProjects };
};

export default useProjects;
