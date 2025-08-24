import React, { useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import SubjectVideoGroupAssignmentForm from '../components/forms/SubjectVideoGroupAssignmentForm.jsx';
import FormPageWrapper from '../components/forms/FormPageWrapper.jsx';
import { FAKE_ASSIGNMENTS, addToCollection } from '../data/fakeData.js';

const SubjectVideoGroupAssignmentAddPage = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const { t } = useTranslation(['assignments', 'common']);
    const [loading, setLoading] = useState(false);
    
    const queryParams = new URLSearchParams(location.search);
    const projectId = parseInt(queryParams.get('projectId'), 10);

    const createAssignment = async (assignmentData) => {
        setLoading(true);
        await new Promise(resolve => setTimeout(resolve, 1000)); // simulate API
        addToCollection(FAKE_ASSIGNMENTS, {
            ...assignmentData,
            id: Date.now(),
            createdAt: new Date().toISOString(),
        });
        setLoading(false);
    };

    const handleSubmit = async (assignmentData) => {
        try {
            await createAssignment(assignmentData);
            navigate(`/projects/${projectId}`);
        } catch (error) {
            console.error('Failed to create assignment:', error);
        }
    };

    const handleCancel = () => navigate(`/projects/${projectId}`);

    return (
        <FormPageWrapper title={t('assignments:add_title')} maxWidth={700}>
            <SubjectVideoGroupAssignmentForm
                projectId={projectId}
                onSubmit={handleSubmit}
                onCancel={handleCancel}
                loading={loading}
                submitText={t('assignments:buttons.create')}
            />
        </FormPageWrapper>
    );
};

export default SubjectVideoGroupAssignmentAddPage;
