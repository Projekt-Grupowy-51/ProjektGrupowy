import React from 'react';
import { useTranslation } from 'react-i18next';
import VideoForm from '../components/forms/VideoForm.jsx';
import FormPageWrapper from '../components/forms/FormPageWrapper.jsx';
import { useVideoAdd } from '../hooks/useVideoAdd.js';

const VideoAdd = () => {
  const { t } = useTranslation(['videos', 'common']);
  const {
    videoGroupId,
    videoGroupName,
    loading,
    uploadProgress,
    error,
    handleSubmit,
    handleCancel
  } = useVideoAdd();

  return (
    <FormPageWrapper title={t('upload.title')} maxWidth={900}>
      <VideoForm
        onSubmit={handleSubmit}
        onCancel={handleCancel}
        loading={loading}
        uploadProgress={uploadProgress}
        videoGroupId={videoGroupId}
        videoGroupName={videoGroupName}
        error={error}
        submitText={t('buttons.upload_all')}
      />
    </FormPageWrapper>
  );
};

export default VideoAdd;