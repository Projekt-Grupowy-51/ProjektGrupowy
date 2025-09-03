import React, { useRef } from 'react';
import PropTypes from 'prop-types';
import { useTranslation } from 'react-i18next';
import { Button, Input, Alert, Table, TableHead, TableBody, TableRow, TableCell } from '../ui';
import { useVideoUpload } from '../../hooks/forms/useVideoUpload.js';

const VideoForm = ({
  onSubmit,
  onCancel,
  loading = false,
  uploadProgress = 0,
  submitText,
  cancelText,
  videoGroupId,
  videoGroupName
}) => {
  const { t } = useTranslation(['videos', 'common']);
  const fileInputRef = useRef(null);
  const dropRef = useRef(null);

  // Use video upload hook
  const {
    videos,
    error,
    hasVideos,
    handleFileSelect,
    handleDrop,
    handleDragOver,
    updateVideo,
    removeVideo,
    validateVideos,
    setErrorMessage,
    clearError
  } = useVideoUpload();

  const handleButtonClick = () => {
    fileInputRef.current.click();
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    clearError();

    const validation = validateVideos();
    if (!validation.isValid) {
      return;
    }

    try {
      await onSubmit(videos, videoGroupId);
    } catch (error) {
      setErrorMessage(error.message || t('upload.error_general'));
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      {error && <Alert variant="danger">{error}</Alert>}

      <div
        ref={dropRef}
        className="drop-area border border-primary rounded mb-4 p-4 text-center"
        onDrop={handleDrop}
        onDragOver={handleDragOver}
        style={{ background: '#f8f9fa', cursor: 'pointer' }}
      >
        <i className="fas fa-cloud-upload-alt fa-2x mb-2"></i>
        <p className="mb-0">{t('upload.drag_drop')}</p>
        <small className="text-muted">{t('upload.max_size')}</small>

        <div className="mt-3">
          <Button
            type="button"
            variant="secondary"
            onClick={handleButtonClick}
          >
            {t('buttons.select_files')}
          </Button>
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

      {hasVideos && (
        <>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>{t('table.title')}</TableCell>
                <TableCell>{t('upload.max_size').split(':')[0]}</TableCell>
                <TableCell>{t('table.position')}</TableCell>
                <TableCell>{t('common:actions')}</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {videos.map((video, index) => (
                <TableRow key={index}>
                  <TableCell>
                    <Input
                      name={`video-title-${index}`}
                      type="text"
                      value={video.title}
                      onChange={(e) =>
                        updateVideo(index, 'title', e.target.value)
                      }
                      disabled={loading}
                      required
                    />
                  </TableCell>
                  <TableCell>
                    {(video.file.size / (1024 * 1024)).toFixed(2)} MB
                  </TableCell>
                  <TableCell>
                    <Input
                      name={`video-position-${index}`}
                      type="number"
                      value={video.positionInQueue}
                      min="1"
                      onChange={(e) =>
                        updateVideo(index, 'positionInQueue', e.target.value)
                      }
                      disabled={loading}
                      required
                    />
                  </TableCell>
                  <TableCell>
                    <div className="d-flex justify-content-center">
                      <Button
                        type="button"
                        variant="danger"
                        onClick={() => removeVideo(index)}
                        disabled={loading}
                        icon="fas fa-trash-alt"
                      >
                        {t('buttons.remove')}
                      </Button>
                    </div>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>

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

          <div className="d-flex justify-content-end gap-2">
            <Button
              type="submit"
              variant="primary"
              loading={loading}
              icon="fas fa-upload"
            >
              {loading ? t('buttons.uploading') : (submitText || t('buttons.upload_all'))}
            </Button>
            {onCancel && (
              <Button
                type="button"
                variant="secondary"
                onClick={onCancel}
                disabled={loading}
              >
                {cancelText || t('common:buttons.cancel')}
              </Button>
            )}
          </div>
        </>
      )}

      {videoGroupId && (
        <Input type="hidden" name="videoGroupId" value={videoGroupId} onChange={() => {}} />
      )}
    </form>
  );
};

VideoForm.propTypes = {
  onSubmit: PropTypes.func.isRequired,
  onCancel: PropTypes.func,
  loading: PropTypes.bool,
  uploadProgress: PropTypes.number,
  submitText: PropTypes.string,
  cancelText: PropTypes.string,
  videoGroupId: PropTypes.number,
  videoGroupName: PropTypes.string
};

export default VideoForm;