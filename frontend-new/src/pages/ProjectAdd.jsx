import React from 'react';
import { useTranslation } from 'react-i18next';
import ProjectForm from '../components/forms/ProjectForm.jsx';
import FormPageWrapper from '../components/forms/FormPageWrapper.jsx';
import { useProjectAdd } from '../hooks/useProjectAdd.js';

const ProjectAddPage = () => {
  const { t } = useTranslation(['projects', 'common']);
  const { handleSubmit, handleCancel, loading, error } = useProjectAdd();

  return (
    <FormPageWrapper title={t('projects:add_title')} maxWidth={700}>
      <ProjectForm
        onSubmit={handleSubmit}
        onCancel={handleCancel}
        loading={loading}
        error={error}
        submitText={t('projects:buttons.create')}
        showFinishedCheckbox={false}
      />
    </FormPageWrapper>
  );
};

export default ProjectAddPage;
