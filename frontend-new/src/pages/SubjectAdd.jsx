import React, { useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import SubjectForm from '../components/forms/SubjectForm.jsx';
import FormPageWrapper from '../components/forms/FormPageWrapper.jsx';
import { FAKE_SUBJECTS, addToCollection } from '../data/fakeData.js';

const SubjectAddPage = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const { t } = useTranslation(['subjects', 'common']);

  const [loading, setLoading] = useState(false);

  const projectId = new URLSearchParams(location.search).get("projectId");

  const createSubject = async (subjectData) => {
    setLoading(true);
    await new Promise(resolve => setTimeout(resolve, 1000));

    const newSubject = addToCollection(FAKE_SUBJECTS, {
      ...subjectData,
      projectId: parseInt(projectId)
    });

    console.log('Created subject:', newSubject);
    console.log('Current subjects collection:', FAKE_SUBJECTS);
    setLoading(false);
  };

  const handleSubmit = async (subjectData) => {
    try {
      await createSubject(subjectData);
      navigate(`/projects/${projectId}`);
    } catch (error) {
      console.error('Failed to create subject:', error);
    }
  };

  const handleCancel = () => navigate(`/projects/${projectId}`);

  return (
      <FormPageWrapper
          title={t('subjects:add_title')}
          maxWidth={700}
      >
        <SubjectForm
            projectId={projectId}
            onSubmit={handleSubmit}
            onCancel={handleCancel}
            loading={loading}
            submitText={t('subjects:buttons.add')}
        />
      </FormPageWrapper>
  );
};

export default SubjectAddPage;
