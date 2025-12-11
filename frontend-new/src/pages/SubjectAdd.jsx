import React from 'react';
import { useTranslation } from 'react-i18next';
import SubjectForm from '../components/forms/SubjectForm.jsx';
import FormPageWrapper from '../components/forms/FormPageWrapper.jsx';
import { useSubjectAdd } from '../hooks/useSubjectAdd.js';

const SubjectAddPage = () => {
  const { t } = useTranslation(['subjects', 'common']);
  const { projectId, handleSubmit, handleCancel, loading, error } = useSubjectAdd();

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
        error={error}
        submitText={t('subjects:buttons.add')}
      />
    </FormPageWrapper>
  );
};

export default SubjectAddPage;
