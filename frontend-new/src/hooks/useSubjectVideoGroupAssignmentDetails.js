import { useParams } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { useMultipleDataFetching, useNavigation, useConfirmDialog } from './common';
import SubjectVideoGroupAssignmentService from '../services/SubjectVideoGroupAssignmentService.js';
import SubjectService from '../services/SubjectService.js';
import VideoGroupService from '../services/VideoGroupService.js';
import { useCallback } from 'react';

export const useSubjectVideoGroupAssignmentDetails = () => {
  const { t } = useTranslation(['assignments', 'common']);
  const { id } = useParams();
  const { goTo } = useNavigation();
  const { confirmWithPrompt } = useConfirmDialog();

  let cachedAssignment = null;

  const operations = {
    assignment: async () => {
      cachedAssignment = await SubjectVideoGroupAssignmentService.getById(id);
      return cachedAssignment;
    },
    subject: async () => {
      if (!cachedAssignment) cachedAssignment = await SubjectVideoGroupAssignmentService.getById(id);
      return await SubjectService.getById(cachedAssignment.subjectId);
    },
    videoGroup: async () => {
      if (!cachedAssignment) cachedAssignment = await SubjectVideoGroupAssignmentService.getById(id);
      return await VideoGroupService.getById(cachedAssignment.videoGroupId);
    },
    labelers: async () => {
      return await SubjectVideoGroupAssignmentService.getLabelers(id);
    }
  };

  const { data, loading, error, refetchAll } = useMultipleDataFetching(operations);

  const handleDelete = useCallback(async () => {
    await confirmWithPrompt(t('assignments:confirm.delete'), async () => {
      await SubjectVideoGroupAssignmentService.delete(id);
      if (data.assignment?.projectId) {
        goTo(`/projects/${data.assignment.projectId}`);
      }
    });
  }, [confirmWithPrompt, data.assignment, goTo, id, t]);

  const handleRefresh = useCallback(() => {
    refetchAll();
  }, [refetchAll]);

  const handleBack = useCallback(() => {
    if (data.assignment?.projectId) {
      goTo(`/projects/${data.assignment.projectId}`);
    } else {
      goTo(-1);
    }
  }, [data.assignment, goTo]);

  return {
    assignmentDetails: data.assignment || null,
    subject: data.subject || null,
    videoGroup: data.videoGroup || null,
    labelers: data.labelers || [],
    loading: loading.assignment || loading.subject || loading.videoGroup || loading.labelers,
    error: error.assignment || error.subject || error.videoGroup || error.labelers || '',
    handleDelete,
    handleRefresh,
    handleBack
  };
};
