import React, { useState } from 'react';
import PropTypes from 'prop-types';
import { useTranslation } from 'react-i18next';
import { Button, Input } from '../ui';

const VideoGroupForm = ({
                          initialData = {},
                          onSubmit,
                          onCancel,
                          loading = false,
                          submitText,
                          cancelText
                        }) => {
  const { t } = useTranslation(['videoGroups', 'common']);
  const [formData, setFormData] = useState({
    name: initialData.name || '',
    description: initialData.description || '',
    projectId: initialData.projectId || null
  });
  const [errors, setErrors] = useState({});

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));

    if (errors[name]) setErrors(prev => ({ ...prev, [name]: '' }));
  };

  const validateForm = () => {
    const newErrors = {};
    if (!formData.name.trim()) newErrors.name = t('videoGroups:form.validation.name_required');
    if (!formData.description.trim()) newErrors.description = t('videoGroups:form.validation.description_required');
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validateForm()) return;
    try {
      await onSubmit(formData);
    } catch (error) {
      console.error('Form submission error:', error);
    }
  };

  return (
      <form onSubmit={handleSubmit}>
        <Input
            label={t('videoGroups:form.name')}
            name="name"
            value={formData.name}
            onChange={handleChange}
            required
            error={errors.name}
            disabled={loading}
        />

        <Input
            label={t('videoGroups:form.description')}
            name="description"
            type="textarea"
            value={formData.description}
            onChange={handleChange}
            required
            error={errors.description}
            disabled={loading}
            rows={5}
        />

        <div className="d-flex gap-2">
          <Button type="submit" variant="primary" icon="fas fa-save" loading={loading}>
            {submitText || t('videoGroups:buttons.save')}
          </Button>
          {onCancel && (
              <Button type="button" variant="secondary" onClick={onCancel} disabled={loading}>
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
