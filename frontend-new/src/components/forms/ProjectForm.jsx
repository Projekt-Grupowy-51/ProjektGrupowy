import React, { useEffect } from 'react';
import PropTypes from 'prop-types';
import { useTranslation } from 'react-i18next';
import { Button, Input } from '../ui';
import { useBaseForm } from '../../hooks/forms/useBaseForm.js';
import { ValidationRules } from '../../utils/formValidation.js';

const ProjectForm = ({
                       initialData = {},
                       onSubmit,
                       onCancel,
                       loading = false,
                       submitText,
                       cancelText,
                       showFinishedCheckbox = true
                     }) => {
  const { t } = useTranslation(['projects', 'common']);
  
  // Initialize form data
  const defaultData = {
    name: initialData?.name || '',
    description: initialData?.description || '',
    finished: initialData?.finished ?? false
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
    handleSubmit,
    resetToData
  } = useBaseForm(defaultData, validationRules);

  // Update form when initialData changes
  useEffect(() => {
    if (initialData && Object.keys(initialData).length > 0) {
      const updatedData = {
        name: initialData?.name || '',
        description: initialData?.description || '',
        finished: initialData?.finished ?? false
      };
      resetToData(updatedData);
    }
  }, [initialData, resetToData]);

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <Input
        label={t('projects:form.name')}
        name="name"
        type="text"
        value={formData.name}
        onChange={handleChange}
        required
        error={errors.name}
        disabled={loading || isSubmitting}
      />

      <Input
        label={t('projects:form.description')}
        name="description"
        type="textarea"
        value={formData.description}
        onChange={handleChange}
        required
        rows={4}
        error={errors.description}
        disabled={loading || isSubmitting}
      />

      {showFinishedCheckbox && (
        <Input
          label={t('projects:form.finished')}
          name="finished"
          type="checkbox"
          checked={formData.finished}
          onChange={handleChange}
          disabled={loading || isSubmitting}
        />
      )}

      <div className="d-flex gap-2">
        <Button 
          type="submit" 
          variant="primary" 
          loading={loading || isSubmitting} 
          icon="fas fa-save"
        >
          {submitText || t('projects:buttons.save')}
        </Button>
        {onCancel && (
          <Button 
            type="button" 
            variant="secondary" 
            onClick={onCancel} 
            disabled={loading || isSubmitting}
          >
            {cancelText || t('common:buttons.cancel')}
          </Button>
        )}
      </div>
    </form>
  );
};

ProjectForm.propTypes = {
  initialData: PropTypes.shape({
    name: PropTypes.string,
    description: PropTypes.string,
    finished: PropTypes.bool
  }),
  onSubmit: PropTypes.func.isRequired,
  onCancel: PropTypes.func,
  loading: PropTypes.bool,
  submitText: PropTypes.string,
  cancelText: PropTypes.string,
  showFinishedCheckbox: PropTypes.bool
};

export default ProjectForm;
