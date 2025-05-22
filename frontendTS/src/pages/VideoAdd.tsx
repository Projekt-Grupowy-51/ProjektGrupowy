import React, { useState, useEffect, useRef } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import "./css/ScientistProjects.css";
import { useTranslation } from "react-i18next";
import { useNotification } from "../context/NotificationContext";
import NavigateButton from "../components/NavigateButton";
import { createVideo } from "../services/api/videoService";
import { getVideoGroup } from "../services/api/videoGroupService";

const VideoAdd = () => {
  const [videoGroupId, setVideoGroupId] = useState(null);
  const [videoGroupName, setVideoGroupName] = useState("");
  const [videos, setVideos] = useState([]);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const [uploadProgress, setUploadProgress] = useState(0);
  const navigate = useNavigate();
  const location = useLocation();
  const dropRef = useRef(null);
  const fileInputRef = useRef(null);
  const { t } = useTranslation(['videos', 'common']);
  const { addNotification } = useNotification();


  const handleButtonClick = () => {
    fileInputRef.current.click();
  };

  const handleFileSelect = (e) => {
    const selectedFiles = Array.from(e.target.files);

    const accepted = [];
    const rejected = [];

    selectedFiles.forEach((file) => {
      if (!file.type.startsWith("video/")) {
        rejected.push(t('upload.error_not_video', { filename: file.name }));
      } else if (file.size > 100 * 1024 * 1024) {
        rejected.push(t('upload.error_size', { filename: file.name }));
      } else {
        accepted.push({
          file,
          title: file.name.replace(/\.[^/.]+$/, ""),
          positionInQueue: videos.length + accepted.length + 1,
        });
      }
    });

    setVideos((prev) => [...prev, ...accepted]);
    if (rejected.length > 0) setError(rejected.join("\n"));
  };

  useEffect(() => {
    const queryParams = new URLSearchParams(location.search);
    const groupId = queryParams.get("videogroupId");

    if (!groupId) {
      setError(t('errors.load_video_group'));
      return;
    }

    const parsedId = parseInt(groupId);
    setVideoGroupId(parsedId);
    fetchVideoGroupName(parsedId);
  }, [location.search, t]);

  const fetchVideoGroupName = async (id: number) => {
    try {
      const group = await getVideoGroup(id);
      setVideoGroupName(group.name);
    } catch {
      setError(t('errors.load_video_group_details'));
    }
  };

  const handleDrop = (e) => {
    e.preventDefault();
    const droppedFiles = Array.from(e.dataTransfer.files);

    const accepted = [];
    const rejected = [];

    droppedFiles.forEach((file) => {
      if (!file.type.startsWith("video/")) {
        rejected.push(t('upload.error_not_video', { filename: file.name }));
      } else if (file.size > 100 * 1024 * 1024) {
        rejected.push(t('upload.error_size', { filename: file.name }));
      } else {
        accepted.push({
          file,
          title: file.name.replace(/\.[^/.]+$/, ""),
          positionInQueue: videos.length + accepted.length + 1,
        });
      }
    });

    setVideos((prev) => [...prev, ...accepted]);
    if (rejected.length > 0) setError(rejected.join("\n"));
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
    setError("");
    setUploadProgress(0);

    if (videos.length === 0) {
      setError(t('upload.error_empty'));
      setLoading(false);
      return;
    }

    try {
      // Track individual upload progress
      const progressPerVideo = Array(videos.length).fill(0);

      const uploadPromises = videos.map((video, index) => {
        const formDataObj = new FormData();
        formDataObj.append("Title", video.title);
        formDataObj.append("VideoGroupId", videoGroupId);
        formDataObj.append("PositionInQueue", video.positionInQueue);
        formDataObj.append("File", video.file);

        return createVideo(formDataObj, (progressEvent) => {
          const percentCompleted = Math.round(
            (progressEvent.loaded * 100) / progressEvent.total
          );
          progressPerVideo[index] = percentCompleted;
          const overallProgress = Math.round(
            progressPerVideo.reduce((a, b) => a + b, 0) / videos.length
          );
          setUploadProgress(overallProgress);
        });
      });

      await Promise.all(uploadPromises);

      navigate(`/video-groups/${videoGroupId}`);
    } catch (err) {
      setError(
        err.response?.data?.message ||
          t('upload.error_general')
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
          <h1 className="heading mb-0">{t('upload.title')}</h1>
        </div>
        <div className="card-body">
          {error && (
            <div className="alert alert-danger">
              <i className="fas fa-exclamation-triangle me-2"></i>
              {error}
            </div>
          )}

          <div
            ref={dropRef}
            className="drop-area border border-primary rounded mb-4 p-4 text-center"
            onDrop={handleDrop}
            onDragOver={handleDragOver}
            style={{ background: "#f8f9fa", cursor: "pointer" }}
          >
            <i className="fas fa-cloud-upload-alt fa-2x mb-2"></i>
            <p className="mb-0">{t('upload.drag_drop')}</p>
            <small className="text-muted">{t('upload.max_size')}</small>

            {/* Add a div to move the button to a new line */}
            <div className="mt-3">
              <button
                type="button"
                className="btn btn-secondary"
                onClick={handleButtonClick}
              >
                {t('buttons.select_files')}
              </button>
            </div>

            <input
              type="file"
              ref={fileInputRef}
              style={{ display: "none" }}
              multiple
              accept="video/*"
              onChange={handleFileSelect}
            />
          </div>

          {videos.length > 0 && (
            <form onSubmit={handleSubmit}>
              <table className="table table-bordered">
                <thead className="table-light">
                  <tr>
                    <th>{t('table.title')}</th>
                    <th>{t('upload.max_size').split(':')[0]}</th>
                    <th>{t('table.position')}</th>
                    <th>{t('common:actions')}</th>
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
                          <button
                            type="button"
                            className="btn btn-danger"
                            onClick={() => handleRemove(index)}
                            disabled={loading}
                          >
                            <i className="fas fa-trash-alt"></i> {t('buttons.remove')}
                          </button>
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>

              {loading && (
                <div className="mb-3">
                  <label className="form-label">{t('upload.progress')}</label>
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
                  {loading ? t('buttons.uploading') : t('buttons.upload_all')}
                </button>
              </div>
            </form>
          )}

          {videoGroupId && (
            <input type="hidden" name="videoGroupId" value={videoGroupId} />
          )}
        </div>
      </div>
      <NavigateButton
          path={`/video-groups/${videoGroupId}`}
          actionType="Back"
          value={t('common:buttons.cancel')}
      />
    </div>
  );
};

export default VideoAdd;
