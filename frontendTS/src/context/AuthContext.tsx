import React, { createContext, useCallback, useContext, useEffect, useState } from "react";
import authService from "../auth";

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
    const [isAuthenticated, setIsAuthenticated] = useState(null);
    const [roles, setRoles] = useState([]);

    useEffect(() => {
        const checkAuthStatus = async () => {
            try {
                const authData = await authService.checkAuth();
                if (authData && authData.isAuthenticated) {
                    setIsAuthenticated(true);
                    setRoles(authData.roles);
                } else {
                    setIsAuthenticated(false);
                    setRoles([]);
                }
            } catch (error) {
                setIsAuthenticated(false);
                setRoles([]);
            }
        };
        checkAuthStatus();
    }, []);

    const handleLogin = useCallback(async (username, password) => {
        try {
            const response = await authService.login(username, password);
            setIsAuthenticated(true);
            setRoles(response.roles);
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

    const hasRole = useCallback((role) => roles.includes(role), [roles]);

    return (
        <AuthContext.Provider
            value={{
                isAuthenticated,
                roles,
                handleLogin,
                handleLogout,
                hasRole,
            }}
        >
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => useContext(AuthContext);
