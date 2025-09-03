import { useParams } from 'react-router-dom';
import { useCallback } from 'react';
import { useMultipleDataFetching, useNavigation, useConfirmDialog } from './common';
import SubjectService from '../services/SubjectService.js';
import LabelService from '../services/LabelService.js';

export const useSubjectDetails = () => {
  const { id } = useParams();
  const { goTo } = useNavigation();
  const { confirmWithPrompt } = useConfirmDialog();

  const operations = {
    subject: useCallback(() => SubjectService.getById(id), [id]),
    labels: useCallback(() => SubjectService.getLabels(id), [id])
  };

  const { data, loading, error, fetchData } = useMultipleDataFetching(operations);

  const deleteLabel = useCallback(
    async (labelId, confirmMessage) => {
      await confirmWithPrompt(confirmMessage, async () => {
        await LabelService.delete(labelId);
        await fetchData('labels', operations.labels);
      });
    },
    [confirmWithPrompt, fetchData, operations.labels]
  );

  const handleBackToProject = useCallback(() => {
    if (data.subject?.projectId) {
      goTo(`/projects/${data.subject.projectId}`);
    }
  }, [data.subject, goTo]);

  const handleAddLabel = useCallback(() => {
    goTo(`/labels/add?subjectId=${id}`);
  }, [id, goTo]);

  const handleEditSubject = useCallback(() => {
    goTo(`/subjects/${id}/edit`);
  }, [id, goTo]);

  const handleEditLabel = useCallback(
    (labelId) => {
      goTo(`/labels/${labelId}/edit`);
    },
    [goTo]
  );

  return {
    id,
    subject: data.subject || null,
    labels: data.labels || [],
    subjectLoading: loading.subject,
    labelsLoading: loading.labels,
    subjectError: error.subject,
    labelsError: error.labels,
    deleteLabel,
    handleBackToProject,
    handleAddLabel,
    handleEditSubject,
    handleEditLabel
  };
};
