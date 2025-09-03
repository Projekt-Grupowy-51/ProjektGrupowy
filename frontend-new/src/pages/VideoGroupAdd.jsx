import React from 'react';
import { useTranslation } from 'react-i18next';
import VideoGroupForm from '../components/forms/VideoGroupForm.jsx';
import FormPageWrapper from '../components/forms/FormPageWrapper.jsx';
import { useVideoGroupAdd } from '../hooks/useVideoGroupAdd.js';

const VideoGroupAdd = () => {
  const { t } = useTranslation(['videoGroups', 'common']);
  const { projectId, handleSubmit, handleCancel, loading, error } = useVideoGroupAdd();

  return (
    <FormPageWrapper
      title={t('videoGroups:add_title')}
      maxWidth={700}
      onBack={handleCancel}
    >
      <VideoGroupForm
        onSubmit={handleSubmit}
        onCancel={handleCancel}
        loading={loading}
        error={error}
        submitText={t('videoGroups:buttons.create')}
      />
    </FormPageWrapper>
  );
};

export default VideoGroupAdd;
