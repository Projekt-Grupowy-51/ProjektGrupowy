import httpClient from '../../httpclient';
import { Video } from '../../models/Video';

export const createVideo = async (
  formData: FormData,
  onUploadProgress?: (event: ProgressEvent) => void,
): Promise<void> => {
  await httpClient.post('/video', formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
    onUploadProgress,
  });
};

export const getVideo = async (id: number): Promise<Video> => {
  const { data } = await httpClient.get<Video>(`/Video/${id}`);
  return data;
};

export const getVideoStream = async (id: number): Promise<Blob> => {
  const { data } = await httpClient.get(`/Video/${id}/stream`, { responseType: 'blob' });
  return data;
};

export const getAssignedLabels = async (id: number) => {
  const { data } = await httpClient.get(`/Video/${id}/assignedlabels`);
  return data;
};
