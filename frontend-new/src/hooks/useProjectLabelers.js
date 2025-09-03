import { useState, useCallback, useMemo } from 'react';
import { useParams } from 'react-router-dom';
import { useDataFetching, useAsyncOperation } from './common';
import ProjectService from '../services/ProjectService.js';
import SubjectVideoGroupAssignmentService from '../services/SubjectVideoGroupAssignmentService.js';

export const useProjectLabelers = () => {
  const { projectId: paramId } = useParams();
  const projectId = parseInt(paramId, 10);

  const [selectedLabeler, setSelectedLabeler] = useState('');
  const [selectedAssignment, setSelectedAssignment] = useState('');

  const fetchLabelerData = useCallback(async () => {
    if (!projectId) return { labelers: [], assignments: [] };

    const [labelers, assignments] = await Promise.all([
      ProjectService.getUsers(projectId),
      ProjectService.getAssignments(projectId)
    ]);

    return {
      labelers: labelers.filter(user => user.role === 'labeler'),
      assignments
    };
  }, [projectId]);

  const { data, loading, error, refetch } = useDataFetching(
    projectId ? fetchLabelerData : null,
    [projectId]
  );

  const { execute: assignLabelerOp, loading: assignLoading } = useAsyncOperation();
  const { execute: unassignLabelerOp, loading: unassignLoading } = useAsyncOperation();

  const assignLabeler = useCallback(async (labelerId, assignmentId) => {
    if (!labelerId || !assignmentId) return;
    await assignLabelerOp(() => SubjectVideoGroupAssignmentService.addLabeler(assignmentId, labelerId));
    setSelectedLabeler('');
    setSelectedAssignment('');
    refetch();
  }, [assignLabelerOp, refetch]);

  const unassignLabeler = useCallback(async (assignmentId, labelerId) => {
    if (!assignmentId || !labelerId) return;
    await unassignLabelerOp(() => SubjectVideoGroupAssignmentService.removeLabeler(assignmentId, labelerId));
    refetch();
  }, [unassignLabelerOp, refetch]);

  const labelers = data?.labelers || [];
  const assignments = data?.assignments || [];

  const { unassignedLabelers, assignedLabelerRows } = useMemo(() => {
    const assignedLabelerIds = new Set(assignments.flatMap(a => a.labelers?.map(l => l.id) || []));
    const unassigned = labelers.filter(l => !assignedLabelerIds.has(l.id));

    const rows = assignments.flatMap(assignment =>
      (assignment.labelers || []).map(labeler => ({
        labelerId: labeler.id,
        labelerName: labeler.name,
        videoGroupName: assignment.videoGroupName || 'Unknown',
        subjectName: assignment.subjectName || 'Unknown',
        assignmentId: assignment.id
      }))
    );

    return { unassignedLabelers: unassigned, assignedLabelerRows: rows };
  }, [labelers, assignments]);

  return {
    projectId,
    labelers,
    assignments,
    unassignedLabelers,
    assignedLabelerRows,
    selectedLabeler,
    setSelectedLabeler,
    selectedAssignment,
    setSelectedAssignment,
    assignLabeler,
    unassignLabeler,
    loading,
    assignLoading,
    unassignLoading,
    error,
    refetch
  };
};
