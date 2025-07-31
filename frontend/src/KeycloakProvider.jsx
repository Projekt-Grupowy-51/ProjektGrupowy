import React, { useState, useEffect, createContext, useContext, useRef } from 'react';
import keycloak from './keycloak';

// Utwórz kontekst
export const KeycloakContext = createContext();

const KeycloakProvider = ({ children }) => {
  const [keycloakInitialized, setKeycloakInitialized] = useState(false);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [userInfo, setUserInfo] = useState(null);
  const [roles, setRoles] = useState([]);
  const [initError, setInitError] = useState(null);
  const initializationRef = useRef(false);

  useEffect(() => {
    const initKeycloak = async () => {
      // Sprawdź czy Keycloak już został zainicjalizowany
      if (initializationRef.current) {
        return;
      }

      // Sprawdź czy Keycloak już ma stan inicjalizacji
      if (keycloak.authenticated !== undefined) {
        console.log('Keycloak already initialized');
        setKeycloakInitialized(true);
        setIsAuthenticated(keycloak.authenticated);
        
        if (keycloak.authenticated) {
          setUserInfo(keycloak.tokenParsed);
          
          // Pobierz role z realm i klienta
          const realmRoles = keycloak.tokenParsed?.realm_access?.roles || [];
          const clientRoles = keycloak.tokenParsed?.resource_access?.['projektgrupowy-client']?.roles || [];
          const allRoles = [...realmRoles, ...clientRoles];
          console.log('Roles:', allRoles);
          setRoles(allRoles);
        }
        return;
      }

      initializationRef.current = true;

      try {
        console.log('Initializing Keycloak...');
        const authenticated = await keycloak.init({
          onLoad: 'check-sso',
          checkLoginIframe: false,
        });
        
        console.log('Keycloak initialized, authenticated:', authenticated);
        setKeycloakInitialized(true);
        setIsAuthenticated(authenticated);
        
        if (authenticated) {
          setUserInfo(keycloak.tokenParsed);
          
          // Pobierz role z realm i klienta
          const realmRoles = keycloak.tokenParsed?.realm_access?.roles || [];
          const clientRoles = keycloak.tokenParsed?.resource_access?.['projektgrupowy-client']?.roles || [];
          const allRoles = [...realmRoles, ...clientRoles];
          console.log('Roles:', allRoles);
          setRoles(allRoles);
          
          // Automatyczne odświeżanie tokenu
          const interval = setInterval(() => {
            keycloak.updateToken(70).catch(() => {
              console.log('Token refresh failed, logging out');
              handleLogout();
            });
          }, 60000);
          
          return () => clearInterval(interval);
        }
      } catch (error) {
        console.error('Keycloak initialization failed:', error);
        setInitError(error.message);
        setKeycloakInitialized(true);
        setIsAuthenticated(false);
        setUserInfo(null);
        setRoles([]);
        initializationRef.current = false;
      }
    };

    initKeycloak();
  }, []);

  const handleLogin = async () => {
    try {
      await keycloak.login();
    } catch (error) {
      console.error('Login failed:', error);
      throw error;
    }
  };

  const handleLogout = async () => {
    try {
      // Wyloguj z przekierowaniem na stronę główną
      await keycloak.logout();
      setIsAuthenticated(false);
      setUserInfo(null);
      setRoles([]);
    } catch (error) {
      console.error("Logout failed:", error);
    }
  };

  const hasRole = (role) => {
    return roles.includes(role);
  };

  const hasUserRole = (role) => {
    return userInfo.userRole === role;
  };
  
  const getToken = () => {
    return keycloak.token;
  };

  const updateToken = async () => {
    try {
      const refreshed = await keycloak.updateToken(70);
      return refreshed;
    } catch (error) {
      console.error('Token refresh failed:', error);
      throw error;
    }
  };

  if (!keycloakInitialized) {
    return <div className="d-flex justify-content-center align-items-center vh-100">
      <div className="text-center">
        <div className="spinner-border text-primary mb-3" role="status">
          <span className="visually-hidden">Loading Keycloak...</span>
        </div>
        <p>Initializing authentication...</p>
      </div>
    </div>;
  }

  // Jeśli wystąpił błąd inicjalizacji, pokaż komunikat
  if (initError) {
    return <div className="d-flex justify-content-center align-items-center vh-100">
      <div className="text-center">
        <div className="alert alert-danger">
          <h4>Authentication Error</h4>
          <p>Failed to connect to authentication server.</p>
          <small className="text-muted">Error: {initError}</small>
          <br />
          <button 
            className="btn btn-outline-primary mt-3" 
            onClick={() => window.location.reload()}
          >
            Retry
          </button>
        </div>
      </div>
    </div>;
  }

  const contextValue = {
    // Keycloak instance
    keycloak,
    
    // Auth state
    isAuthenticated,
    userInfo,
    roles,
    
    // Auth methods
    handleLogin,
    handleLogout,
    hasRole,
    hasUserRole,
    getToken,
    updateToken,
    
    // User info helpers
    username: userInfo?.preferred_username,
    email: userInfo?.email,
    name: userInfo?.name,
  };

  return (
    <KeycloakContext.Provider value={contextValue}>
      {children}
    </KeycloakContext.Provider>
  );
};

// Hook do używania Keycloak
export const useKeycloak = () => {
  const context = useContext(KeycloakContext);
  if (!context) {
    throw new Error('useKeycloak must be used within a KeycloakProvider');
  }
  return context;
};

// Alias dla kompatybilności z poprzednim kodem
export const useAuth = useKeycloak;

export default KeycloakProvider;
