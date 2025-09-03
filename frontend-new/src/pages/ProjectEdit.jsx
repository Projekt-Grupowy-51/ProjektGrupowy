import React from 'react';
import { useTranslation } from 'react-i18next';
import ProjectForm from '../components/forms/ProjectForm.jsx';
import FormPageWrapper from '../components/forms/FormPageWrapper.jsx';
import { LoadingSpinner, ErrorAlert } from '../components/common';
import { useProjectEdit } from '../hooks/useProjectEdit.js';

const ProjectEditPage = () => {
  const { t } = useTranslation(['projects', 'common']);
  const {
    project,
    loading,
    error,
    submitLoading,
    handleSubmit,
    handleCancel
  } = useProjectEdit();

  return (
    <FormPageWrapper title={t('projects:edit_title')} maxWidth={700}>
      {loading ? (
        <LoadingSpinner />
      ) : error ? (
        <ErrorAlert error={error} />
      ) : (
        <ProjectForm
          initialData={project}
          onSubmit={handleSubmit}
          onCancel={handleCancel}
          loading={submitLoading}
          submitText={t('projects:buttons.save')}
        />
      )}
    </FormPageWrapper>
  );
};

export default ProjectEditPage;
