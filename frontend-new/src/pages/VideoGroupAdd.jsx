import React, { useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import VideoGroupForm from '../components/forms/VideoGroupForm.jsx';
import FormPageWrapper from '../components/forms/FormPageWrapper.jsx';
import { FAKE_VIDEO_GROUPS, addToCollection } from '../data/fakeData.js';

const VideoGroupAdd = () => {
  const { t } = useTranslation(['videoGroups', 'common']);
  const [searchParams] = useSearchParams();
  const projectId = searchParams.get('projectId');
  const navigate = useNavigate();

  const [loading, setLoading] = useState(false);

  const createVideoGroup = async (videoGroupData) => {
    setLoading(true);
    await new Promise(resolve => setTimeout(resolve, 1000));

    const newVideoGroup = addToCollection(FAKE_VIDEO_GROUPS, {
      ...videoGroupData,
      projectId: projectId ? parseInt(projectId) : null,
      createdAt: new Date().toISOString()
    });

    console.log('Created video group:', newVideoGroup);
    console.log('Current video groups collection:', FAKE_VIDEO_GROUPS);
    setLoading(false);
  };

  const handleSubmit = async (videoGroupData) => {
    try {
      await createVideoGroup(videoGroupData);
      navigate(`/projects/${projectId}`);
    } catch (error) {
      console.error('Failed to create video group:', error);
    }
  };

  const handleCancel = () => navigate(`/projects/${projectId}`);

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
            submitText={t('videoGroups:buttons.create')}
        />
      </FormPageWrapper>
  );
};

export default VideoGroupAdd;
