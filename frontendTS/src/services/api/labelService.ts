import httpClient from '../../httpclient';
import { Label } from '../../models/Label';

export const getLabel = async (id: number): Promise<Label> => {
  const { data } = await httpClient.get<Label>(`/Label/${id}`);
  return data;
};

export const createLabel = async (label: Omit<Label, 'id'>): Promise<void> => {
  await httpClient.post('/label', label);
};

export const updateLabel = async (id: number, label: Omit<Label, 'id'>): Promise<void> => {
  await httpClient.put(`/Label/${id}`, label);
};

export const deleteLabel = async (id: number): Promise<void> => {
  await httpClient.delete(`/label/${id}`);
};
