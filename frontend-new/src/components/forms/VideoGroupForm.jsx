import React, { useState } from 'react';
import PropTypes from 'prop-types';
import { useTranslation } from 'react-i18next';
import { Button, Input } from '../ui';

const VideoGroupForm = ({
  initialData = {},
  onSubmit,
  onCancel,
  loading = false,
  submitText
}) => {
  const { t } = useTranslation(['videoGroups', 'common']);
  const [formData, setFormData] = useState({
    name: initialData.name || '',
    description: initialData.description || '',
    projectId: initialData.projectId || null
  });
  const [errors, setErrors] = useState({});

  const validateForm = () => {
    const newErrors = {};

    if (!formData.name?.trim()) {
      newErrors.name = t('videoGroups:form.validation.name_required');
    }

    if (!formData.description?.trim()) {
      newErrors.description = t('videoGroups:form.validation.description_required');
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));

    // Clear error when user starts typing
    if (errors[name]) {
      setErrors(prev => ({
        ...prev,
        [name]: ''
      }));
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    if (!validateForm()) {
      return;
    }

    try {
      await onSubmit(formData);
    } catch (error) {
      console.error('Form submission error:', error);
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      {/* Video Group Name */}
      <Input
        id="name"
        name="name"
        label={t('videoGroups:form.name')}
        value={formData.name}
        onChange={handleInputChange}
        placeholder={t('videoGroups:form.name_placeholder')}
        error={errors.name}
        disabled={loading}
        required
      />

      {/* Description */}
      <Input
        id="description"
        name="description"
        type="textarea"
        label={t('videoGroups:form.description')}
        value={formData.description}
        onChange={handleInputChange}
        placeholder={t('videoGroups:form.description_placeholder')}
        error={errors.description}
        disabled={loading}
        required
        rows={5}
      />

      {/* Action Buttons */}
      <div className="d-flex gap-2">
        <Button
          type="submit"
          variant="primary"
          icon="fas fa-save"
          loading={loading}
          disabled={loading}
        >
          {loading ? t('videoGroups:buttons.saving') : submitText}
        </Button>
        
        <Button
          type="button"
          variant="outline-secondary"
          icon="fas fa-times"
          onClick={onCancel}
          disabled={loading}
        >
          {t('common:buttons.cancel')}
        </Button>
      </div>
    </form>
  );
};

VideoGroupForm.propTypes = {
  initialData: PropTypes.object,
  onSubmit: PropTypes.func.isRequired,
  onCancel: PropTypes.func.isRequired,
  loading: PropTypes.bool,
  submitText: PropTypes.string
};

export default VideoGroupForm;
