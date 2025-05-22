import httpClient from '../../httpclient';
import { VideoGroup } from '../../models/VideoGroup';
import { Video } from '../../models/Video';

export const getVideoGroup = async (id: number): Promise<VideoGroup> => {
  const { data } = await httpClient.get<VideoGroup>(`/videogroup/${id}`);
  return data;
};

export const createVideoGroup = async (group: Omit<VideoGroup, 'id'>): Promise<void> => {
  await httpClient.post('/videogroup', group);
};

export const getVideoGroupVideos = async (id: number): Promise<Video[]> => {
  const { data } = await httpClient.get<Video[]>(`/VideoGroup/${id}/videos`);
  return data;
};

export const getVideoBatch = async (videoGroupId: number, batch: number) => {
  const { data } = await httpClient.get(`/Video/batch/${videoGroupId}/${batch}`);
  return data;
};
