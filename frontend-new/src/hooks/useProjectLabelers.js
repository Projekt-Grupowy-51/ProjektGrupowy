import { useState, useCallback, useMemo } from 'react';
import { useParams } from 'react-router-dom';
import { useDataFetching, useAsyncOperation } from './common';
import ProjectService from '../services/ProjectService.js';
import SubjectVideoGroupAssignmentService from '../services/SubjectVideoGroupAssignmentService.js';

export const useProjectLabelers = (passedProjectId) => {
  const { projectId: paramId } = useParams();
  const projectId = passedProjectId ? parseInt(passedProjectId, 10) : parseInt(paramId, 10);

  const [selectedLabeler, setSelectedLabeler] = useState('');
  const [selectedAssignment, setSelectedAssignment] = useState('');
  const [selectedCustomAssignments, setSelectedCustomAssignments] = useState({});

  const fetchLabelerData = useCallback(async () => {
    if (!projectId) return { labelers: [], allLabelers: [], unassignedLabelers: [], assignments: [] };

    const [allLabelers, unassignedLabelers, assignments] = await Promise.all([
      ProjectService.getUsers(projectId),
      ProjectService.getUnassignedLabelers(projectId),
      ProjectService.getAssignments(projectId)
    ]);

    return {
      labelers: allLabelers,
      allLabelers: allLabelers,
      unassignedLabelers,
      assignments
    };
  }, [projectId]);

  const { data, loading, error, refetch } = useDataFetching(
    projectId ? fetchLabelerData : null,
    [projectId]
  );

  const { execute: assignLabelerOp, loading: assignLoading } = useAsyncOperation();
  const { execute: unassignLabelerOp, loading: unassignLoading } = useAsyncOperation();
  const { execute: distributeLabelerOp, loading: distributeLoading } = useAsyncOperation();
  const { execute: unassignAllOp, loading: unassignAllLoading } = useAsyncOperation();
  const { execute: assignAllSelectedOp, loading: assignAllSelectedLoading } = useAsyncOperation();

  const assignLabeler = useCallback(async (labelerId, assignmentId) => {
    if (!labelerId || !assignmentId) return;
    try {
      await assignLabelerOp(() => SubjectVideoGroupAssignmentService.addLabeler(assignmentId, labelerId));
      setSelectedLabeler('');
      setSelectedAssignment('');
    } finally {
      refetch();
    }
  }, [assignLabelerOp, refetch]);

  const unassignLabeler = useCallback(async (assignmentId, labelerId) => {
    if (!assignmentId || !labelerId) return;
    await unassignLabelerOp(() => SubjectVideoGroupAssignmentService.removeLabeler(assignmentId, labelerId));
    refetch();
  }, [unassignLabelerOp, refetch]);

  const distributeLabelers = useCallback(async () => {
    await distributeLabelerOp(() => ProjectService.distributeLabelers(projectId));
    refetch();
  }, [distributeLabelerOp, projectId, refetch]);

  const unassignAllLabelers = useCallback(async () => {
    await unassignAllOp(() => ProjectService.unassignAllLabelers(projectId));
    refetch();
  }, [unassignAllOp, projectId, refetch]);

  const handleCustomLabelerAssignmentChange = useCallback((labelerId, assignmentId) => {
    setSelectedCustomAssignments(prev => {
      const updated = { ...prev };
      
      if (!assignmentId || !Number.isInteger(Number(assignmentId))) {
        delete updated[labelerId];
      } else {
        updated[labelerId] = assignmentId;
      }
      
      return updated;
    });
  }, []);

  const assignAllSelected = useCallback(async () => {
    const entries = Object.entries(selectedCustomAssignments);
    
    if (entries.length === 0) {
      throw new Error('No labelers have been assigned to any assignment.');
    }

    await assignAllSelectedOp(async () => {
      await Promise.all(
        entries.map(([labelerId, assignmentId]) => 
          SubjectVideoGroupAssignmentService.addLabeler(assignmentId, labelerId)
        )
      );
    });
    
    setSelectedCustomAssignments({});
    refetch();
  }, [selectedCustomAssignments, assignAllSelectedOp, refetch]);

  const labelers = data?.labelers || [];
  const allLabelers = data?.allLabelers || [];
  const assignments = data?.assignments || [];
  const fetchedUnassignedLabelers = data?.unassignedLabelers || [];


  const { unassignedLabelers, assignedLabelerRows } = useMemo(() => {
    const assignedLabelerIds = new Set(assignments.flatMap(a => a.labelers?.map(l => l.id) || []));
    
    const rows = assignments.flatMap(assignment =>
      (assignment.labelers || []).map(labeler => ({
        labelerId: labeler.id,
        labelerName: labeler.name,
        videoGroupName: assignment.videoGroupName || 'Unknown',
        subjectName: assignment.subjectName || 'Unknown',
        assignmentId: assignment.id
      }))
    );

    return { unassignedLabelers: fetchedUnassignedLabelers, assignedLabelerRows: rows };
  }, [fetchedUnassignedLabelers, assignments]);

  return {
    projectId,
    labelers,
    allLabelers,
    assignments,
    unassignedLabelers,
    assignedLabelerRows,
    selectedLabeler,
    setSelectedLabeler,
    selectedAssignment,
    setSelectedAssignment,
    selectedCustomAssignments,
    setSelectedCustomAssignments,
    handleCustomLabelerAssignmentChange,
    assignLabeler,
    unassignLabeler,
    distributeLabelers,
    unassignAllLabelers,
    assignAllSelected,
    loading,
    assignLoading,
    unassignLoading,
    distributeLoading,
    unassignAllLoading,
    assignAllSelectedLoading,
    error,
    refetch
  };
};
