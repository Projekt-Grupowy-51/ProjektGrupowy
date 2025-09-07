import { useState, useEffect } from 'react';
import ProjectReportService from '../services/ProjectReportService.js';
import ProjectService from '../services/ProjectService.js';

export const useProjectReports = (projectId) => {
  
  const [reports, setReports] = useState([]);
  const [loading, setLoading] = useState(false);
  const [creating, setCreating] = useState(false);
  const [deleting, setDeleting] = useState(false);
  const [error, setError] = useState(null);

  const fetchReports = () => {
    if (!projectId) return Promise.resolve();
    
    setLoading(true);
    setError(null);
    return ProjectService.getReports(projectId)
      .then(data => setReports(data || []))
      .catch(err => setError(err.message))
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    fetchReports();
  }, [projectId]);

  const generateReport = async (title, description) => {
    setCreating(true);
    setError(null);
    try {
      const newReport = await ProjectReportService.create({
        title: title || `Project Report ${new Date().toLocaleDateString()}`,
        description: description || 'Generated project progress report',
        projectId
      });
      await fetchReports();
      return newReport;
    } catch (err) {
      setError(err.message);
      throw err;
    } finally {
      setCreating(false);
    }
  };

  const deleteReport = async (reportId) => {
    setDeleting(true);
    setError(null);
    return ProjectReportService.delete(reportId)
      .then(() => fetchReports())
      .catch(err => setError(err.message))
      .finally(() => setDeleting(false));
  };

  const downloadReport = async (reportId) => {
    try {
      const blob = await ProjectReportService.download(reportId);
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = `project-report-${reportId}.json`;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      window.URL.revokeObjectURL(url);
    } catch (error) {
      setError(error.message);
      throw error;
    }
  };

  return {
    reports,
    loading: loading || creating || deleting,
    error,
    generateReport,
    deleteReport,
    downloadReport,
    refetch: fetchReports
  };
};
