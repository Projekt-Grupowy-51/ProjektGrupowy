import React from 'react';
import { useLoading } from '../context/LoadingContext';

const LoadingScreen = () => {
  const { isLoading } = useLoading();
  if (!isLoading) return null;
  return (
    <div className="loading-overlay">
      <div className="spinner" />
    </div>
  );
};

export default LoadingScreen;
