import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import SubjectService from '../services/SubjectService.js';
import LabelService from '../services/LabelService.js';

export const useLabelEdit = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const labelId = parseInt(id, 10);

  const [label, setLabel] = useState(null);
  const [subject, setSubject] = useState(null);
  const [loading, setLoading] = useState(false);
  const [submitLoading, setSubmitLoading] = useState(false);
  const [error, setError] = useState(null);

  const fetchData = async () => {
    if (!labelId) return;
    
    setLoading(true);
    setError(null);
    
    try {
      const labelData = await LabelService.getById(labelId);
      setLabel(labelData);
      
      if (labelData) {
        const subjectData = await SubjectService.getById(labelData.subjectId);
        setSubject(subjectData);
      }
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = (formData) => {
    if (!label) {
      setError('Label not loaded');
      return Promise.reject(new Error('Label not loaded'));
    }
    
    setSubmitLoading(true);
    setError(null);
    return LabelService.update(labelId, formData)
      .then(() => navigate(`/subjects/${label.subjectId}`))
      .catch(err => setError(err.message))
      .finally(() => setSubmitLoading(false));
  };

  const handleCancel = () => window.history.back();

  useEffect(() => {
    fetchData();
  }, [labelId]);

  return {
    id: labelId,
    label,
    subject,
    loading,
    submitLoading,
    error,
    handleSubmit,
    handleCancel
  };
};
