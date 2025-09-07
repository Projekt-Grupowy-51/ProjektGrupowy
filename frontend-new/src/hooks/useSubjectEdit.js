import { useState, useEffect } from 'react';
import { useParams, useLocation, useNavigate } from 'react-router-dom';
import SubjectService from '../services/SubjectService.js';

export const useSubjectEdit = () => {
  const { id } = useParams();
  const location = useLocation();
  const navigate = useNavigate();
  const subjectId = parseInt(id, 10);

  const [subject, setSubject] = useState(null);
  const [loading, setLoading] = useState(false);
  const [submitLoading, setSubmitLoading] = useState(false);
  const [error, setError] = useState(null);

  const fetchSubject = () => {
    setLoading(true);
    setError(null);
    return SubjectService.getById(subjectId)
      .then(setSubject)
      .catch(err => setError(err.message))
      .finally(() => setLoading(false));
  };

  const handleSubmit = (formData) => {
    if (!subject) {
      setError('Subject not loaded');
      return Promise.reject(new Error('Subject not loaded'));
    }
    
    setSubmitLoading(true);
    setError(null);
    const redirectPath = location.state?.from || `/subjects/${subjectId}`;
    
    return SubjectService.update(subjectId, formData)
      .then(() => navigate(redirectPath))
      .catch(err => setError(err.message))
      .finally(() => setSubmitLoading(false));
  };

  const handleCancel = () => window.history.back();

  useEffect(() => {
    fetchSubject();
  }, [subjectId]);

  return {
    id: subjectId,
    subject,
    loading,
    submitLoading,
    error,
    handleSubmit,
    handleCancel
  };
};
