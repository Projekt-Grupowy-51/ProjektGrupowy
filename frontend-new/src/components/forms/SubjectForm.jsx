import React from 'react';
import PropTypes from 'prop-types';
import { useTranslation } from 'react-i18next';
import { Input, Button } from '../ui';
import { useBaseForm } from '../../hooks/forms/useBaseForm.js';
import { ValidationRules } from '../../utils/formValidation.js';

const SubjectForm = ({ 
  initialData = {}, 
  onSubmit, 
  onCancel, 
  loading = false, 
  submitText = 'Save',
  projectId = null
}) => {
  const { t } = useTranslation(['subjects', 'common']);
  
  // Initialize form data
  const defaultData = {
    name: initialData?.name || '',
    description: initialData?.description || '',
    projectId: initialData?.projectId || projectId || ''
  };

  // Setup validation rules
  const validationRules = {
    name: ValidationRules.name(t),
    description: ValidationRules.description(t)
  };

  // Use base form hook
  const {
    formData,
    errors,
    isSubmitting,
    handleChange,
    handleSubmit
  } = useBaseForm(defaultData, validationRules);

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <Input
        label={t('subjects:form.name')}
        name="name"
        value={formData.name}
        onChange={handleChange}
        error={errors.name}
        required
        disabled={loading || isSubmitting}
      />

      <Input
        label={t('subjects:form.description')}
        name="description"
        type="textarea"
        value={formData.description}
        onChange={handleChange}
        error={errors.description}
        required
        disabled={loading || isSubmitting}
        rows={4}
      />

      <div className="d-flex gap-2">
        <Button
          type="submit"
          variant="primary"
          icon="fas fa-save"
          loading={loading || isSubmitting}
        >
          {(loading || isSubmitting) ? t('subjects:buttons.adding') : submitText}
        </Button>
        
        <Button
          type="button"
          variant="outline-secondary"
          icon="fas fa-times"
          onClick={onCancel}
          disabled={loading || isSubmitting}
        >
          {t('common:buttons.cancel')}
        </Button>
      </div>
    </form>
  );
};

SubjectForm.propTypes = {
  initialData: PropTypes.object,
  onSubmit: PropTypes.func.isRequired,
  onCancel: PropTypes.func.isRequired,
  loading: PropTypes.bool,
  submitText: PropTypes.string,
  projectId: PropTypes.oneOfType([PropTypes.string, PropTypes.number])
};

export default SubjectForm;
