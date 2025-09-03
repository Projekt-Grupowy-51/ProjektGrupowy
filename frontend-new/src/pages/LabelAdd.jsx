import React from 'react';
import { useTranslation } from 'react-i18next';
import LabelForm from '../components/forms/LabelForm';
import FormPageWrapper from '../components/forms/FormPageWrapper.jsx';
import { LoadingSpinner, ErrorAlert } from '../components/common';
import { useLabelAdd } from '../hooks/useLabelAdd.js';

const LabelAdd = () => {
  const { t } = useTranslation(['labels', 'common']);
  const {
    subject,
    subjectId,
    subjectLoading,
    submitLoading,
    error,
    handleSubmit,
    handleCancel
  } = useLabelAdd();

  return (
    <FormPageWrapper
      title={t('labels:add_title')}
      subtitle={subject ? `${t('labels:for_subject')}: ${subject.name}` : ''}
      maxWidth={700}
    >
      {subjectLoading ? (
        <LoadingSpinner message="Loading subject..." />
      ) : error ? (
        <ErrorAlert error={error} />
      ) : (
        <LabelForm
          subjectId={subjectId}
          subjectName={subject?.name}
          onSubmit={handleSubmit}
          onCancel={handleCancel}
          loading={submitLoading}
          submitText={t('labels:buttons.create')}
        />
      )}
    </FormPageWrapper>
  );
};

export default LabelAdd;
