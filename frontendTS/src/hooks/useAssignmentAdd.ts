import { useCallback, useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { getProjectSubjects, getProjectVideoGroups } from '../services/api/projectService';
import { createAssignment } from '../services/api/assignmentService';
import { Subject } from '../models/Subject';
import { VideoGroup } from '../models/VideoGroup';

interface AssignmentForm {
  subjectId: string;
  videoGroupId: string;
}

export default function useAssignmentAdd(search: string) {
  const navigate = useNavigate();
  const { t } = useTranslation(['assignments', 'common']);
  const [formData, setFormData] = useState<AssignmentForm>({ subjectId: '', videoGroupId: '' });
  const [subjects, setSubjects] = useState<Subject[]>([]);
  const [videoGroups, setVideoGroups] = useState<VideoGroup[]>([]);
  const [projectId, setProjectId] = useState<number | null>(null);

  const fetchData = useCallback(async (id: number) => {
    const [subjectsData, videoGroupsData] = await Promise.all([
      getProjectSubjects(id),
      getProjectVideoGroups(id),
    ]);
    setSubjects(subjectsData);
    setVideoGroups(videoGroupsData);
  }, []);

  useEffect(() => {
    const params = new URLSearchParams(search);
    const projectIdParam = params.get('projectId');
    if (projectIdParam) {
      const pid = parseInt(projectIdParam);
      setProjectId(pid);
      fetchData(pid);
    }
  }, [search, fetchData]);

  const handleChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!formData.subjectId || !formData.videoGroupId) return;
    await createAssignment({
      subjectId: parseInt(formData.subjectId),
      videoGroupId: parseInt(formData.videoGroupId),
    });
    navigate(`/projects/${projectId}`);
  };

  return {
    formData,
    subjects,
    videoGroups,
    projectId,
    handleChange,
    handleSubmit,
  };
}
