import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import ProjectForm from '../components/forms/ProjectForm.jsx';
import FormPageWrapper from '../components/forms/FormPageWrapper.jsx';
import { FAKE_PROJECTS, findById, updateInCollection } from '../data/fakeData.js';

const ProjectEditPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { t } = useTranslation(['projects', 'common']);

  const [project, setProject] = useState(null);
  const [loading, setLoading] = useState(true);
  const [updateLoading, setUpdateLoading] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    const loadProject = async () => {
      setLoading(true);
      await new Promise(resolve => setTimeout(resolve, 500));

      const foundProject = findById(FAKE_PROJECTS, id);
      if (foundProject) {
        setProject(foundProject);
      } else {
        setError(t('projects:messages.project_not_found'));
      }
      setLoading(false);
    };

    if (id) loadProject();
  }, [id, t]);

  const handleSubmit = async (projectData) => {
    setUpdateLoading(true);
    await new Promise(resolve => setTimeout(resolve, 1000));
    updateInCollection(FAKE_PROJECTS, id, projectData);
    setUpdateLoading(false);
    navigate(`/projects/${id}`);
  };

  const handleCancel = () => navigate(`/projects/${id}`);

  return (
      <FormPageWrapper title={t('projects:edit_title')} maxWidth={700}>
        {loading ? (
            <div className="text-center py-4">
              <div className="spinner-border" role="status">
                <span className="visually-hidden">Loading...</span>
              </div>
            </div>
        ) : error ? (
            <div className="alert alert-danger" role="alert">
              {error}
            </div>
        ) : (
            <ProjectForm
                initialData={project}
                onSubmit={handleSubmit}
                onCancel={handleCancel}
                loading={updateLoading}
                submitText={t('projects:buttons.save')}
            />
        )}
      </FormPageWrapper>
  );
};

export default ProjectEditPage;
