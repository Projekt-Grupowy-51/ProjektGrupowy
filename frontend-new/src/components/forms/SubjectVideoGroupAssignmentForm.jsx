import React, { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import { useTranslation } from 'react-i18next';
import { Button, Select } from '../ui';

// Example fake data
const FAKE_SUBJECTS = [
    { id: 1, name: 'Math' },
    { id: 2, name: 'Physics' },
];
const FAKE_VIDEOGROUPS = [
    { id: 1, name: 'Intro Videos' },
    { id: 2, name: 'Advanced Videos' },
];

const SubjectVideoGroupAssignmentForm = ({
                                             projectId,
                                             initialData = {},
                                             onSubmit,
                                             onCancel,
                                             loading = false,
                                             submitText,
                                             cancelText,
                                         }) => {
    const { t } = useTranslation(['assignments', 'common']);
    const [formData, setFormData] = useState({
        subjectId: '',
        videoGroupId: '',
        ...initialData,
    });
    const [subjects, setSubjects] = useState([]);
    const [videoGroups, setVideoGroups] = useState([]);
    const [errors, setErrors] = useState({});

    useEffect(() => {
        // Replace API call with fake data
        setSubjects(FAKE_SUBJECTS);
        setVideoGroups(FAKE_VIDEOGROUPS);
    }, [projectId]);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData((prev) => ({ ...prev, [name]: value }));
        if (errors[name]) setErrors((prev) => ({ ...prev, [name]: '' }));
    };

    const validateForm = () => {
        const newErrors = {};
        if (!formData.subjectId) newErrors.subjectId = t('assignments:validation.subject_required');
        if (!formData.videoGroupId) newErrors.videoGroupId = t('assignments:validation.video_group_required');
        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        if (!validateForm()) return;
        onSubmit({
            subjectId: parseInt(formData.subjectId),
            videoGroupId: parseInt(formData.videoGroupId),
        });
    };

    return (
        <form onSubmit={handleSubmit}>
            <Select
                label={t('assignments:form.subject')}
                name="subjectId"
                value={formData.subjectId}
                onChange={handleChange}
                options={subjects.map((s) => ({ value: s.id, label: s.name }))}
                required
                error={errors.subjectId}
            />

            <Select
                label={t('assignments:form.video_group')}
                name="videoGroupId"
                value={formData.videoGroupId}
                onChange={handleChange}
                options={videoGroups.map((vg) => ({ value: vg.id, label: vg.name }))}
                required
                error={errors.videoGroupId}
            />

            <div className="d-flex gap-2">
                <Button type="submit" variant="primary" loading={loading} icon="fas fa-save">
                    {submitText || t('assignments:buttons.create')}
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

SubjectVideoGroupAssignmentForm.propTypes = {
    projectId: PropTypes.number.isRequired,
    initialData: PropTypes.shape({
        subjectId: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
        videoGroupId: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
    }),
    onSubmit: PropTypes.func.isRequired,
    onCancel: PropTypes.func,
    loading: PropTypes.bool,
    submitText: PropTypes.string,
    cancelText: PropTypes.string,
};

export default SubjectVideoGroupAssignmentForm;
