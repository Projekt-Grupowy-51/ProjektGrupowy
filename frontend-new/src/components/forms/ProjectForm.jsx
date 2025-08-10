import React, { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import { useTranslation } from 'react-i18next';
import { Button, Input } from '../ui';

const ProjectForm = ({
  initialData = {},
  onSubmit,
  onCancel,
  loading = false,
  submitText,
  cancelText
}) => {
  const { t } = useTranslation(['projects', 'common']);
  const [formData, setFormData] = useState({
    name: '',
    description: '',
    finished: false,
    ...initialData
  });
  const [errors, setErrors] = useState({});

  useEffect(() => {
    setFormData({
      name: '',
      description: '',
      finished: false,
      ...initialData
    });
  }, [initialData]);

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value
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
      newErrors.name = t('projects:validation.name_required');
    }

    if (!formData.description.trim()) {
      newErrors.description = t('projects:validation.description_required');
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
        label={t('projects:form.name')}
        name="name"
        type="text"
        value={formData.name}
        onChange={handleChange}
        required
        error={errors.name}
        disabled={loading}
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
        disabled={loading}
      />

      <div className="mb-3">
        <div className="form-check">
          <input
            className="form-check-input"
            type="checkbox"
            id="finished"
            name="finished"
            checked={formData.finished}
            onChange={handleChange}
            disabled={loading}
          />
          <label className="form-check-label" htmlFor="finished">
            {t('projects:form.finished')}
          </label>
        </div>
      </div>

      <div className="d-flex gap-2">
        <Button
          type="submit"
          variant="primary"
          loading={loading}
          icon="fas fa-save"
        >
          {submitText || t('projects:buttons.save')}
        </Button>
        
        {onCancel && (
          <Button
            type="button"
            variant="secondary"
            onClick={onCancel}
            disabled={loading}
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
  cancelText: PropTypes.string
};

export default ProjectForm;
