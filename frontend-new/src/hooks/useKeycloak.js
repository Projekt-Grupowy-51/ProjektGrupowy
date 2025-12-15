import { useState, useEffect } from "react";
import keycloak from "../services/keycloak.js";

let isInitialized = false;
let initPromise = null;

export const useKeycloak = () => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [user, setUser] = useState(null);

  useEffect(() => {
    const initKeycloak = async () => {
      if (isInitialized) {
        // Atomic state update: set user first, then auth, then loading
        const authState = keycloak.authenticated || false;
        const userData = keycloak.tokenParsed || null;
        setUser(userData);
        setIsAuthenticated(authState);
        setIsLoading(false);
        return;
      }

      if (initPromise) {
        await initPromise;
        // Atomic state update: set user first, then auth, then loading
        const authState = keycloak.authenticated || false;
        const userData = keycloak.tokenParsed || null;
        setUser(userData);
        setIsAuthenticated(authState);
        setIsLoading(false);
        return;
      }

      try {
        initPromise = keycloak.init({
          onLoad: "check-sso",
          checkLoginIframe: false,
          redirectUri: window.location.origin + "/",
        });

        const authenticated = await initPromise;
        isInitialized = true;

        if (authenticated) {
          await new Promise((resolve) => setTimeout(resolve, 100));
        }

        // Atomic state update: set user first, then auth, then loading
        const userData = authenticated ? keycloak.tokenParsed : null;
        setUser(userData);
        setIsAuthenticated(authenticated);
        setIsLoading(false);

        if (authenticated) {
          setInterval(() => {
            keycloak.updateToken(70).catch(() => {
              login();
            });
          }, 60000);
        }
      } catch (error) {
        isInitialized = true;
        // Ensure loading is turned off even on error
        setIsLoading(false);
      }
    };

    initKeycloak();
  }, []);

  const login = () => {
    // Always redirect to root after login to trigger proper role-based routing
    keycloak.login({ redirectUri: window.location.origin + "/" });
  };
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
