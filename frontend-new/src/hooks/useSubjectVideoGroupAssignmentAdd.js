import { useParams } from 'react-router-dom';
import { useMultipleDataFetching, useAsyncOperation, useFormSubmission } from './common';
import SubjectVideoGroupAssignmentService from '../services/SubjectVideoGroupAssignmentService.js';
import ProjectService from '../services/ProjectService.js';
import { useCallback } from 'react';

export const useSubjectVideoGroupAssignmentAdd = () => {
  const { projectId: projectIdParam } = useParams();
  const projectId = projectIdParam ? parseInt(projectIdParam, 10) : null;

  const fetchSubjects = useCallback(
    () => (projectId ? ProjectService.getSubjects(projectId) : Promise.resolve([])),
    [projectId]
  );

  const fetchVideoGroups = useCallback(
    () => (projectId ? ProjectService.getVideoGroups(projectId) : Promise.resolve([])),
    [projectId]
  );

  const operations = {
    subjects: fetchSubjects,
    videoGroups: fetchVideoGroups
  };

  const { data, loading, error } = useMultipleDataFetching(operations);

  const { execute: createAssignment, loading: submitLoading, error: submitError } = useAsyncOperation();

  const submitOperation = useCallback(
    async (assignmentData) => {
      if (!projectId) throw new Error('Project ID not found');

      return await createAssignment(() =>
        SubjectVideoGroupAssignmentService.create({ ...assignmentData })
      );
    },
    [projectId, createAssignment]
  );

  const successPath = `/projects/${projectId}`;
  const cancelPath = `/projects/${projectId}`;

  const { handleSubmit, handleCancel } = useFormSubmission(submitOperation, successPath, cancelPath);

  return {
    projectId,
    subjects: data.subjects || [],
    videoGroups: data.videoGroups || [],
    loading: loading.subjects || loading.videoGroups,
    error: error.subjects || error.videoGroups,
    submitLoading,
    submitError,
    handleSubmit,
    handleCancel
  };
};
