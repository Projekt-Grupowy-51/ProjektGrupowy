import React, { useState } from 'react';
import PropTypes from 'prop-types';
import { useTranslation } from 'react-i18next';
import { Input, Button } from '../ui';

const SubjectForm = ({ 
  initialData = {}, 
  onSubmit, 
  onCancel, 
  loading = false, 
  submitText = 'Save',
  projectId = null
}) => {
  const { t } = useTranslation(['subjects', 'common']);
  const [formData, setFormData] = useState({
    name: initialData.name || '',
    description: initialData.description || '',
    projectId: initialData.projectId || projectId || ''
  });

  const [errors, setErrors] = useState({});

  const handleChange = (e) => {
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

  const validateForm = () => {
    const newErrors = {};
    
    if (!formData.name.trim()) {
      newErrors.name = t('subjects:notifications.required_fields');
    }
    
    if (!formData.description.trim()) {
      newErrors.description = t('subjects:notifications.required_fields');
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
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
      <Input
        label={t('subjects:form.name')}
        name="name"
        value={formData.name}
        onChange={handleChange}
        error={errors.name}
        required
        disabled={loading}
      />

      <Input
        label={t('subjects:form.description')}
        name="description"
        type="textarea"
        value={formData.description}
        onChange={handleChange}
        error={errors.description}
        required
        disabled={loading}
        rows={4}
      />

      <div className="d-flex gap-2">
        <Button
          type="submit"
          variant="primary"
          icon="fas fa-save"
          loading={loading}
          disabled={loading}
        >
          {loading ? t('subjects:buttons.adding') : submitText}
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

SubjectForm.propTypes = {
  initialData: PropTypes.object,
  onSubmit: PropTypes.func.isRequired,
  onCancel: PropTypes.func.isRequired,
  loading: PropTypes.bool,
  submitText: PropTypes.string,
  projectId: PropTypes.oneOfType([PropTypes.string, PropTypes.number])
};

export default SubjectForm;
