import React from 'react';
import PropTypes from 'prop-types';
import { useTranslation } from 'react-i18next';
import { Button, Input, Alert } from '../ui';
import { ActionButtons } from '../common';
import { useBaseForm } from '../../hooks/forms/useBaseForm.js';
import { ValidationRules } from '../../utils/formValidation.js';

const SubjectVideoGroupAssignmentForm = ({
                                             projectId,
                                             subjects = [],
                                             videoGroups = [],
                                             initialData = {},
                                             onSubmit,
                                             onCancel,
                                             loading = false,
                                             error,
                                             submitText,
                                             cancelText,
                                         }) => {
    const { t } = useTranslation(['assignments', 'common']);
    
    // Initialize form data
    const defaultData = {
        subjectId: initialData?.subjectId || '',
        videoGroupId: initialData?.videoGroupId || '',
    };

    // Setup validation rules
    const validationRules = {
        subjectId: ValidationRules.requiredSelect('subject', t),
        videoGroupId: ValidationRules.requiredSelect('video_group', t)
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
            {error && <Alert variant="danger">{error}</Alert>}
            
            <Input
                label={t('assignments:form.subject')}
                name="subjectId"
                type="select"
                value={formData.subjectId}
                onChange={handleChange}
                options={subjects.map((s) => ({ value: s.id, label: s.name }))}
                required
                error={errors.subjectId}
                disabled={loading || isSubmitting}
            />

            <Input
                label={t('assignments:form.video_group')}
                name="videoGroupId"
                type="select"
                value={formData.videoGroupId}
                onChange={handleChange}
                options={videoGroups.map((vg) => ({ value: vg.id, label: vg.name }))}
                required
                error={errors.videoGroupId}
                disabled={loading || isSubmitting}
            />

            <ActionButtons>
                <Button 
                    type="submit" 
                    variant="primary" 
                    loading={loading || isSubmitting} 
                    icon="fas fa-save"
                >
                    {submitText || t('assignments:buttons.create')}
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
            </ActionButtons>
        </form>
    );
};

SubjectVideoGroupAssignmentForm.propTypes = {
    projectId: PropTypes.number.isRequired,
    subjects: PropTypes.arrayOf(PropTypes.shape({
        id: PropTypes.number.isRequired,
        name: PropTypes.string.isRequired
    })),
    videoGroups: PropTypes.arrayOf(PropTypes.shape({
        id: PropTypes.number.isRequired,
        name: PropTypes.string.isRequired
    })),
    initialData: PropTypes.shape({
        subjectId: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
        videoGroupId: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
    }),
    onSubmit: PropTypes.func.isRequired,
    onCancel: PropTypes.func,
    loading: PropTypes.bool,
    error: PropTypes.string,
    submitText: PropTypes.string,
    cancelText: PropTypes.string,
};

export default SubjectVideoGroupAssignmentForm;
