import { useState, useEffect } from 'react';
import { useAsyncOperation } from './useAsyncOperation.js';

export const useDataFetching = (fetchOperation, dependencies = []) => {
  const [data, setData] = useState(null);
  const { execute, loading, error } = useAsyncOperation();

  const fetchData = async () => {
    try {
      const result = await execute(fetchOperation);
      setData(result);
      return result;
    } catch (err) {
      setData(null);
      throw err;
    }
  };

  const refetch = () => {
    return fetchData();
  };

  useEffect(() => {
    if (fetchOperation) {
      fetchData();
    }
  }, [fetchOperation, ...dependencies]);

  return {
    data,
    loading,
    error,
    refetch
  };
};

export const useMultipleDataFetching = (operations) => {
  const [data, setData] = useState({});
  const [loading, setLoading] = useState({});
  const [error, setError] = useState({});

  const fetchData = async (key, operation) => {
    setLoading(prev => ({ ...prev, [key]: true }));
    setError(prev => ({ ...prev, [key]: null }));

    try {
      const result = await operation();
      setData(prev => ({ ...prev, [key]: result }));
      setLoading(prev => ({ ...prev, [key]: false }));
      return result;
    } catch (err) {
      setError(prev => ({ ...prev, [key]: err.message }));
      setLoading(prev => ({ ...prev, [key]: false }));
      throw err;
    }
  };

  const fetchAll = async () => {
    const promises = Object.entries(operations).map(([key, operation]) =>
      fetchData(key, operation)
    );
    return Promise.all(promises);
  };

  useEffect(() => {
    if (operations && Object.keys(operations).length > 0) {
      fetchAll();
    }
  }, []);

  return {
    data,
    loading,
    error,
    fetchData,
    refetchAll: fetchAll
  };
};