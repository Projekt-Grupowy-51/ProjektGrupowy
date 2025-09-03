import { useCallback, useState, useMemo } from 'react';
import { useDataFetching } from './common';
import AccessCodeService from '../services/AccessCodeService.js';

export const useProjectAccessCodes = (projectIdRaw) => {
  const projectId = useMemo(() => parseInt(projectIdRaw, 10), [projectIdRaw]);
  const [visibleCodes, setVisibleCodes] = useState({});

  const fetchAccessCodes = useCallback(async () => {
    if (!projectId) return [];
    const codes = await AccessCodeService.getByProjectId(projectId);
    return codes.sort((a, b) => new Date(b.createdAtUtc || 0) - new Date(a.createdAtUtc || 0));
  }, [projectId]);

  const { data: accessCodes = [], loading, error, refetch } = useDataFetching(fetchAccessCodes, [projectId]);

  const withRefetch = useCallback(async (operation) => {
    try {
      await operation();
      await refetch();
    } catch (err) {
      console.error(err);
      throw err;
    }
  }, [refetch]);

  const createAccessCode = useCallback(
    (description = 'Access code for project') =>
      withRefetch(() => AccessCodeService.createForProject({ projectId, description })),
    [projectId, withRefetch]
  );

  const retireCode = useCallback(
    (code) => withRefetch(() => AccessCodeService.retireCode(code)),
    [withRefetch]
  );

  const copyCode = useCallback(async (code) => {
    try {
      await navigator.clipboard.writeText(code);
    } catch {
      const textArea = document.createElement('textarea');
      textArea.value = code;
      document.body.appendChild(textArea);
      textArea.select();
      document.execCommand('copy');
      document.body.removeChild(textArea);
    }
  }, []);

  const toggleVisibility = useCallback((code) => {
    setVisibleCodes(prev => ({ ...prev, [code]: !prev[code] }));
  }, []);

  return {
    accessCodes,
    visibleCodes,
    loading,
    error,
    createAccessCode,
    retireCode,
    copyCode,
    toggleVisibility,
    refetch
  };
};
