import React, { createContext, useState, useEffect, useContext } from 'react';
import { subscribe } from '../services/loadingService';

const LoadingContext = createContext({ isLoading: false });
export const LoadingProvider = ({ children }) => {
  const [count, setCount] = useState(0);
  useEffect(() => subscribe(setCount), []);
  return (
    <LoadingContext.Provider value={{ isLoading: count > 0 }}>
      {children}
    </LoadingContext.Provider>
  );
};
export const useLoading = () => useContext(LoadingContext);
