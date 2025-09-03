import { useCallback } from 'react';
import { useDataFetching, useAsyncOperation } from './common';
import ProjectService from '../services/ProjectService.js';

export const useProjects = () => {
  const fetchProjects = useCallback(() => ProjectService.getAll(), []);

  const { data: projects = [], loading, error, refetch } =
    useDataFetching(fetchProjects, []);

  const { execute: executeDelete, loading: deleteLoading, error: deleteError } =
    useAsyncOperation();

  const handleDelete = async (id) => {
    await executeDelete(() => ProjectService.delete(id));
    await refetch();
  };

  return {
    projects,
    loading,
    error: error || deleteError,
    deleteLoading,
    handleDelete,
    refetch,
  };
};
