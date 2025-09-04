import { useParams, useLocation } from 'react-router-dom';
import { useCallback } from 'react';
import { useDataFetching, useFormSubmission } from './common';
import SubjectService from '../services/SubjectService.js';

export const useSubjectEdit = () => {
  const { id } = useParams();
  const location = useLocation();
  const subjectId = parseInt(id, 10);

  const fetchSubject = useCallback(() => SubjectService.getById(subjectId), [subjectId]);

  const { data: subject, loading: subjectLoading, error: subjectError } =
    useDataFetching(fetchSubject, [subjectId]);

  const submitOperation = useCallback(async (formData) => {
    if (!subject) throw new Error('Subject not loaded');
    return await SubjectService.update(subjectId, formData);
  }, [subject, subjectId]);

  const redirectPath = location.state?.from || `/subjects/${subjectId}`;

  const {
    handleSubmit,
    handleCancel: defaultHandleCancel,
    loading: submitLoading,
    error: submitError
  } = useFormSubmission(submitOperation, redirectPath, redirectPath);

  // Override cancel to use browser back
  const handleCancel = useCallback(() => {
    window.history.back();
  }, []);

  return {
    id: subjectId,
    subject,
    loading: subjectLoading,
    submitLoading,
    error: subjectError || submitError,
    handleSubmit,
    handleCancel
  };
};
