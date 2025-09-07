import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useConfirmDialog } from './common/useConfirmDialog.js';
import SubjectService from '../services/SubjectService.js';
import LabelService from '../services/LabelService.js';

export const useSubjectDetails = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { confirmWithPrompt } = useConfirmDialog();
  
  const [subject, setSubject] = useState(null);
  const [labels, setLabels] = useState([]);
  const [subjectLoading, setSubjectLoading] = useState(false);
  const [labelsLoading, setLabelsLoading] = useState(false);
  const [subjectError, setSubjectError] = useState(null);
  const [labelsError, setLabelsError] = useState(null);

  const fetchSubject = () => {
    setSubjectLoading(true);
    setSubjectError(null);
    return SubjectService.getById(id)
      .then(setSubject)
      .catch(err => setSubjectError(err.message))
      .finally(() => setSubjectLoading(false));
  };

  const fetchLabels = () => {
    setLabelsLoading(true);
    setLabelsError(null);
    return SubjectService.getLabels(id)
      .then(setLabels)
      .catch(err => setLabelsError(err.message))
      .finally(() => setLabelsLoading(false));
  };

  const deleteLabel = async (labelId, confirmMessage) => {
    await confirmWithPrompt(confirmMessage, async () => {
      await LabelService.delete(labelId);
      await fetchLabels();
    });
  };

  useEffect(() => {
    fetchSubject();
    fetchLabels();
  }, [id]);

  const handleBackToProject = () => {
    if (subject?.projectId) {
      navigate(`/projects/${subject.projectId}`);
    }
  };

  const handleAddLabel = () => navigate(`/labels/add?subjectId=${id}`);
  const handleEditSubject = () => navigate(`/subjects/${id}/edit`);
  const handleEditLabel = (labelId) => navigate(`/labels/${labelId}/edit`);

  return {
    id,
    subject,
    labels,
    subjectLoading,
    labelsLoading,
    subjectError,
    labelsError,
    deleteLabel,
    handleBackToProject,
    handleAddLabel,
    handleEditSubject,
    handleEditLabel
  };
};
