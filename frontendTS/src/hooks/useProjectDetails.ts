import { useCallback, useEffect, useState } from 'react';
import { Project } from '../models/Project';
import { Report } from '../models/Report';
import { getProject, getProjectReports } from '../services/api/projectService';
import { getSignalRService } from '../services/SignalRServiceInstance';
import { useNotification } from '../context/NotificationContext';
import { MessageTypes } from '../config/messageTypes';

export default function useProjectDetails(id?: number) {
  const [project, setProject] = useState<Project | null>(null);
  const [reports, setReports] = useState<Report[]>([]);
  const [activeTab, setActiveTab] = useState('details');
  const [labelersCount, setLabelersCount] = useState(0);
  const { addNotification } = useNotification();

  const fetchReports = useCallback(async () => {
    if (!id) return;
    const data = await getProjectReports(id);
    setReports(data);
  }, [id]);

  const fetchProject = useCallback(async () => {
    if (!id) return;
    const data = await getProject(id);
    setProject(data);
    await fetchReports();
  }, [id, fetchReports]);

  useEffect(() => {
    fetchProject();
  }, [fetchProject]);

  useEffect(() => {
    const cached = localStorage.getItem(`project_${id}_activeTab`);
    if (cached) setActiveTab(cached);
  }, [id]);

  useEffect(() => {
    const signalRService = getSignalRService(addNotification);
    signalRService.onMessage(MessageTypes.LabelersCountChanged, (msg: number) => setLabelersCount(msg));
    signalRService.onMessage(MessageTypes.ReportGenerated, fetchReports);
  }, [addNotification, fetchReports]);

  const handleTabChange = (tab: string) => {
    setActiveTab(tab);
    if (id) localStorage.setItem(`project_${id}_activeTab`, tab);
  };

  return { project, reports, activeTab, handleTabChange, labelersCount, setLabelersCount, fetchReports };
}
