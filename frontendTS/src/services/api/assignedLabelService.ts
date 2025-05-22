import httpClient from '../../httpclient';

export const createAssignedLabel = async (
  labelId: number,
  videoId: number,
  start: string,
  end: string,
): Promise<void> => {
  await httpClient.post('/AssignedLabel', { labelId, videoId, start, end });
};

export const deleteAssignedLabel = async (id: number): Promise<void> => {
  await httpClient.delete(`/AssignedLabel/${id}`);
};
