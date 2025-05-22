import httpClient from '../../httpclient';
import { Subject } from '../../models/Subject';
import { Label } from '../../models/Label';

export const getSubject = async (id: number): Promise<Subject> => {
  const { data } = await httpClient.get<Subject>(`/subject/${id}`);
  return data;
};

export const createSubject = async (subject: Omit<Subject, 'id'>): Promise<void> => {
  await httpClient.post('/Subject', subject);
};

export const getSubjectLabels = async (id: number): Promise<Label[]> => {
  const { data } = await httpClient.get<Label[]>(`/subject/${id}/label`);
  return data;
};
