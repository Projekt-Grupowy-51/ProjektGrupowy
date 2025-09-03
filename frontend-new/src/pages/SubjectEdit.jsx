import React from 'react';
import { useTranslation } from 'react-i18next';
import SubjectForm from '../components/forms/SubjectForm.jsx';
import FormPageWrapper from '../components/forms/FormPageWrapper.jsx';
import { useSubjectEdit } from '../hooks/useSubjectEdit.js';

const SubjectEditPage = () => {
  const { t } = useTranslation(['subjects', 'common']);
  const {
    data: subject,
    loading,
    error,
    updateLoading,
    handleSubmit,
    handleCancel
  } = useSubjectEdit();

  if (loading) return <FormPageWrapper loading />;

  if (error) return <FormPageWrapper error={error} onBack={handleCancel} />;

  return (
      <FormPageWrapper
          title={t('subjects:edit_title')}
          subtitle={subject?.name}
          maxWidth={700}
          onBack={handleCancel}
      >
        <SubjectForm
            projectId={subject?.projectId}
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
