import { useState, useEffect } from 'react';
import keycloak from '../services/keycloak.js';

let isInitialized = false;
let initPromise = null;

export const useKeycloak = () => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [user, setUser] = useState(null);

  useEffect(() => {
    const initKeycloak = async () => {
      if (isInitialized) {
        setIsAuthenticated(keycloak.authenticated || false);
        setUser(keycloak.tokenParsed || null);
        setIsLoading(false);
        return;
      }

      if (initPromise) {
        await initPromise;
        setIsAuthenticated(keycloak.authenticated || false);
        setUser(keycloak.tokenParsed || null);
        setIsLoading(false);
        return;
      }

      try {
        initPromise = keycloak.init({
          onLoad: 'check-sso',
          checkLoginIframe: false,
        });

        const authenticated = await initPromise;
        isInitialized = true;
        
        setIsAuthenticated(authenticated);
        setUser(authenticated ? keycloak.tokenParsed : null);

        if (authenticated) {
          setInterval(() => {
            keycloak.updateToken(70).catch(() => {
              console.log('Token refresh failed');
              login();
            });
          }, 60000);
        }
      } catch (error) {
        console.error('Keycloak init failed:', error);
        isInitialized = true;
      } finally {
        setIsLoading(false);
      }
    };

    initKeycloak();
  }, []);

  const login = () => keycloak.login();
  const logout = () => keycloak.logout();
  const getToken = () => keycloak.token;

  return {
    isAuthenticated,
    isLoading,
    user,
    login,
    logout,
    getToken,
  };
};