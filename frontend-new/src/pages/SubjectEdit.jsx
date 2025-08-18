import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import SubjectForm from '../components/forms/SubjectForm.jsx';
import FormPageWrapper from '../components/forms/FormPageWrapper.jsx';
import { FAKE_SUBJECTS, findById, updateInCollection } from '../data/fakeData.js';

const SubjectEditPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { t } = useTranslation(['subjects', 'common']);

  const [subject, setSubject] = useState(null);
  const [loading, setLoading] = useState(true);
  const [updateLoading, setUpdateLoading] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    const loadSubject = async () => {
      setLoading(true);
      await new Promise(resolve => setTimeout(resolve, 500));

      const foundSubject = findById(FAKE_SUBJECTS, id);
      if (foundSubject) {
        setSubject(foundSubject);
      } else {
        setError(t('subjects:messages.not_found'));
      }
      setLoading(false);
    };

    if (id) loadSubject();
  }, [id, t]);

  const updateSubject = async (subjectId, subjectData) => {
    setUpdateLoading(true);
    await new Promise(resolve => setTimeout(resolve, 1000));

    const updated = updateInCollection(FAKE_SUBJECTS, subjectId, subjectData);
    if (updated) {
      console.log('Updated subject:', updated);
      console.log('Current subjects collection:', FAKE_SUBJECTS);
    }
    setUpdateLoading(false);
  };

  const handleSubmit = async (subjectData) => {
    try {
      await updateSubject(id, subjectData);
      navigate(`/subjects/${id}`);
    } catch (err) {
      console.error('Failed to update subject:', err);
    }
  };

  const handleCancel = () => navigate(`/subjects/${id}`);

  if (loading) return <FormPageWrapper loading />;

  if (error) return <FormPageWrapper error={error} onBack={() => navigate(`/subjects/${id}`)} />;

  return (
      <FormPageWrapper
          title={t('subjects:edit_title')}
          subtitle={subject.name}
          maxWidth={700}
          onBack={() => navigate(`/subjects/${id}`)}
      >
        <SubjectForm
            projectId={subject.projectId}
            initialData={subject}
            onSubmit={handleSubmit}
            onCancel={handleCancel}
            loading={updateLoading}
            submitText={t('common:buttons.save')}
            isEdit
        />
      </FormPageWrapper>
  );
};

export default SubjectEditPage;
