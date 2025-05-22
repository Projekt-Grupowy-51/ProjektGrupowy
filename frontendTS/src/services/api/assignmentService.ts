import httpClient from '../../httpclient';
import { Assignment } from '../../models/Assignment';

export const createAssignment = async (assignment: Omit<Assignment, 'id'>): Promise<void> => {
  await httpClient.post('/SubjectVideoGroupAssignment', assignment);
};

export const getAssignment = async (id: number): Promise<Assignment> => {
  const { data } = await httpClient.get<Assignment>(`/SubjectVideoGroupAssignment/${id}`);
  return data;
};

export const deleteAssignment = async (id: number): Promise<void> => {
  await httpClient.delete(`/SubjectVideoGroupAssignment/${id}`);
};

export const getAssignmentLabelers = async (id: number) => {
  const { data } = await httpClient.get(`/SubjectVideoGroupAssignment/${id}/labelers`);
  return data;
};

export const getAssignments = async (): Promise<Assignment[]> => {
  const { data } = await httpClient.get<Assignment[]>('/SubjectVideoGroupAssignment');
  return data;
};
