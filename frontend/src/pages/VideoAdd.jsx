import React, { useState, useEffect, useRef } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import httpClient from "../httpClient";
import "./css/ScientistProjects.css";
import { useNotification } from "../context/NotificationContext";
import DeleteButton from "../components/DeleteButton";

const VideoAdd = () => {
  const [videoGroupId, setVideoGroupId] = useState(null);
  const [videoGroupName, setVideoGroupName] = useState("");
  const [videos, setVideos] = useState([]);
  const [loading, setLoading] = useState(false);
  const [uploadProgress, setUploadProgress] = useState(0);
  const navigate = useNavigate();
  const location = useLocation();
  const dropRef = useRef(null);
  const { addNotification } = useNotification();

  useEffect(() => {
    const queryParams = new URLSearchParams(location.search);
    const groupId = queryParams.get("videogroupId");

    if (!groupId) {
      addNotification("Video Group ID is required. Please go back and try again.", "error");
      return;
    }

    const parsedId = parseInt(groupId);
    setVideoGroupId(parsedId);
    fetchVideoGroupName(parsedId);
  }, [location.search]);

  const fetchVideoGroupName = async (id) => {
    try {
      const response = await httpClient.get(`/videogroup/${id}`);
      setVideoGroupName(response.data.name);
    } catch {
      addNotification("Failed to load video group information.", "error");
    }
  };

  const handleDrop = (e) => {
    e.preventDefault();
    const droppedFiles = Array.from(e.dataTransfer.files);

    const accepted = [];
    const rejected = [];

    droppedFiles.forEach((file) => {
      if (!file.type.startsWith("video/")) {
        rejected.push(`${file.name} is not a valid video.`);
      } else if (file.size > 100 * 1024 * 1024) {
        rejected.push(`${file.name} exceeds 100MB limit.`);
      } else {
        accepted.push({
          file,
          title: file.name.replace(/\.[^/.]+$/, ""),
          positionInQueue: videos.length + accepted.length + 1,
        });
      }
    });

    setVideos((prev) => [...prev, ...accepted]);
    if (rejected.length > 0) {
      addNotification(rejected.join("\n"), "error");
    }
  };

  const handleDragOver = (e) => e.preventDefault();

  const handleInputChange = (index, name, value) => {
    setVideos((prev) => {
      const updated = [...prev];
      updated[index][name] =
        name === "positionInQueue" ? parseInt(value) : value;
      return updated;
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setUploadProgress(0);

    if (videos.length === 0) {
      addNotification("Please drag and drop at least one video.", "error");
      setLoading(false);
      return;
    }

    try {
      const progressPerVideo = Array(videos.length).fill(0);

      const uploadPromises = videos.map((video, index) => {
        const formDataObj = new FormData();
        formDataObj.append("Title", video.title);
        formDataObj.append("VideoGroupId", videoGroupId);
        formDataObj.append("PositionInQueue", video.positionInQueue);
        formDataObj.append("File", video.file);

        return httpClient.post("/video", formDataObj, {
          headers: {
            "Content-Type": "multipart/form-data",
          },
          onUploadProgress: (progressEvent) => {
            const percentCompleted = Math.round(
              (progressEvent.loaded * 100) / progressEvent.total
            );
            progressPerVideo[index] = percentCompleted;
            const overallProgress = Math.round(
              progressPerVideo.reduce((a, b) => a + b, 0) / videos.length
            );
            setUploadProgress(overallProgress);
          },
        });
      });

      await Promise.all(uploadPromises);

      navigate(`/video-groups/${videoGroupId}`, {
        state: { successMessage: "All videos uploaded successfully!" },
      });
    } catch (err) {
      addNotification(
        err.response?.data?.message ||
          "An error occurred while uploading the videos.",
        "error"
      );
      setLoading(false);
    }
  };

  const handleRemove = (indexToRemove) => {
    setVideos((prev) => prev.filter((_, i) => i !== indexToRemove));
  };

  return (
    <div className="container py-4">
      <div className="card shadow-sm">
        <div className="card-header bg-primary text-white">
          <h1 className="heading mb-0">Upload Multiple Videos</h1>
        </div>
        <div className="card-body">
          <div
            ref={dropRef}
            className="drop-area border border-primary rounded mb-4 p-4 text-center"
            onDrop={handleDrop}
            onDragOver={handleDragOver}
            style={{ background: "#f8f9fa", cursor: "pointer" }}
          >
            <i className="fas fa-cloud-upload-alt fa-2x mb-2"></i>
            <p className="mb-0">Drag and drop your videos here</p>
            <small className="text-muted">Maximum size: 100MB each</small>
          </div>

          {videos.length > 0 && (
            <form onSubmit={handleSubmit}>
              <table className="normal-table">
                <thead className="table-light">
                  <tr>
                    <th>Title</th>
                    <th>File Size</th>
                    <th>Position in Queue</th>
                    <th>Actions</th> 
                  </tr>
                </thead>
                <tbody>
                  {videos.map((video, index) => (
                    <tr key={index}>
                      <td>
                        <input
                          type="text"
                          className="form-control"
                          value={video.title}
                          onChange={(e) =>
                            handleInputChange(index, "title", e.target.value)
                          }
                          disabled={loading}
                          required
                        />
                      </td>
                      <td>{(video.file.size / (1024 * 1024)).toFixed(2)} MB</td>
                      <td>
                        <input
                          type="number"
                          className="form-control"
                          value={video.positionInQueue}
                          min="1"
                          onChange={(e) =>
                            handleInputChange(
                              index,
                              "positionInQueue",
                              e.target.value
                            )
                          }
                          disabled={loading}
                          required
                        />
                      </td>
                      <td>
                        <div className="d-flex justify-content-center">
                          <DeleteButton onClick={() => handleRemove(index)} />
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>

              {loading && (
                <div className="mb-3">
                  <label className="form-label">Upload Progress</label>
                  <div className="progress">
                    <div
                      className="progress-bar progress-bar-striped progress-bar-animated"
                      style={{ width: `${uploadProgress}%` }}
                    >
                      {uploadProgress}%
                    </div>
                  </div>
                </div>
              )}

              <div className="d-flex justify-content-end">
                <button
                  type="submit"
                  className="btn btn-primary"
                  disabled={loading}
                >
                  <i className="fas fa-upload me-2"></i>
                  {loading ? "Uploading..." : "Upload All Videos"}
                </button>
              </div>
            </form>
          )}

          {videoGroupId && (
            <input type="hidden" name="videoGroupId" value={videoGroupId} />
          )}
        </div>
      </div>
    </div>
  );
};

export default VideoAdd;
