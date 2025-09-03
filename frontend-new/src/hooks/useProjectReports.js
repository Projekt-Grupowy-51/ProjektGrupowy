import { useCallback } from 'react';
import { useDataFetching, useAsyncOperation, useConfirmDialog } from './common';
import ProjectReportService from '../services/ProjectReportService.js';
import ProjectService from '../services/ProjectService.js';

export const useProjectReports = (projectId) => {
  const { confirmWithPrompt } = useConfirmDialog();

  const fetchReports = useCallback(async () => {
    if (!projectId) return [];
    const reports = await ProjectService.getReports(projectId);
    return reports || [];
  }, [projectId]);

  const { data: reports = [], loading, error, refetch } = useDataFetching(
    fetchReports,
    [projectId]
  );

  const { execute: createReportOp, loading: creating, error: createError } = useAsyncOperation();
  const { execute: deleteReportOp, loading: deleting, error: deleteError } = useAsyncOperation();

  const generateReport = useCallback(
    async (title, description) => {
      try {
        const newReport = await createReportOp(() =>
          ProjectReportService.create({
            title: title || `Project Report ${new Date().toLocaleDateString()}`,
            description: description || 'Generated project progress report',
            projectId
          })
        );
        await refetch();
        return newReport;
      } catch (err) {
        console.error('Failed to generate report:', err);
        throw err;
      }
    },
    [projectId, createReportOp, refetch]
  );

  const deleteReport = useCallback(
    async (reportId, confirmMessage = 'Are you sure you want to delete this report?') => {
      await confirmWithPrompt(confirmMessage, async () => {
        try {
          await deleteReportOp(() => ProjectReportService.delete(reportId));
          await refetch();
        } catch (err) {
          console.error('Failed to delete report:', err);
          throw err;
        }
      });
    },
    [deleteReportOp, confirmWithPrompt, refetch]
  );

  const downloadReport = useCallback((reportId) => {
    window.open(ProjectReportService.getDownloadUrl(reportId));
  }, []);

  return {
    reports: reports || [],
    loading: loading || creating || deleting,
    error: error || createError || deleteError,
    generateReport,
    deleteReport,
    downloadReport,
    refetch
  };
};
