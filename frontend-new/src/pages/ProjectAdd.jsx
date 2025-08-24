import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import ProjectForm from '../components/forms/ProjectForm.jsx';
import FormPageWrapper from '../components/forms/FormPageWrapper.jsx';
import { FAKE_PROJECTS, addToCollection } from '../data/fakeData.js';

const ProjectAddPage = () => {
  const navigate = useNavigate();
  const { t } = useTranslation(['projects', 'common']);
  const [loading, setLoading] = useState(false);

  const createProject = async (projectData) => {
    setLoading(true);
    await new Promise(resolve => setTimeout(resolve, 1000));
    addToCollection(FAKE_PROJECTS, {
      ...projectData,
      createdAt: new Date().toISOString(),
      status: 'active'
    });
    setLoading(false);
  };

  const handleSubmit = async (projectData) => {
    try {
      await createProject(projectData);
      navigate('/projects');
    } catch (error) {
      console.error('Failed to create project:', error);
    }
  };

  const handleCancel = () => navigate('/projects');

  return (
      <FormPageWrapper title={t('projects:add_title')} maxWidth={700}>
        <ProjectForm
            onSubmit={handleSubmit}
            onCancel={handleCancel}
            loading={loading}
            submitText={t('projects:buttons.create')}
            showFinishedCheckbox={false}
        />
      </FormPageWrapper>
  );
};

export default ProjectAddPage;
