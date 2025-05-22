import { useCallback, useEffect, useState } from 'react';
import { VideoGroup } from '../models/VideoGroup';
import { Video } from '../models/Video';
import { getVideoGroup, getVideoGroupVideos } from '../services/api/videoGroupService';

export default function useVideoGroupDetails(id?: number) {
  const [videoGroup, setVideoGroup] = useState<VideoGroup | null>(null);
  const [videos, setVideos] = useState<Video[]>([]);

  const fetchVideos = useCallback(async () => {
    if (!id) return;
    const data = await getVideoGroupVideos(id);
    setVideos(data);
  }, [id]);

  const fetchVideoGroup = useCallback(async () => {
    if (!id) return;
    const data = await getVideoGroup(id);
    setVideoGroup(data);
    fetchVideos();
  }, [id, fetchVideos]);

  useEffect(() => {
    fetchVideoGroup();
  }, [fetchVideoGroup]);

  return { videoGroup, videos, setVideos, fetchVideos };
}
