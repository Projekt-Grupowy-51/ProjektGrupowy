import React, { createContext, useCallback, useContext, useEffect, useState } from "react";
import authService from "../auth";
import keycloak from "../keycloak";

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
    const [isAuthenticated, setIsAuthenticated] = useState(null);
    const [roles, setRoles] = useState([]);
    const [keycloakInitialized, setKeycloakInitialized] = useState(false);

    useEffect(() => {
        const initKeycloak = async () => {
            try {
                const authenticated = await keycloak.init({
                    onLoad: 'check-sso',
                    checkLoginIframe: false,
                });
                
                setKeycloakInitialized(true);
                setIsAuthenticated(authenticated);
                
                if (authenticated) {
                    setRoles(authService.getRoles());
                    
                    // Automatyczne odświeżanie tokenu
                    const interval = setInterval(() => {
                        authService.updateToken().catch(() => {
                            authService.logout();
                        });
                    }, 60000);
                    
                    return () => clearInterval(interval);
                }
            } catch (error) {
                console.error('Keycloak initialization failed:', error);
                setKeycloakInitialized(true);
                setIsAuthenticated(false);
                setRoles([]);
            }
        };

        initKeycloak();
    }, []);

    const handleLogin = useCallback(async () => {
        try {
            await authService.login();
            setIsAuthenticated(true);
            setRoles(authService.getRoles());
        } catch (error) {
            throw error;
        }
    }, []);

    const handleLogout = useCallback(async () => {
        try {
            await authService.logout();
            setIsAuthenticated(false);
            setRoles([]);
        } catch (error) {
            console.error("Logout failed:", error);
        }
    }, []);

    const hasRole = useCallback((role) => authService.hasRole(role), []);

    if (!keycloakInitialized) {
        return <div>Loading Keycloak...</div>;
    }

    return (
        <AuthContext.Provider
            value={{
                isAuthenticated,
                roles,
                handleLogin,
                handleLogout,
                hasRole,
                userInfo: authService.getUserInfo(),
            }}
        >
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => useContext(AuthContext);
