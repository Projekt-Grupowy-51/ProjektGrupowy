import React from 'react';
import { useTranslation } from 'react-i18next';
import VideoGroupForm from '../components/forms/VideoGroupForm.jsx';
import FormPageWrapper from '../components/forms/FormPageWrapper.jsx';
import { LoadingSpinner, ErrorAlert } from '../components/common';
import { useVideoGroupEdit } from '../hooks/useVideoGroupEdit.js';

const VideoGroupEditPage = () => {
  const { t } = useTranslation(['videoGroups', 'common']);
  const {
    videoGroup,
    loading,
    error,
    submitLoading,
    handleSubmit,
    handleCancel
  } = useVideoGroupEdit();

  if (loading) return <LoadingSpinner />;

  if (error) return <ErrorAlert error={error} />;

  if (!videoGroup) return <ErrorAlert error="VideoGroup not found" />;

  return (
    <FormPageWrapper
      title={t('videoGroups:edit_title')}
      subtitle={videoGroup?.name}
      maxWidth={700}
      onBack={handleCancel}
    >
      <VideoGroupForm
        projectId={videoGroup?.projectId}
        initialData={videoGroup}
        onSubmit={handleSubmit}
        onCancel={handleCancel}
        loading={submitLoading}
        submitText={t('common:buttons.save')}
        isEdit
      />
    </FormPageWrapper>
  );
};

export default VideoGroupEditPage;