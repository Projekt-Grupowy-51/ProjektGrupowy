import { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import VideoService from "../services/VideoService.js";
import config from "../config/config.js";

export const useVideoDetails = () => {
  const { id } = useParams();
  const navigate = useNavigate();

  const [video, setVideo] = useState(null);
  const [assignedLabels, setAssignedLabels] = useState([]);
  const [videoLoading, setVideoLoading] = useState(false);
  const [labelsLoading, setLabelsLoading] = useState(false);
  const [videoError, setVideoError] = useState(null);
  const [labelsError, setLabelsError] = useState(null);
  const [videoStreamUrl, setVideoStreamUrl] = useState(null);
  const [videoStreamLoading, setVideoStreamLoading] = useState(false);

  // Pagination state
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(config.ui.defaultPageSize);
  const [totalPages, setTotalPages] = useState(0);
  const [totalItems, setTotalItems] = useState(0);

  const fetchVideo = async () => {
    const videoId = parseInt(id);
    setVideoLoading(true);
    setVideoError(null);

    try {
      const videoData = await VideoService.getById(videoId);
      setVideo(videoData);
    } catch (err) {
      setVideoError(err.message);
    } finally {
      setVideoLoading(false);
    }
  };

  const fetchLabels = async (page = currentPage, size = pageSize) => {
    const videoId = parseInt(id);
    setLabelsLoading(true);
    setLabelsError(null);

    try {
      const response = await VideoService.getAssignedLabelsPaginated(
        videoId,
        page,
        size
      );

      // Handle response structure: { assignedLabels: [...], totalLabelCount: number }
      const labels = response.assignedLabels || [];
      const totalCount = response.totalLabelCount || 0;

      setAssignedLabels(labels);
      setTotalItems(totalCount);
      setTotalPages(Math.ceil(totalCount / size));
      setCurrentPage(page);
    } catch (err) {
      setLabelsError(err.message);
      setAssignedLabels([]);
    } finally {
      setLabelsLoading(false);
    }
  };

  useEffect(() => {
    fetchVideo();
    fetchLabels();
  }, [id]);

  // Load video stream URL when video data is available
  useEffect(() => {
    const loadVideoStream = async () => {
      if (video?.id && !videoStreamUrl) {
        setVideoStreamLoading(true);
        try {
          const streamUrl = await VideoService.getStreamBlob(video.id);
          setVideoStreamUrl(streamUrl);
        } catch (error) {
        } finally {
          setVideoStreamLoading(false);
        }
      }
    };

    loadVideoStream();
  }, [video?.id, videoStreamUrl]);

  // Cleanup blob URL on unmount
  useEffect(() => {
    return () => {
      if (videoStreamUrl) {
        URL.revokeObjectURL(videoStreamUrl);
      }
    };
  }, [videoStreamUrl]);

  const handleBackToVideoGroup = () => {
    if (video?.videoGroupId) {
      navigate(`/videogroups/${video.videoGroupId}`);
    }
  };

  const handlePageChange = (newPage) => {
    fetchLabels(newPage, pageSize);
  };

  const handlePageSizeChange = (newSize) => {
    setPageSize(newSize);
    fetchLabels(1, newSize);
  };

  const formatDuration = (seconds) => {
    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = seconds % 60;
    return `${minutes}:${remainingSeconds.toString().padStart(2, "0")}`;
  };

  return {
    video,
    videoLoading,
    videoError,
    assignedLabels,
    labelsLoading,
    labelsError,
    videoStreamUrl,
    videoStreamLoading,
    currentPage,
    pageSize,
    totalPages,
    totalItems,
    handleBackToVideoGroup,
    handlePageChange,
    handlePageSizeChange,
    formatDuration,
  };
};
