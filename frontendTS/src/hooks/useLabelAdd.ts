import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useNotification } from '../context/NotificationContext';
import { useTranslation } from 'react-i18next';
import { createLabel } from '../services/api/labelService';
import { getSubject } from '../services/api/subjectService';

interface LabelFormData {
  name: string;
  colorHex: string;
  type: string;
  shortcut: string;
  subjectId: number | null;
}

export default function useLabelAdd(search: string) {
  const [formData, setFormData] = useState<LabelFormData>({
    name: '',
    colorHex: '#3498db',
    type: 'range',
    shortcut: '',
    subjectId: null,
  });
  const [subjectName, setSubjectName] = useState('');
  const navigate = useNavigate();
  const { addNotification } = useNotification();
  const { t } = useTranslation(['labels', 'common']);

  useEffect(() => {
    const params = new URLSearchParams(search);
    const id = params.get('subjectId');
    if (id) {
      const num = parseInt(id);
      setFormData(prev => ({ ...prev, subjectId: num }));
      fetchSubjectName(num);
    }
  }, [search]);

  const fetchSubjectName = async (id: number) => {
    const subject = await getSubject(id);
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
    if (!validateForm() || !formData.subjectId) return;
    await createLabel(formData as any);
    navigate(`/subjects/${formData.subjectId}`);
  };

  return {
    formData,
    subjectName,
    setFormData,
    handleChange,
    handleSubmit,
  };
}
