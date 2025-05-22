import { useCallback, useEffect, useState } from 'react';
import { Subject } from '../models/Subject';
import { getSubject } from '../services/api/subjectService';

const useSubject = (id?: number) => {
  const [subject, setSubject] = useState<Subject | null>(null);

  const fetchSubject = useCallback(async () => {
    if (!id) return;
    const data = await getSubject(id);
    setSubject(data);
  }, [id]);

  useEffect(() => {
    fetchSubject();
  }, [fetchSubject]);

  return { subject, setSubject, fetchSubject };
};

export default useSubject;
