import React, { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../KeycloakProvider";
import { useNotification } from "../context/NotificationContext";
import { useTranslation } from 'react-i18next';
import "./css/ScientistProjects.css";

const AuthPage = () => {
    const navigate = useNavigate();
    const { addNotification } = useNotification();
    const { t } = useTranslation(['auth', 'common']);
    const { handleLogin, roles, isAuthenticated } = useAuth();

    const redirectIfAuthenticated = () => {
        if (isAuthenticated) {
            if (roles.includes("Scientist")) {
                navigate("/projects");
            } else if (roles.includes("Labeler")) {
                navigate("/labeler-video-groups");
            } else {
                addNotification(t('auth:error.no_access'), "error");
            }
        }
    };

    useEffect(() => {
        redirectIfAuthenticated();
    }, [isAuthenticated, roles, navigate, t, addNotification]);

    const handleKeycloakLogin = async () => {
        try {
            await handleLogin();
        } catch (err) {
            addNotification(
                err.message || t('auth:error.login_failed'),
                "error"
            );
        }
    };

    // Jeśli użytkownik nie jest zalogowany, przekieruj do Keycloak
    if (isAuthenticated === false) {
        return (
            <div className="container py-5" style={{ maxWidth: "600px" }}>
                <div className="bg-primary text-white text-center py-4 rounded-top">
                    <h1 className="heading mb-0 text-white">
                        {t('auth:welcome_back')}
                    </h1>
                    <p className="mt-2 mb-0">
                        {t('auth:keycloak_login_required')}
                    </p>
                </div>
                <div className="bg-white p-4 rounded-bottom shadow">
                    <div className="text-center">
                        <p className="mb-4">{t('auth:keycloak_login_description')}</p>
                        <button 
                            className="btn btn-primary btn-lg"
                            onClick={handleKeycloakLogin}
                        >
                            <i className="fas fa-sign-in-alt me-2"></i>
                            {t('auth:login_with_keycloak')}
                        </button>
                    </div>
                </div>
            </div>
        );
    }

    // Jeśli użytkownik jest zalogowany, pokaż loading lub przekieruj
    return (
        <div className="container py-5" style={{ maxWidth: "600px" }}>
            <div className="bg-primary text-white text-center py-4 rounded-top">
                <h1 className="heading mb-0 text-white">
                    {t('auth:redirecting')}
                </h1>
            </div>
            <div className="bg-white p-4 rounded-bottom shadow">
                <div className="text-center">
                    <div className="spinner-border text-primary mb-3" role="status">
                        <span className="visually-hidden">Loading...</span>
                    </div>
                    <p>{t('auth:redirecting_message')}</p>
                </div>
            </div>
        </div>
    );
};

export default AuthPage;
