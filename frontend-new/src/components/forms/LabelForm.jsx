import React, { useState } from 'react';
import PropTypes from 'prop-types';
import { useTranslation } from 'react-i18next';
import { Input, Button } from '../ui';

const LabelForm = ({ 
  initialData = {}, 
  onSubmit, 
  onCancel, 
  loading = false, 
  submitText,
  subjectId = null,
  subjectName = null
}) => {
  const { t } = useTranslation(['labels', 'common']);
  const [formData, setFormData] = useState({
    name: initialData.name || '',
    colorHex: initialData.colorHex || '#3498db',
    type: initialData.type || 'range',
    shortcut: initialData.shortcut || '',
    subjectId: initialData.subjectId || subjectId || null
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

  const handleColorChange = (value) => {
    setFormData(prev => ({
      ...prev,
      colorHex: value
    }));
    
    // Clear error when user changes color
    if (errors.colorHex) {
      setErrors(prev => ({
        ...prev,
        colorHex: ''
      }));
    }
  };

  const validateForm = () => {
    const newErrors = {};
    
    if (!formData.name.trim()) {
      newErrors.name = t('labels:validation.name_required');
    }
    
    if (formData.shortcut && formData.shortcut.length !== 1) {
      newErrors.shortcut = t('labels:validation.shortcut_invalid');
    }
    
    if (!/^#[0-9A-Fa-f]{6}$/.test(formData.colorHex)) {
      newErrors.colorHex = t('labels:validation.color_invalid');
    }

    if (!formData.subjectId) {
      newErrors.subjectId = t('labels:validation.subject_required');
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
        label={t('labels:form.name')}
        name="name"
        type="text"
        value={formData.name}
        onChange={handleChange}
        required
        error={errors.name}
        disabled={loading}
      />

      <div className="mb-3">
        <label htmlFor="colorHex" className="form-label">
          {t('labels:form.color')}
        </label>
        <div className="input-group">
          <input
            type="color"
            id="colorHex"
            name="colorHex"
            value={formData.colorHex}
            onChange={handleChange}
            className="form-control form-control-color"
            title={t('labels:form.color')}
            style={{ maxWidth: "60px" }}
            required
            disabled={loading}
          />
          <input
            type="text"
            className={`form-control ${errors.colorHex ? 'is-invalid' : ''}`}
            value={formData.colorHex}
            onChange={(e) => handleColorChange(e.target.value)}
            pattern="#[0-9A-Fa-f]{6}"
            placeholder="#RRGGBB"
            required
            disabled={loading}
          />
          <span
            className="input-group-text"
            style={{
              backgroundColor: formData.colorHex,
              width: "40px",
            }}
          ></span>
        </div>
        {errors.colorHex && (
          <div className="invalid-feedback d-block">
            {errors.colorHex}
          </div>
        )}
        <div className="form-text">
          {t('labels:form.color_format')}
        </div>
      </div>

      <div className="mb-3">
        <label htmlFor="type" className="form-label">
          {t('labels:form.type')}
        </label>
        <select
          id="type"
          name="type"
          value={formData.type}
          onChange={handleChange}
          className="form-select"
          required
          disabled={loading}
        >
          <option value="range">{t('labels:form.type_options.range')}</option>
          <option value="point">{t('labels:form.type_options.point')}</option>
        </select>
      </div>

      <Input
        label={t('labels:form.shortcut')}
        name="shortcut"
        type="text"
        value={formData.shortcut}
        onChange={handleChange}
        maxLength={1}
        placeholder={t('labels:form.shortcut_hint')}
        error={errors.shortcut}
        disabled={loading}
        helpText={t('labels:form.shortcut_hint')}
      />

      <div className="mb-4">
        <label htmlFor="subjectId" className="form-label">
          {t('labels:form.subject')}
        </label>
        <div className="input-group">
          <span className="input-group-text">
            <i className="fas fa-folder"></i>
          </span>
          <input
            type="text"
            className="form-control"
            value={subjectName || `${t('common:subject.id')}: ${formData.subjectId || ''}`}
            disabled
          />
        </div>
        {errors.subjectId && (
          <div className="invalid-feedback d-block">
            {errors.subjectId}
          </div>
        )}
      </div>

      <div className="d-flex gap-2">
        <Button
          type="submit"
          variant="primary"
          loading={loading}
          icon="fas fa-save"
        >
          {submitText || t('labels:buttons.save')}
        </Button>
        
        {onCancel && (
          <Button
            type="button"
            variant="secondary"
            onClick={onCancel}
            disabled={loading}
          >
            {t('common:buttons.cancel')}
          </Button>
        )}
      </div>
    </form>
  );
};

LabelForm.propTypes = {
  initialData: PropTypes.shape({
    name: PropTypes.string,
    colorHex: PropTypes.string,
    type: PropTypes.string,
    shortcut: PropTypes.string,
    subjectId: PropTypes.number
  }),
  onSubmit: PropTypes.func.isRequired,
  onCancel: PropTypes.func,
  loading: PropTypes.bool,
  submitText: PropTypes.string,
  subjectId: PropTypes.number,
  subjectName: PropTypes.string
};

export default LabelForm;
