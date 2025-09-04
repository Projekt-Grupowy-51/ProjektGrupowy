import React from 'react';
import PropTypes from 'prop-types';
import { useTranslation } from 'react-i18next';
import { Button, Input } from '../ui';
import { useBaseForm } from '../../hooks/forms/useBaseForm.js';
import { ValidationRules } from '../../utils/formValidation.js';

const VideoGroupForm = ({
                          initialData = {},
                          onSubmit,
                          onCancel,
                          loading = false,
                          submitText,
                          cancelText
                        }) => {
  const { t } = useTranslation(['videoGroups', 'common']);
  
  // Initialize form data
  const defaultData = {
    name: initialData?.name || '',
    description: initialData?.description || '',
    projectId: initialData?.projectId || null
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
        label={t('videoGroups:form.name')}
        name="name"
        value={formData.name}
        onChange={handleChange}
        required
        error={errors.name}
        disabled={loading || isSubmitting}
      />

      <Input
        label={t('videoGroups:form.description')}
        name="description"
        type="textarea"
        value={formData.description}
        onChange={handleChange}
        required
        error={errors.description}
        disabled={loading || isSubmitting}
        rows={5}
      />

      <div className="d-flex gap-2">
        <Button 
          type="submit" 
          variant="primary" 
          icon="fas fa-save" 
          loading={loading || isSubmitting}
        >
          {submitText || t('videoGroups:buttons.save')}
        </Button>
        {onCancel && (
          <Button 
            type="button" 
            variant="secondary" 
            onClick={() => onCancel()} 
            disabled={loading || isSubmitting}
          >
            {cancelText || t('common:buttons.cancel')}
          </Button>
        )}
      </div>
    </form>
  );
};

VideoGroupForm.propTypes = {
  initialData: PropTypes.object,
  onSubmit: PropTypes.func.isRequired,
  onCancel: PropTypes.func.isRequired,
  loading: PropTypes.bool,
  submitText: PropTypes.string,
  cancelText: PropTypes.string
};

export default VideoGroupForm;
