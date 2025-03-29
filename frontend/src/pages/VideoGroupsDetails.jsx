import React, { useState, useEffect } from "react";
import { useParams, useNavigate, Link } from "react-router-dom";
import httpClient, { API_BASE_URL } from "../httpClient";
import "./css/ScientistProjects.css";

const VideoGroupDetails = () => {
  const { id } = useParams();
  const [videoGroupDetails, setVideoGroupDetails] = useState(null);
  const [videos, setVideos] = useState([]);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  // Fetch video group details
  async function fetchVideoGroupDetails() {
    setLoading(true);
    setError("");
    try {
      const response = await httpClient.get(`/videogroup/${id}`);
      setVideoGroupDetails(response.data);
      fetchVideos();
    } catch (error) {
      setError(
        error.response?.data?.message || "Failed to fetch video group details"
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
      setError(error.response?.data?.message || "Failed to fetch videos");
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

  // Redirect to "Add Video" form
  function addVideo() {
    navigate(`/videos/add?videogroupId=${id}`);
  }

  // Delete a video
  async function deleteVideo(videoId) {
    if (!window.confirm("Are you sure you want to delete this video?")) return;

    try {
      await httpClient.delete(`/video/${videoId}`);
      setVideos(videos.filter((video) => video.id !== videoId));
      alert("Video deleted successfully");
    } catch (error) {
      setError(error.response?.data?.message || "Failed to delete video");
    }
  }

    if (loading) return (
        <div className="container d-flex justify-content-center align-items-center py-5">
            <div className="spinner-border text-primary" role="status">
                <span className="visually-hidden">Loading...</span>
            </div>
        </div>
    );

    if (error) return (
        <div className="container">
            <div className="alert alert-danger">
                <i className="fas fa-exclamation-circle me-2"></i>{error}
            </div>
            <button className="btn btn-secondary" 
                    onClick={() => navigate('/projects')}
                    style={{height: 'fit-content', margin: '1%'}}
>
                <i className="fas fa-arrow-left me-2"></i>Back to Projects
            </button>
        </div>
    );

  if (!videoGroupDetails) return null;

    return (
        <div className="container">
            <div className="content">
                <h1 className="heading mb-4">{videoGroupDetails.name}</h1>
                
                {/* <div className="card shadow-sm mb-4">
                    <div className="card-header bg-primary text-white">
                        <h5 className="card-title mb-0">Video Group Details</h5>
                    </div>
                    <div className="card-body">
                        <p className="card-text">
                            <strong>Description:</strong> {videoGroupDetails.description}
                        </p>
                    </div>
                </div> */}

                <div className="d-flex justify-content-between mb-4">
                    <button className="btn btn-primary" onClick={addVideo}>
                        <i className="fas fa-plus-circle me-2"></i>Add Video
                    </button>
                    <Link to={`/projects/${videoGroupDetails.projectId}`} 
                        className="btn btn-secondary"
                        style={{height: 'fit-content', margin: '1%'}}>
                        <i className="fas fa-arrow-left me-2"></i>Back to Project
                    </Link>
                </div>

                <div className="card shadow-sm">
                    <div className="card-header bg-primary text-white d-flex justify-content-between align-items-center">
                        <h5 className="card-title mb-0">Videos</h5>
                        <span className="badge bg-light text-dark">{videos.length} videos</span>
                    </div>
                    <div className="card-body p-0">
                        {videos.length > 0 ? (
                            <div className="table-responsive">
                                <table className="normal-table" style={{margin: '0px'}}>
                                    <thead>
                                        <tr>
                                            <th>ID</th>
                                            <th>Title</th>
                                            <th>Position</th>
                                            <th>Actions</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {videos.map((video) => (
                                            <tr key={video.id}>
                                                <td>{video.id}</td>
                                                <td>{video.title}</td>
                                                <td>{video.positionInQueue}</td>
                                                <td>
                                                    <div className="btn-group">
                                                        <button
                                                            className="btn btn-info btn-sm me-2"
                                                            onClick={() => navigate(`/videos/${video.id}`)}
                                                        >
                                                            <i className="fas fa-eye me-1"></i>Details
                                                        </button>
                                                        <button
                                                            className="btn btn-danger btn-sm"
                                                            onClick={() => deleteVideo(video.id)}
                                                        >
                                                            <i className="fas fa-trash me-1"></i>Delete
                                                        </button>
                                                    </div>
                                                </td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>
                        ) : (
                            <div className="text-center py-4">
                                <i className="fas fa-film fs-1 text-muted"></i>
                                <p className="text-muted mt-2">No videos found in this group. Add some videos to get started.</p>
                            </div>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
};

export default VideoGroupDetails;
