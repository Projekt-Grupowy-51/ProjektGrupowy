import React, { useState, useEffect } from "react";
import { useParams, useNavigate, Link, useLocation } from "react-router-dom";
import httpClient, { API_BASE_URL } from "../httpclient";
import DeleteButton from "../components/DeleteButton";
import "./css/ScientistProjects.css";
import NavigateButton from "../components/NavigateButton";
import DataTable from "../components/DataTable";
import { useNotification } from "../context/NotificationContext";

const VideoGroupDetails = () => {
  const { id } = useParams();
  const [videoGroupDetails, setVideoGroupDetails] = useState(null);
  const [videos, setVideos] = useState([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();
  const location = useLocation();
  const { addNotification } = useNotification();

  // Fetch video group details
  async function fetchVideoGroupDetails() {
    setLoading(true);
    try {
      const response = await httpClient.get(`/videogroup/${id}`);
      setVideoGroupDetails(response.data);
      fetchVideos();
    } catch (error) {
      addNotification(
        error.response?.data?.message || "Failed to fetch video group details",
        "error"
      );
      setLoading(false);
    }
  }

  // Fetch the list of videos
  async function fetchVideos() {
    try {
      const response = await httpClient.get(`/VideoGroup/${id}/videos`);
      setVideos(response.data);
    } catch (error) {
      addNotification(
        error.response?.data?.message || "Failed to fetch videos",
        "error"
      );
    } finally {
      setLoading(false);
    }
  }

  function openVideoStream(videoId) {
    window.open(`${API_BASE_URL}/video/${videoId}/stream`);
  }

  useEffect(() => {
    if (id) fetchVideoGroupDetails();
  }, [id]);

  // Handle location state for success messages
  useEffect(() => {
    if (location.state?.successMessage) {
      addNotification(location.state.successMessage, "success");
      window.history.replaceState({}, document.title);
    }
  }, [location.state]);

  // Simplified delete handler - will be passed to DeleteButton
  const handleDeleteVideo = async (videoId, videoTitle) => {
    try {
      await httpClient.delete(`/video/${videoId}`);
      setVideos(videos.filter((video) => video.id !== videoId));
      addNotification("Video deleted successfully!", "success");
    } catch (error) {
      addNotification(
        error.response?.data?.message || "Failed to delete video",
        "error"
      );
    }
  };

  // Define columns for videos table
  const videoColumns = [
    { field: "title", header: "Title" },
    { field: "positionInQueue", header: "Position" },
  ];

  if (loading)
    return (
      <div className="container d-flex justify-content-center align-items-center py-5">
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">Loading...</span>
        </div>
      </div>
    );

  if (!videoGroupDetails) return null;

  return (
    <div className="container">
      <div className="content">
        <h1 className="heading mb-4">{videoGroupDetails.name}</h1>

        <div className="d-flex justify-content-between mb-4">
          <NavigateButton
            path={`/videos/add?videogroupId=${id}`}
            actionType="Add"
          />
          <NavigateButton actionType="Back" />
        </div>

        {videos.length > 0 ? (
          <DataTable
            showRowNumbers={true}
            columns={videoColumns}
            data={videos}
            tableClassName="normal-table table-hover"
            navigateButton={(video) => (
              <NavigateButton
                path={`/videos/${video.id}`}
                actionType="Details"
              />
            )}
            deleteButton={(video) => (
              <DeleteButton
                onClick={() => handleDeleteVideo(video.id, video.title)}
                itemType={`video "${video.title}"`}
              />
            )}
          />
        ) : (
          <div className="card-body text-center py-5">
            <i className="fas fa-film fs-1 text-muted opacity-50"></i>
            <p className="text-muted mt-3 mb-0">
              No videos found in this group. Add some videos to get started.
            </p>
          </div>
        )}
      </div>
    </div>
  );
};

export default VideoGroupDetails;
