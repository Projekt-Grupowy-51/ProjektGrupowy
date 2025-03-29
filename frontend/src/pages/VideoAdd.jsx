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

      navigate(`/video-groups/${formData.videoGroupId}`);
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
        <div className="content">
          <div className="error">{error}</div>
          <button
            className="btn btn-secondary"
            onClick={() => navigate("/projects")}
          >
            Back to Projects
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="container">
      <div className="content">
        <h1>Add New Video</h1>
        {error && <div className="error">{error}</div>}

        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="title">Title</label>
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

          <div className="form-group">
            <label htmlFor="file">Video File</label>
            <div className="file-input-container">
              <label className={`btn btn-primary ${loading ? "disabled" : ""}`}>
                Choose File
                <input
                  type="file"
                  id="file"
                  name="file"
                  onChange={handleFileChange}
                  className="file-input"
                  accept="video/*"
                  required
                  disabled={loading}
                  style={{ display: "none" }} // Hide the default file input
                />
              </label>
              {file && (
                <div className="file-info mt-2">
                  <p>Selected file: {file.name}</p>
                  <p>Size: {(file.size / (1024 * 1024)).toFixed(2)} MB</p>
                </div>
              )}
            </div>
          </div>

          <div className="form-group">
            <label htmlFor="positionInQueue">Position in Queue</label>
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

          <div className="form-group">
            <label htmlFor="videoGroupId">Video Group</label>
            <div className="videogroup-display">
              <input
                type="text"
                value={videoGroupName || `Group ID: ${formData.videoGroupId}`}
                className="form-control"
                disabled
              />
              <input
                type="hidden"
                name="videoGroupId"
                value={formData.videoGroupId}
              />
            </div>
          </div>

          {loading && (
            <div className="progress-container">
              <div className="progress-bar-container">
                <div
                  className="progress-bar"
                  style={{ width: `${uploadProgress}%` }}
                ></div>
              </div>
              <div className="progress-text">Uploading: {uploadProgress}%</div>
            </div>
          )}

          <div className="button-group">
            <button
              type="submit"
              className="btn btn-primary"
              disabled={loading}
            >
              {loading ? "Uploading..." : "Upload Video"}
            </button>
            <button
              type="button"
              className="btn btn-secondary"
              onClick={() => navigate(`/video-groups/${formData.videoGroupId}`)}
              disabled={loading}
            >
              Cancel
            </button>
          </div>
        </form>
      </div>

      <style jsx>{`
        .file-input-container {
          margin-bottom: 15px;
        }

        .file-input {
          padding: 10px 0;
          width: 100%;
        }

        .file-info {
          margin-top: 10px;
          padding: 10px;
          background-color: #f8f8f8;
          border-radius: 4px;
          border-left: 3px solid #3498db;
        }

        .progress-container {
          margin: 20px 0;
        }

        .progress-bar-container {
          height: 20px;
          background-color: #f0f0f0;
          border-radius: 10px;
          overflow: hidden;
        }

        .progress-bar {
          height: 100%;
          background-color: #4caf50;
          transition: width 0.3s ease;
        }

        .progress-text {
          text-align: center;
          margin-top: 5px;
          font-weight: bold;
        }

        .videogroup-display {
          display: flex;
          align-items: center;
        }
      `}</style>
    </div>
  );
};

export default VideoAdd;
