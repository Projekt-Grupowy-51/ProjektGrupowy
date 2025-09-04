import { useSearchParams } from 'react-router-dom';
import { useCallback } from 'react';
import { useDataFetching, useFormSubmission } from './common';
import SubjectService from '../services/SubjectService.js';
import LabelService from '../services/LabelService.js';

export const useLabelAdd = () => {
  const [searchParams] = useSearchParams();
  const subjectId = parseInt(searchParams.get('subjectId'), 10);

  // Funkcja fetchująca opakowana w useCallback
  const fetchSubject = useCallback(() => {
    if (!subjectId) return Promise.resolve(null);
    return SubjectService.getById(subjectId);
  }, [subjectId]);

  const { 
    data: subject, 
    loading: subjectLoading, 
    error: subjectError 
  } = useDataFetching(fetchSubject, [subjectId]);

  const submitOperation = useCallback(async (formData) => {
    return await LabelService.create({ ...formData, subjectId });
  }, [subjectId]);

  const redirectPath = `/subjects/${subjectId}`;

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

  const error = subjectError || submitError || 
    (!subjectLoading && !subject ? 'Subject not found' : null);

  return {
    subject,
    subjectId,
    subjectLoading,
    submitLoading,
    error,
    handleSubmit,
    handleCancel
  };
};
