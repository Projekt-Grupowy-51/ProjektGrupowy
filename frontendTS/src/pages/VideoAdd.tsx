import React from "react";
import { useLocation } from "react-router-dom";
import "./css/ScientistProjects.css";
import { useTranslation } from "react-i18next";
import NavigateButton from "../components/NavigateButton";
import useVideoUpload from "../hooks/useVideoUpload";

const VideoAdd = () => {
  const { t } = useTranslation(['videos', 'common']);
  const location = useLocation();
  const {
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
  } = useVideoUpload(location.search);

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
              style={{ display: 'none' }}
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
                          onChange={(e) => handleInputChange(index, 'title', e.target.value)}
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
                          onChange={(e) => handleInputChange(index, 'positionInQueue', e.target.value)}
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
                <button type="submit" className="btn btn-primary" disabled={loading}>
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
