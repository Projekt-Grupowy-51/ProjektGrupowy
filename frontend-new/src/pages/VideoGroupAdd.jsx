import React, { useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Container, Card } from '../components/ui';
import VideoGroupForm from '../components/forms/VideoGroupForm.jsx';
import { FAKE_VIDEO_GROUPS, addToCollection } from '../data/fakeData.js';

const VideoGroupAdd = () => {
  const { t } = useTranslation(['videoGroups', 'common']);
  const [searchParams] = useSearchParams();
  const projectId = searchParams.get('projectId');
  const navigate = useNavigate();
  
  // Stan komponentu
  const [loading, setLoading] = useState(false);

  const createVideoGroup = async (videoGroupData) => {
    setLoading(true);
    // Symulacja dodawania do kolekcji
    await new Promise(resolve => setTimeout(resolve, 1000));
    
    const newVideoGroup = addToCollection(FAKE_VIDEO_GROUPS, {
      ...videoGroupData,
      createdAt: new Date().toISOString()
    });
    
    console.log('Created video group:', newVideoGroup);
    console.log('Current video groups collection:', FAKE_VIDEO_GROUPS);
    setLoading(false);
  };

  const handleSubmit = async (videoGroupData) => {
    try {
      const dataWithProject = {
        ...videoGroupData,
        projectId: projectId ? parseInt(projectId) : null
      };
      await createVideoGroup(dataWithProject);
      navigate(`/projects/${projectId}`);
    } catch (error) {
      console.error('Failed to create video group:', error);
      // Error handling could be improved with toast notifications
    }
  };

  const handleCancel = () => {
    navigate(`/projects/${projectId}`);
  };

  return (
    <Container className="py-4">
      <Container.Row className="justify-content-center">
        <Container.Col lg={8}>
          <Card className="shadow-sm">
            <Card.Header variant="primary">
              <Card.Title level={1} className="mb-0">
                {t('videoGroups:add_title')}
              </Card.Title>
            </Card.Header>
            <Card.Body>
              <VideoGroupForm
                onSubmit={handleSubmit}
                onCancel={handleCancel}
                loading={loading}
                submitText={t('videoGroups:buttons.create')}
              />
            </Card.Body>
          </Card>
        </Container.Col>
      </Container.Row>
    </Container>
  );
};

export default VideoGroupAdd;
