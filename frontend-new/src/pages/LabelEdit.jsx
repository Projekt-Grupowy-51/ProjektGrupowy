import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import LabelForm from '../components/forms/LabelForm';
import FormPageWrapper from '../components/forms/FormPageWrapper.jsx';
import { FAKE_SUBJECTS, FAKE_LABELS, findById, updateInCollection } from '../data/fakeData.js';

const LabelEdit = () => {
  const { t } = useTranslation(['labels', 'common']);
  const { id } = useParams();
  const navigate = useNavigate();

  const [label, setLabel] = useState(null);
  const [subject, setSubject] = useState(null);
  const [loading, setLoading] = useState(true);
  const [updateLoading, setUpdateLoading] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    const loadData = async () => {
      try {
        setLoading(true);

        // Load label
        const labelData = await new Promise(resolve => {
          setTimeout(() => resolve(findById(FAKE_LABELS, parseInt(id))), 500);
        });

        if (!labelData) {
          setError(t('labels:messages.label_not_found'));
          return;
        }

        setLabel(labelData);

        // Load subject
        const subjectData = await new Promise(resolve => {
          setTimeout(() => resolve(findById(FAKE_SUBJECTS, labelData.subjectId)), 300);
        });

        if (!subjectData) {
          setError(t('labels:messages.subject_not_found'));
          return;
        }

        setSubject(subjectData);
      } catch (err) {
        console.error('Error loading data:', err);
        setError(t('labels:errors.loadFailed'));
      } finally {
        setLoading(false);
      }
    };

    if (id) loadData();
  }, [id, t]);

  const handleSubmit = async (formData) => {
    try {
      setUpdateLoading(true);

      await new Promise(resolve => {
        setTimeout(() => {
          updateInCollection(FAKE_LABELS, parseInt(id), formData);
          resolve();
        }, 1000);
      });

      navigate(`/subjects/${label.subjectId}`);
    } catch (err) {
      console.error('Error updating label:', err);
    } finally {
      setUpdateLoading(false);
    }
  };

  const handleCancel = () => navigate(`/subjects/${label?.subjectId}`);

  return (
      <FormPageWrapper
          title={t('labels:edit_title')}
          subtitle={subject ? `${t('labels:for_subject')}: ${subject.name}` : ''}
          maxWidth={700}
      >
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
            <LabelForm
                initialData={label}
                onSubmit={handleSubmit}
                onCancel={handleCancel}
                loading={updateLoading}
                submitText={t('labels:buttons.save')}
            />
        )}
      </FormPageWrapper>
  );
};

export default LabelEdit;
