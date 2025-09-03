import { useCallback } from 'react';
import { useParams } from 'react-router-dom';
import { useDataFetching, useFormSubmission } from './common';
import SubjectService from '../services/SubjectService.js';
import LabelService from '../services/LabelService.js';

export const useLabelEdit = () => {
  const { id } = useParams();
  const labelId = parseInt(id, 10);

  // Fetch label
  const fetchLabel = useCallback(() => {
    if (!labelId) return Promise.resolve(null);
    return LabelService.getById(labelId);
  }, [labelId]);

  const { data: label, loading: labelLoading, error: labelError } = 
    useDataFetching(fetchLabel, [labelId]);

  // Fetch subject after label is loaded
  const fetchSubject = useCallback(() => {
    if (!label) return Promise.resolve(null);
    return SubjectService.getById(label.subjectId);
  }, [label]);

  const { data: subject, loading: subjectLoading, error: subjectError } =
    useDataFetching(fetchSubject, [label]);

  // Submit operation
  const submitOperation = useCallback(async (formData) => {
    if (!label) throw new Error('Label not loaded');
    return await LabelService.update(labelId, formData);
  }, [label, labelId]);

  const redirectPath = `/subjects/${label?.subjectId}`;

  const { handleSubmit, handleCancel, loading: submitLoading, error: submitError } =
    useFormSubmission(submitOperation, redirectPath, redirectPath);

  return {
    id: labelId,
    label,
    subject,
    loading: labelLoading || subjectLoading,
    submitLoading,
    error: labelError || subjectError || submitError,
    handleSubmit,
    handleCancel
  };
};
