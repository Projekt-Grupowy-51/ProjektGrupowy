import { useState, useEffect, useMemo } from 'react';
import { useParams } from 'react-router-dom';
import ProjectService from '../services/ProjectService.js';
import SubjectVideoGroupAssignmentService from '../services/SubjectVideoGroupAssignmentService.js';

export const useProjectLabelers = (passedProjectId) => {
  const { projectId: paramId } = useParams();
  const projectId = passedProjectId ? parseInt(passedProjectId, 10) : parseInt(paramId, 10);

  const [selectedLabeler, setSelectedLabeler] = useState('');
  const [selectedAssignment, setSelectedAssignment] = useState('');
  const [selectedCustomAssignments, setSelectedCustomAssignments] = useState({});
  
  const [data, setData] = useState({ labelers: [], allLabelers: [], unassignedLabelers: [], assignments: [] });
  const [loading, setLoading] = useState(false);
  const [assignLoading, setAssignLoading] = useState(false);
  const [unassignLoading, setUnassignLoading] = useState(false);
  const [distributeLoading, setDistributeLoading] = useState(false);
  const [unassignAllLoading, setUnassignAllLoading] = useState(false);
  const [assignAllSelectedLoading, setAssignAllSelectedLoading] = useState(false);
  const [error, setError] = useState(null);

  const fetchLabelerData = () => {
    if (!projectId) return Promise.resolve();
    
    setLoading(true);
    setError(null);
    
    return Promise.all([
      ProjectService.getUsers(projectId),
      ProjectService.getUnassignedLabelers(projectId),
      ProjectService.getAssignments(projectId)
    ])
      .then(([allLabelers, unassignedLabelers, assignments]) => {
        setData({
          labelers: allLabelers,
          allLabelers: allLabelers,
          unassignedLabelers,
          assignments
        });
      })
      .catch(err => setError(err.message))
      .finally(() => setLoading(false));
  };

  const assignLabeler = (labelerId, assignmentId) => {
    if (!labelerId || !assignmentId) return Promise.resolve();
    
    setAssignLoading(true);
    setError(null);
    return SubjectVideoGroupAssignmentService.addLabeler(assignmentId, labelerId)
      .then(() => {
        setSelectedLabeler('');
        setSelectedAssignment('');
        return fetchLabelerData();
      })
      .catch(err => setError(err.message))
      .finally(() => setAssignLoading(false));
  };

  const unassignLabeler = (assignmentId, labelerId) => {
    if (!assignmentId || !labelerId) return Promise.resolve();
    
    setUnassignLoading(true);
    setError(null);
    return SubjectVideoGroupAssignmentService.removeLabeler(assignmentId, labelerId)
      .then(() => fetchLabelerData())
      .catch(err => setError(err.message))
      .finally(() => setUnassignLoading(false));
  };

  const distributeLabelers = () => {
    setDistributeLoading(true);
    setError(null);
    return ProjectService.distributeLabelers(projectId)
      .then(() => fetchLabelerData())
      .catch(err => setError(err.message))
      .finally(() => setDistributeLoading(false));
  };

  const unassignAllLabelers = () => {
    setUnassignAllLoading(true);
    setError(null);
    return ProjectService.unassignAllLabelers(projectId)
      .then(() => fetchLabelerData())
      .catch(err => setError(err.message))
      .finally(() => setUnassignAllLoading(false));
  };

  const handleCustomLabelerAssignmentChange = (labelerId, assignmentId) => {
    setSelectedCustomAssignments(prev => {
      const updated = { ...prev };
      
      if (!assignmentId || !Number.isInteger(Number(assignmentId))) {
        delete updated[labelerId];
      } else {
        updated[labelerId] = assignmentId;
      }
      
      return updated;
    });
  };

  const assignAllSelected = () => {
    const entries = Object.entries(selectedCustomAssignments);
    
    if (entries.length === 0) {
      setError('No labelers have been assigned to any assignment.');
      return Promise.reject(new Error('No labelers have been assigned to any assignment.'));
    }

    setAssignAllSelectedLoading(true);
    setError(null);
    
    return Promise.all(
      entries.map(([labelerId, assignmentId]) => 
        SubjectVideoGroupAssignmentService.addLabeler(assignmentId, labelerId)
      )
    )
      .then(() => {
        setSelectedCustomAssignments({});
        return fetchLabelerData();
      })
      .catch(err => setError(err.message))
      .finally(() => setAssignAllSelectedLoading(false));
  };

  useEffect(() => {
    fetchLabelerData();
  }, [projectId]);

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
    refetch: fetchLabelerData
  };
};
