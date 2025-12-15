import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useNotification } from '../context/NotificationContext';
import { useTranslation } from 'react-i18next';
import { getLabel, updateLabel } from '../services/api/labelService';
import { getSubject } from '../services/api/subjectService';

interface LabelFormData {
  name: string;
  colorHex: string;
  type: string;
  shortcut: string;
  subjectId: number | null;
}

export default function useLabelEdit(id?: number) {
  const [formData, setFormData] = useState<LabelFormData>({
    name: '',
    colorHex: '#ffffff',
    type: 'range',
    shortcut: '',
    subjectId: null,
  });
  const [subjectName, setSubjectName] = useState('');
  const navigate = useNavigate();
  const { addNotification } = useNotification();
  const { t } = useTranslation(['labels', 'common']);

  useEffect(() => {
    const fetchLabelData = async () => {
      if (!id) return;
      const data = await getLabel(id);
      setFormData({ ...data });
      fetchSubjectName(data.subjectId);
    };
    if (id) fetchLabelData();
  }, [id]);

  const fetchSubjectName = async (subjectId: number) => {
    const subject = await getSubject(subjectId);
    setSubjectName(subject.name);
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const validateForm = () => {
    if (formData.shortcut.length !== 1) {
      addNotification(t('labels:notification.validation.shortcut'), 'error');
      return false;
    }
    if (!/^#[0-9A-Fa-f]{6}$/.test(formData.colorHex)) {
      addNotification(t('labels:notification.validation.color'), 'error');
      return false;
    }
    if (!formData.name) {
      addNotification(t('labels:notification.validation.name'), 'error');
      return false;
    }
    return true;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validateForm() || !id) return;
    await updateLabel(id, formData as any);
    navigate(`/subjects/${formData.subjectId}`);
  };

  return { formData, subjectName, setFormData, handleChange, handleSubmit };
}
