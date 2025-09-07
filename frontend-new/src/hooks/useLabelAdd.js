import { useState, useEffect } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import SubjectService from '../services/SubjectService.js';
import LabelService from '../services/LabelService.js';

export const useLabelAdd = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const subjectId = parseInt(searchParams.get('subjectId'), 10);

  const [subject, setSubject] = useState(null);
  const [subjectLoading, setSubjectLoading] = useState(false);
  const [submitLoading, setSubmitLoading] = useState(false);
  const [error, setError] = useState(null);

  const fetchSubject = () => {
    if (!subjectId) return Promise.resolve();
    
    setSubjectLoading(true);
    setError(null);
    return SubjectService.getById(subjectId)
      .then(setSubject)
      .catch(err => setError(err.message))
      .finally(() => setSubjectLoading(false));
  };

  const handleSubmit = (formData) => {
    setSubmitLoading(true);
    setError(null);
    return LabelService.create({ ...formData, subjectId })
      .then(() => navigate(`/subjects/${subjectId}`))
      .catch(err => setError(err.message))
      .finally(() => setSubmitLoading(false));
  };

  const handleCancel = () => window.history.back();

  useEffect(() => {
    fetchSubject();
  }, [subjectId]);

  useEffect(() => {
    // Clear error when subject is found
    if (subject) {
      setError(null);
    }
  }, [subject]);

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
