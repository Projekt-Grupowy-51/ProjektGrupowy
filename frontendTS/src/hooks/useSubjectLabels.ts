import { useCallback, useEffect, useState } from 'react';
import { Label } from '../models/Label';
import { getSubjectLabels } from '../services/api/subjectService';

export default function useSubjectLabels(id?: number) {
  const [labels, setLabels] = useState<Label[]>([]);

  const fetchLabels = useCallback(async () => {
    if (!id) return;
    const data = await getSubjectLabels(id);
    setLabels(data.filter(l => l.subjectId === id).sort((a, b) => (a.id || 0) - (b.id || 0)));
  }, [id]);

  useEffect(() => {
    fetchLabels();
  }, [fetchLabels]);

  return { labels, fetchLabels };
}
