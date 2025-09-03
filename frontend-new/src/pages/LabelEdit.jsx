import React from 'react';
import { useTranslation } from 'react-i18next';
import LabelForm from '../components/forms/LabelForm';
import FormPageWrapper from '../components/forms/FormPageWrapper.jsx';
import { LoadingSpinner, ErrorAlert } from '../components/common';
import { useLabelEdit } from '../hooks/useLabelEdit.js';

const LabelEdit = () => {
  const { t } = useTranslation(['labels', 'common']);
  const {
    label,
    subject,
    loading,
    error,
    updateLoading,
    handleSubmit,
    handleCancel
  } = useLabelEdit();

  return (
      <FormPageWrapper
          title={t('labels:edit_title')}
          subtitle={subject ? `${t('labels:for_subject')}: ${subject.name}` : ''}
          maxWidth={700}
      >
        {loading ? (
            <LoadingSpinner />
        ) : error ? (
            <ErrorAlert error={error} />
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
