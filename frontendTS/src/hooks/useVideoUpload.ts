import { useEffect, useRef, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { useNotification } from '../context/NotificationContext';
import { createVideo } from '../services/api/videoService';
import { getVideoGroup } from '../services/api/videoGroupService';

interface UploadVideo {
  file: File;
  title: string;
  positionInQueue: number;
}

export default function useVideoUpload(search: string) {
  const [videoGroupId, setVideoGroupId] = useState<number | null>(null);
  const [videoGroupName, setVideoGroupName] = useState('');
  const [videos, setVideos] = useState<UploadVideo[]>([]);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const [uploadProgress, setUploadProgress] = useState(0);
  const dropRef = useRef<HTMLDivElement | null>(null);
  const fileInputRef = useRef<HTMLInputElement | null>(null);
  const navigate = useNavigate();
  const { t } = useTranslation(['videos', 'common']);
  const { addNotification } = useNotification();

  useEffect(() => {
    const params = new URLSearchParams(search);
    const id = params.get('videogroupId');
    if (!id) {
      setError(t('errors.load_video_group'));
      return;
    }
    const num = parseInt(id);
    setVideoGroupId(num);
    fetchVideoGroupName(num);
  }, [search, t]);

  const fetchVideoGroupName = async (id: number) => {
    try {
      const group = await getVideoGroup(id);
      setVideoGroupName(group.name);
    } catch {
      setError(t('errors.load_video_group_details'));
    }
  };

  const handleButtonClick = () => {
    fileInputRef.current?.click();
  };

  const handleFileSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
    const selectedFiles = Array.from(e.target.files || []);
    processFiles(selectedFiles);
  };

  const processFiles = (files: File[]) => {
    const accepted: UploadVideo[] = [];
    const rejected: string[] = [];

    files.forEach((file) => {
      if (!file.type.startsWith('video/')) {
        rejected.push(t('upload.error_not_video', { filename: file.name }));
      } else if (file.size > 100 * 1024 * 1024) {
        rejected.push(t('upload.error_size', { filename: file.name }));
      } else {
        accepted.push({
          file,
          title: file.name.replace(/\.[^/.]+$/, ''),
          positionInQueue: videos.length + accepted.length + 1,
        });
      }
    });

    setVideos(prev => [...prev, ...accepted]);
    if (rejected.length > 0) setError(rejected.join('\n'));
  };

  const handleDrop = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    const droppedFiles = Array.from(e.dataTransfer.files);
    processFiles(droppedFiles);
  };

  const handleDragOver = (e: React.DragEvent<HTMLDivElement>) => e.preventDefault();

  const handleInputChange = (index: number, name: keyof UploadVideo, value: any) => {
    setVideos(prev => {
      const updated = [...prev];
      // @ts-ignore
      updated[index][name] = name === 'positionInQueue' ? parseInt(value) : value;
      return updated;
    });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    setUploadProgress(0);

    if (videos.length === 0 || !videoGroupId) {
      setError(t('upload.error_empty'));
      setLoading(false);
      return;
    }

    try {
      const progressPerVideo = Array(videos.length).fill(0);

      const uploadPromises = videos.map((video, index) => {
        const formDataObj = new FormData();
        formDataObj.append('Title', video.title);
        formDataObj.append('VideoGroupId', videoGroupId.toString());
        formDataObj.append('PositionInQueue', video.positionInQueue.toString());
        formDataObj.append('File', video.file);

        return createVideo(formDataObj, (progressEvent) => {
          const percentCompleted = Math.round((progressEvent.loaded * 100) / progressEvent.total);
          progressPerVideo[index] = percentCompleted;
          const overallProgress = Math.round(progressPerVideo.reduce((a, b) => a + b, 0) / videos.length);
          setUploadProgress(overallProgress);
        });
      });

      await Promise.all(uploadPromises);
      navigate(`/video-groups/${videoGroupId}`);
    } catch (err: any) {
      setError(err.response?.data?.message || t('upload.error_general'));
      setLoading(false);
    }
  };

  const handleRemove = (index: number) => {
    setVideos(prev => prev.filter((_, i) => i !== index));
  };

  return {
    videoGroupId,
    videoGroupName,
    videos,
    error,
    loading,
    uploadProgress,
    dropRef,
    fileInputRef,
    handleButtonClick,
    handleFileSelect,
    handleDrop,
    handleDragOver,
    handleInputChange,
    handleSubmit,
    handleRemove,
  };
}
