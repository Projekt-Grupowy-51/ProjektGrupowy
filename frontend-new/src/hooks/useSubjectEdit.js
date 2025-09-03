import { useParams } from 'react-router-dom';
import { useCallback } from 'react';
import { useDataFetching, useFormSubmission } from './common';
import SubjectService from '../services/SubjectService.js';

export const useSubjectEdit = () => {
  const { id } = useParams();
  const subjectId = parseInt(id, 10);

  const fetchSubject = useCallback(() => SubjectService.getById(subjectId), [subjectId]);

  const { data: subject, loading: subjectLoading, error: subjectError } =
    useDataFetching(fetchSubject, [subjectId]);

  const submitOperation = useCallback(async (formData) => {
    if (!subject) throw new Error('Subject not loaded');
    return await SubjectService.update(subjectId, formData);
  }, [subject, subjectId]);

  const redirectPath = `/subjects/${subjectId}`;

  const {
    handleSubmit,
    handleCancel,
    loading: submitLoading,
    error: submitError
  } = useFormSubmission(submitOperation, redirectPath, redirectPath);

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
