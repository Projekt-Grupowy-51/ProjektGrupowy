import httpClient from '../../httpclient';
import { Project } from '../../models/Project';

export const getProjects = async (): Promise<Project[]> => {
  const { data } = await httpClient.get<Project[]>('/Project');
  return data;
};

export const getProject = async (id: number): Promise<Project> => {
  const { data } = await httpClient.get<Project>(`/Project/${id}`);
  return data;
};

export const createProject = async (project: Omit<Project, 'id'>): Promise<void> => {
  await httpClient.post('/Project', project);
};

export const updateProject = async (id: number, project: Omit<Project, 'id'>): Promise<void> => {
  await httpClient.put(`/Project/${id}`, project);
};

export const deleteProject = async (id: number): Promise<void> => {
  await httpClient.delete(`/Project/${id}`);
};

export const getProjectSubjects = async (projectId: number) => {
  const { data } = await httpClient.get(`/project/${projectId}/subjects`);
  return data;
};

export const getProjectVideoGroups = async (projectId: number) => {
  const { data } = await httpClient.get(`/project/${projectId}/videogroups`);
  return data;
};

export const joinProject = async (accessCode: string): Promise<void> => {
  await httpClient.post('/project/join', { AccessCode: accessCode });
};

export const getProjectReports = async (projectId: number) => {
  const { data } = await httpClient.get(`/project/${projectId}/reports`);
  return data;
};

export const generateProjectReport = async (projectId: number): Promise<void> => {
  await httpClient.post(`/projectreport/${projectId}/generate-report`);
};

export const deleteProjectReport = async (reportId: number): Promise<void> => {
  await httpClient.delete(`/projectreport/${reportId}`);
};

export const downloadProjectReport = async (reportId: number): Promise<Blob> => {
  const { data } = await httpClient.get(`/projectreport/download/${reportId}`, { responseType: 'blob' });
  return data;
};
