import React, { useState, useEffect } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import LabelForm from '../components/forms/LabelForm';
import FormPageWrapper from '../components/forms/FormPageWrapper.jsx';
import { FAKE_SUBJECTS, FAKE_LABELS, findById, addToCollection } from '../data/fakeData.js';

const LabelAdd = () => {
  const { t } = useTranslation(['labels', 'common']);
  const [searchParams] = useSearchParams();
  const subjectId = searchParams.get('subjectId');
  const navigate = useNavigate();

  const [subject, setSubject] = useState(null);
  const [loading, setLoading] = useState(false);
  const [subjectLoading, setSubjectLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const loadSubject = async () => {
      setSubjectLoading(true);
      await new Promise(resolve => setTimeout(resolve, 500));

      const foundSubject = findById(FAKE_SUBJECTS, subjectId);
      if (foundSubject) {
        setSubject(foundSubject);
      } else {
        setError(t('labels:messages.subject_not_found'));
      }
      setSubjectLoading(false);
    };

    if (subjectId) loadSubject();
  }, [subjectId, t]);

  const handleSubmit = async (formData) => {
    setLoading(true);
    await new Promise(resolve => setTimeout(resolve, 1000));

    addToCollection(FAKE_LABELS, {
      ...formData,
      subjectId: parseInt(subjectId),
      createdAt: new Date().toISOString()
    });

    setLoading(false);
    navigate(`/subjects/${subjectId}`);
  };

  const handleCancel = () => navigate(`/subjects/${subjectId}`);

  return (
      <FormPageWrapper
          title={t('labels:add_title')}
          subtitle={subject ? `${t('labels:for_subject')}: ${subject.name}` : ''}
          maxWidth={700}
      >
        {subjectLoading ? (
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
            <LabelForm
                subjectId={parseInt(subjectId)}
                subjectName={subject?.name}
                onSubmit={handleSubmit}
                onCancel={handleCancel}
                loading={loading}
                submitText={t('labels:buttons.create')}
            />
        )}
      </FormPageWrapper>
  );
};

export default LabelAdd;
