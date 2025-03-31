import React, { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import httpClient from "../httpClient";
import "./css/ScientistProjects.css";

const VideoAdd = () => {
  const [formData, setFormData] = useState({
    title: "",
    videoGroupId: null,
    positionInQueue: 1,
  });
  const [file, setFile] = useState(null);
  const [videoGroupName, setVideoGroupName] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const [uploadProgress, setUploadProgress] = useState(0);
  const navigate = useNavigate();
  const location = useLocation();

  useEffect(() => {
    // Extract videoGroupId from query params
    const queryParams = new URLSearchParams(location.search);
    const videoGroupId = queryParams.get("videogroupId");

    if (!videoGroupId) {
      setError("Video Group ID is required. Please go back and try again.");
      return;
    }

    setFormData((prev) => ({ ...prev, videoGroupId: parseInt(videoGroupId) }));
    fetchVideoGroupName(parseInt(videoGroupId));
  }, [location.search]);

  const fetchVideoGroupName = async (id) => {
    try {
      const response = await httpClient.get(`/videogroup/${id}`);
      setVideoGroupName(response.data.name);
    } catch (err) {
      setError("Failed to load video group information.");
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: name === "positionInQueue" ? parseInt(value) : value,
    }));
  };

  const handleFileChange = (e) => {
    const selectedFile = e.target.files[0];

    if (selectedFile) {
      // Check if the file is a video
      if (!selectedFile.type.startsWith("video/")) {
        setError("Please select a valid video file.");
        setFile(null);
        e.target.value = null;
        return;
      }

      // Check file size (limit to 100MB)
      if (selectedFile.size > 100 * 1024 * 1024) {
        setError("File size exceeds 100MB limit.");
        setFile(null);
        e.target.value = null;
        return;
      }

      setFile(selectedFile);
      setError("");
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError("");
    setUploadProgress(0);

    // Validate form
    if (!formData.title || !formData.videoGroupId || !file) {
      setError("Please fill in all required fields and upload a video file.");
      setLoading(false);
      return;
    }

    // Create FormData to send the file
    const formDataObj = new FormData();
    formDataObj.append("Title", formData.title);
    formDataObj.append("VideoGroupId", formData.videoGroupId);
    formDataObj.append("PositionInQueue", formData.positionInQueue);
    formDataObj.append("File", file);

    try {
      await httpClient.post("/video", formDataObj, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
        onUploadProgress: (progressEvent) => {
          const percentCompleted = Math.round(
            (progressEvent.loaded * 100) / progressEvent.total
          );
          setUploadProgress(percentCompleted);
        },
      });

        navigate(`/video-groups/${formData.videoGroupId}`, {
            state: { successMessage: "Video added successfully!" }
        });
    } catch (err) {
      setError(
        err.response?.data?.message ||
          "An error occurred while uploading the video."
      );
      setLoading(false);
    }
  };

    if (!formData.videoGroupId) {
        return (
            <div className="container">
                <div className="alert alert-danger">
                    <i className="fas fa-exclamation-triangle me-2"></i>
                    {error || 'Missing Video Group ID parameter'}
                </div>
                <button 
                    className="btn btn-secondary"
                    onClick={() => navigate('/projects')}
                    style={{height: 'fit-content', margin: '1%'}}
                >
                    <i className="fas fa-arrow-left me-2"></i>Back to Projects
                </button>
            </div>
        );
    }

    return (
        <div className="container py-4">
            <div className="row justify-content-center">
                <div className="col-lg-8">
                    <div className="card shadow-sm">
                        <div className="card-header bg-primary text-white">
                            <h1 className="heading mb-0">Add New Video</h1>
                        </div>
                        <div className="card-body">
                            {error && (
                                <div className="alert alert-danger mb-4">
                                    <i className="fas fa-exclamation-triangle me-2"></i>
                                    {error}
                                </div>
                            )}
                            
                            <form onSubmit={handleSubmit}>
                                <div className="mb-3">
                                    <label htmlFor="title" className="form-label">Video Title</label>
                                    <input
                                        type="text"
                                        id="title"
                                        name="title"
                                        value={formData.title}
                                        onChange={handleChange}
                                        className="form-control"
                                        required
                                        disabled={loading}
                                    />
                                </div>

                                <div className="mb-3">
                                    <label htmlFor="file" className="form-label">Video File</label>
                                    <input
                                        type="file"
                                        id="file"
                                        name="file"
                                        onChange={handleFileChange}
                                        className="form-control"
                                        accept="video/*"
                                        required
                                        disabled={loading}
                                    />
                                    
                                    {file && (
                                        <div className="alert alert-info mt-2">
                                            <div className="d-flex align-items-center">
                                                <i className="fas fa-file-video me-2"></i>
                                                <div>
                                                    <strong>{file.name}</strong>
                                                    <div className="text-muted">Size: {(file.size / (1024 * 1024)).toFixed(2)} MB</div>
                                                </div>
                                            </div>
                                        </div>
                                    )}
                                </div>

                                <div className="mb-3">
                                    <label htmlFor="positionInQueue" className="form-label">Position in Queue</label>
                                    <input
                                        type="number"
                                        id="positionInQueue"
                                        name="positionInQueue"
                                        value={formData.positionInQueue}
                                        onChange={handleChange}
                                        className="form-control"
                                        min="1"
                                        required
                                        disabled={loading}
                                    />
                                </div>

                                <div className="mb-4">
                                    <label htmlFor="videoGroupId" className="form-label">Video Group</label>
                                    <div className="input-group">
                                        <span className="input-group-text">
                                            <i className="fas fa-film"></i>
                                        </span>
                                        <input
                                            type="text"
                                            value={videoGroupName || `Group ID: ${formData.videoGroupId}`}
                                            className="form-control"
                                            disabled
                                        />
                                    </div>
                                    <input
                                        type="hidden"
                                        name="videoGroupId"
                                        value={formData.videoGroupId}
                                    />
                                </div>

                                {loading && (
                                    <div className="mb-4">
                                        <label className="form-label">Upload Progress</label>
                                        <div className="progress">
                                            <div 
                                                className="progress-bar progress-bar-striped progress-bar-animated" 
                                                role="progressbar" 
                                                style={{ width: `${uploadProgress}%` }} 
                                                aria-valuenow={uploadProgress} 
                                                aria-valuemin="0" 
                                                aria-valuemax="100"
                                            >
                                                {uploadProgress}%
                                            </div>
                                        </div>
                                        <div className="text-center mt-2">
                                            <small>Please wait while your video is being uploaded...</small>
                                        </div>
                                    </div>
                                )}

                                <div className="d-flex">
                                    <button 
                                        type="submit" 
                                        className="btn btn-primary me-2"
                                        disabled={loading}
                                    >
                                        <i className="fas fa-cloud-upload-alt me-2"></i>
                                        {loading ? "Uploading..." : "Upload Video"}
                                    </button>
                                    <button 
                                        type="button" 
                                        className="btn btn-secondary"
                                        onClick={() => navigate(`/video-groups/${formData.videoGroupId}`)}
                                        disabled={loading}
                                    >
                                        <i className="fas fa-times me-2"></i>Cancel
                                    </button>
                                </div>
                            </form>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default VideoAdd;
