import { useCallback, useEffect, useState } from 'react';
import { Project } from '../models/Project';
import { getProject } from '../services/api/projectService';

const useProject = (id?: number) => {
  const [project, setProject] = useState<Project | null>(null);

  const fetchProject = useCallback(async () => {
    if (!id) return;
    const data = await getProject(id);
    setProject(data);
  }, [id]);

  useEffect(() => {
    fetchProject();
  }, [fetchProject]);

  return { project, setProject, fetchProject };
};

export default useProject;
