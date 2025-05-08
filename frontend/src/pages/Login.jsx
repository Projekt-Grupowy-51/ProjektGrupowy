import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import authService from "../auth";
import { useAuth } from "../context/AuthContext";
import { useNotification } from "../context/NotificationContext";
import { useTranslation } from 'react-i18next';
import "./css/ScientistProjects.css";

const roleMap = {
    Labeler: "Labeler",
    Scientist: "Scientist",
};

const AuthPage = () => {
    const [isLoginView, setIsLoginView] = useState(true);
    const [formData, setFormData] = useState({
        username: "",
        email: "",
        password: "",
        confirmPassword: "",
        ScientistCreateToken: "",
        role: "Labeler",
    });
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();
    const { addNotification } = useNotification();
    const { t } = useTranslation(['auth', 'common']);
    const { handleLogin, roles, isAuthenticated } = useAuth();
    const [loginSuccess, setLoginSuccess] = useState(false);

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

    useEffect(() => {
        if (loginSuccess) {
            addNotification(t('auth:notification.login_success'), "success");
            redirectIfAuthenticated();
            setLoginSuccess(false);
        }
    }, [loginSuccess, roles, navigate, t, addNotification]);

    const handleInputChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);

        if (!isLoginView && formData.password !== formData.confirmPassword) {
            addNotification(t('auth:error.password_mismatch'), "error");
            setLoading(false);
            return;
        }

        try {
            if (isLoginView) {
                await handleLogin(formData.username, formData.password);
                setLoginSuccess(true);
            } else {
                await authService.register({
                    userName: formData.username,
                    email: formData.email,
                    password: formData.password,
                    role: roleMap[formData.role],
                    scientistCreateToken: formData.role === "Scientist" ? formData.ScientistCreateToken : undefined,
                });
                setIsLoginView(true);
                setFormData({ ...formData, password: "", confirmPassword: "" });
                addNotification(t('auth:notification.registration_success'), "success");
            }
        } catch (err) {
            addNotification(
                err.message || t(isLoginView ? 'auth:error.invalid_credentials' : 'auth:error.registration_failed'),
                "error"
            );
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="container py-5" style={{ maxWidth: "600px" }}>
            <div className="bg-primary text-white text-center py-4 rounded-top">
                <h1 className="heading mb-0 text-white">
                    {isLoginView ? t('auth:welcome_back') : t('auth:create_account')}
                </h1>
                <p className="mt-2 mb-0">
                    {isLoginView ? t('auth:sign_in_cta') : t('auth:join_community')}
                </p>
            </div>

            <div className="card shadow-sm p-4 rounded-bottom">
                <ul className="nav nav-pills nav-justified mb-4">
                    <li className="nav-item">
                        <button
                            className={`nav-link w-100 ${isLoginView ? "active" : ""}`}
                            onClick={() => setIsLoginView(true)}
                        >
                            <i className="fas fa-sign-in-alt me-2"></i>
                            {t('auth:button.login')}
                        </button>
                    </li>
                    <li className="nav-item">
                        <button
                            className={`nav-link w-100 ${!isLoginView ? "active" : ""}`}
                            onClick={() => setIsLoginView(false)}
                        >
                            <i className="fas fa-user-plus me-2"></i>
                            {t('auth:button.register')}
                        </button>
                    </li>
                </ul>

                <form onSubmit={handleSubmit}>
                    <div className="mb-3">
                        <label htmlFor="username" className="form-label">
                            {t('auth:username')}
                        </label>
                        <div className="input-group">
                            <span className="input-group-text">
                                <i className="fas fa-user"></i>
                            </span>
                            <input
                                type="text"
                                id="username"
                                name="username"
                                className="form-control"
                                placeholder={t('auth:username_placeholder')}
                                value={formData.username}
                                onChange={handleInputChange}
                                required
                                disabled={loading}
                            />
                        </div>
                    </div>

                    {!isLoginView && (
                        <>
                            <div className="mb-3">
                                <label htmlFor="email" className="form-label">
                                    {t('auth:email')}
                                </label>
                                <div className="input-group">
                                    <span className="input-group-text">
                                        <i className="fas fa-envelope"></i>
                                    </span>
                                    <input
                                        type="email"
                                        id="email"
                                        name="email"
                                        className="form-control"
                                        placeholder={t('auth:email_placeholder')}
                                        value={formData.email}
                                        onChange={handleInputChange}
                                        required
                                        disabled={loading}
                                    />
                                </div>
                            </div>
                            <div className="mb-3">
                                <label htmlFor="role" className="form-label">
                                    {t('auth:role')}
                                </label>
                                <div className="input-group">
                                    <span className="input-group-text">
                                        <i className="fas fa-user-tag"></i>
                                    </span>
                                    <select
                                        id="role"
                                        name="role"
                                        className="form-select"
                                        value={formData.role}
                                        onChange={handleInputChange}
                                        required
                                        disabled={loading}
                                    >
                                        <option value="Labeler">{t('auth:roles.labeler')}</option>
                                        <option value="Scientist">{t('auth:roles.scientist')}</option>
                                    </select>
                                </div>
                            </div>

                            {!isLoginView && formData.role === "Scientist" && (
                                <div className="mb-3">
                                    <label htmlFor="ScientistCreateToken" className="form-label">
                                        {t('auth:scientist_token', 'Scientist Token')}
                                    </label>
                                    <div className="input-group">
                                        <span className="input-group-text">
                                            <i className="fas fa-key"></i>
                                        </span>
                                        <input
                                            type="text"
                                            id="ScientistCreateToken"
                                            name="ScientistCreateToken"
                                            className="form-control"
                                            placeholder={t('auth:scientist_token_placeholder', 'Enter scientist registration token')}
                                            value={formData.ScientistCreateToken}
                                            onChange={handleInputChange}
                                            required={formData.role === "Scientist"}
                                            disabled={loading}
                                        />
                                    </div>
                                    <small className="form-text text-muted">
                                        {t('auth:scientist_token_info', 'A token is required to register as a Scientist')}
                                    </small>
                                </div>
                            )}
                        </>
                    )}

                    <div className="mb-3">
                        <label htmlFor="password" className="form-label">
                            {t('auth:password')}
                        </label>
                        <div className="input-group">
                            <span className="input-group-text">
                                <i className="fas fa-lock"></i>
                            </span>
                            <input
                                type="password"
                                id="password"
                                name="password"
                                className="form-control"
                                placeholder={t('auth:password_placeholder')}
                                value={formData.password}
                                onChange={handleInputChange}
                                required
                                disabled={loading}
                            />
                        </div>
                        {!isLoginView && (
                            <div className="form-text text-muted">
                                <small>
                                    {t('auth:password_requirements.title')}
                                    <ul className="mb-0 ps-3">
                                        <li>{t('auth:password_requirements.lowercase')}</li>
                                        <li>{t('auth:password_requirements.uppercase')}</li>
                                        <li>{t('auth:password_requirements.number')}</li>
                                    </ul>
                                </small>
                            </div>
                        )}
                    </div>

                    {!isLoginView && (
                        <div className="mb-3">
                            <label htmlFor="confirmPassword" className="form-label">
                                {t('auth:confirm_password')}
                            </label>
                            <div className="input-group">
                                <span className="input-group-text">
                                    <i className="fas fa-lock"></i>
                                </span>
                                <input
                                    type="password"
                                    id="confirmPassword"
                                    name="confirmPassword"
                                    className="form-control"
                                    placeholder={t('auth:confirm_password_placeholder')}
                                    value={formData.confirmPassword}
                                    onChange={handleInputChange}
                                    required
                                    disabled={loading}
                                />
                            </div>
                        </div>
                    )}

                    <button
                        type="submit"
                        className="btn btn-primary w-100 mt-3 py-2"
                        disabled={loading}
                    >
                        {loading ? (
                            <>
                                <span
                                    className="spinner-border spinner-border-sm me-2"
                                    role="status"
                                    aria-hidden="true"
                                ></span>
                                {isLoginView
                                    ? t('auth:button.signing_in')
                                    : t('auth:button.creating_account')}
                            </>
                        ) : (
                            <>
                                <i
                                    className={`fas ${isLoginView ? "fa-sign-in-alt" : "fa-user-plus"
                                        } me-2`}
                                ></i>
                                {isLoginView
                                    ? t('auth:button.login')
                                    : t('auth:button.register')}
                            </>
                        )}
                    </button>
                </form>

                <div className="text-center mt-4">
                    {isLoginView ? (
                        <p className="mb-0">
                            {t('auth:no_account')}{' '}
                            <button
                                className="btn btn-link p-0"
                                onClick={() => setIsLoginView(false)}
                                type="button"
                            >
                                {t('auth:sign_up_now')}
                            </button>
                        </p>
                    ) : (
                        <p className="mb-0">
                            {t('auth:have_account')}{' '}
                            <button
                                className="btn btn-link p-0"
                                onClick={() => setIsLoginView(true)}
                                type="button"
                            >
                                {t('auth:sign_in_here')}
                            </button>
                        </p>
                    )}
                </div>
            </div>
        </div>
    );
};

export default AuthPage;
