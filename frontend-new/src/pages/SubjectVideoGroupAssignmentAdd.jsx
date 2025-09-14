import React from 'react';
import { useTranslation } from 'react-i18next';
import SubjectVideoGroupAssignmentForm from '../components/forms/SubjectVideoGroupAssignmentForm.jsx';
import FormPageWrapper from '../components/forms/FormPageWrapper.jsx';
import { LoadingSpinner, ErrorAlert } from '../components/common';
import { useSubjectVideoGroupAssignmentAdd } from '../hooks/useSubjectVideoGroupAssignmentAdd.js';

const SubjectVideoGroupAssignmentAddPage = () => {
    const { t } = useTranslation(['assignments', 'common']);
    const { 
        projectId, 
        subjects, 
        videoGroups, 
        dataLoading, 
        dataError, 
        handleSubmit, 
        handleCancel, 
        loading, 
        error 
    } = useSubjectVideoGroupAssignmentAdd();

    if (!projectId) {
        return (
            <FormPageWrapper title={t('assignments:add_title')} maxWidth={700}>
                <div className="alert alert-danger">
                    <i className="fas fa-exclamation-triangle me-2"></i>
                    Missing projectId parameter in URL
                </div>
            </FormPageWrapper>
        );
    }

    if (dataLoading) {
        return (
            <FormPageWrapper title={t('assignments:add_title')} maxWidth={700}>
                <LoadingSpinner message={t('assignments:loading')} />
            </FormPageWrapper>
        );
    }

    if (dataError) {
        return (
            <FormPageWrapper title={t('assignments:add_title')} maxWidth={700}>
                <ErrorAlert error={dataError} />
            </FormPageWrapper>
        );
    }

    return (
        <FormPageWrapper title={t('assignments:add_title')} maxWidth={700}>
            <SubjectVideoGroupAssignmentForm
                projectId={projectId}
                subjects={subjects}
                videoGroups={videoGroups}
                onSubmit={handleSubmit}
                onCancel={handleCancel}
                loading={loading}
                error={error}
                submitText={t('assignments:buttons.create')}
            />
        </FormPageWrapper>
    );
};

export default SubjectVideoGroupAssignmentAddPage;
