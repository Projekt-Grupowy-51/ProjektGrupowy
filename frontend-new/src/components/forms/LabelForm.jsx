import React from 'react';
import PropTypes from 'prop-types';
import { useTranslation } from 'react-i18next';
import { Input, Button } from '../ui';
import { useBaseForm } from '../../hooks/forms/useBaseForm.js';
import { ValidationRules, required, hexColor, singleCharacter } from '../../utils/formValidation.js';

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
  
  // Initialize form data
  const defaultData = {
    name: initialData?.name || '',
    colorHex: initialData?.colorHex || '#3498db',
    type: initialData?.type || 'range',
    shortcut: initialData?.shortcut || '',
    subjectId: initialData?.subjectId || subjectId || null
  };

  // Setup validation rules
  const validationRules = {
    name: ValidationRules.name(t),
    colorHex: ValidationRules.colorHex(t),
    shortcut: [singleCharacter(t('labels:validation.shortcut_invalid'))],
    subjectId: [required(t('labels:validation.subject_required'))]
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
        label={t('labels:form.name')}
        name="name"
        type="text"
        value={formData.name}
        onChange={handleChange}
        required
        error={errors.name}
        disabled={loading || isSubmitting}
      />

      <Input
        label={t('labels:form.color')}
        name="colorHex"
        type="color"
        value={formData.colorHex}
        onChange={handleChange}
        required
        error={errors.colorHex}
        disabled={loading || isSubmitting}
        helpText={t('labels:form.color_format')}
      />

      <Input
        label={t('labels:form.type')}
        name="type"
        type="select"
        value={formData.type}
        onChange={handleChange}
        required
        disabled={loading || isSubmitting}
        options={[
          { value: 'range', label: t('labels:form.type_options.range') },
          { value: 'point', label: t('labels:form.type_options.point') }
        ]}
      />

      <Input
        label={t('labels:form.shortcut')}
        name="shortcut"
        type="text"
        value={formData.shortcut}
        onChange={handleChange}
        maxLength={1}
        placeholder={t('labels:form.shortcut_hint')}
        error={errors.shortcut}
        disabled={loading || isSubmitting}
        helpText={t('labels:form.shortcut_hint')}
      />

      <Input
        label={t('labels:form.subject')}
        name="subjectDisplay"
        type="text"
        value={subjectName || `${t('common:subject.id')}: ${formData.subjectId || ''}`}
        onChange={() => {}}
        disabled={true}
        error={errors.subjectId}
      />

      <div className="d-flex gap-2">
        <Button
          type="submit"
          variant="primary"
          loading={loading || isSubmitting}
          icon="fas fa-save"
        >
          {submitText || t('labels:buttons.save')}
        </Button>
        
        {onCancel && (
          <Button
            type="button"
            variant="secondary"
            onClick={onCancel}
            disabled={loading || isSubmitting}
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
