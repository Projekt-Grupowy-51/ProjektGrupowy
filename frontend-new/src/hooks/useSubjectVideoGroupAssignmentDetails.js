import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { useConfirmDialog } from './common/useConfirmDialog.js';
import SubjectVideoGroupAssignmentService from '../services/SubjectVideoGroupAssignmentService.js';
import SubjectService from '../services/SubjectService.js';
import VideoGroupService from '../services/VideoGroupService.js';

export const useSubjectVideoGroupAssignmentDetails = () => {
  const { t } = useTranslation(['assignments', 'common']);
  const { id } = useParams();
  const navigate = useNavigate();
  const { confirmWithPrompt } = useConfirmDialog();

  const [assignmentDetails, setAssignmentDetails] = useState(null);
  const [subject, setSubject] = useState(null);
  const [videoGroup, setVideoGroup] = useState(null);
  const [labelers, setLabelers] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const fetchAllData = async () => {
    setLoading(true);
    setError('');
    
    try {
      const assignment = await SubjectVideoGroupAssignmentService.getById(id);
      setAssignmentDetails(assignment);
      
      const [subjectData, videoGroupData, labelersData] = await Promise.all([
        SubjectService.getById(assignment.subjectId),
        VideoGroupService.getById(assignment.videoGroupId),
        SubjectVideoGroupAssignmentService.getLabelers(id)
      ]);
      
      setSubject(subjectData);
      setVideoGroup(videoGroupData);
      setLabelers(labelersData);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchAllData();
  }, [id]);

  const handleDelete = async () => {
    await confirmWithPrompt(t('assignments:confirm.delete'), async () => {
      await SubjectVideoGroupAssignmentService.delete(id);
      if (assignmentDetails?.projectId) {
        navigate(`/projects/${assignmentDetails.projectId}`);
      }
    });
  };

  const handleRefresh = () => {
    fetchAllData();
  };

  const handleBack = () => {
    if (assignmentDetails?.projectId) {
      navigate(`/projects/${assignmentDetails.projectId}`);
    } else {
      navigate(-1);
    }
  };

  return {
    assignmentDetails,
    subject,
    videoGroup,
    labelers,
    loading,
    error,
    handleDelete,
    handleRefresh,
    handleBack
  };
};
