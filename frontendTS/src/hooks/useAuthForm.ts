import { useState, useEffect, useCallback } from 'react';
import authService from '../auth';
import { useAuth } from '../context/AuthContext';
import { useNotification } from '../context/NotificationContext';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';

interface FormData {
    username: string;
    email: string;
    password: string;
    confirmPassword: string;
    ScientistCreateToken?: string;
    role: 'Labeler' | 'Scientist';
}

export default function useAuthForm() {
    const [isLoginView, setIsLoginView] = useState(true);
    const [formData, setFormData] = useState<FormData>({
        username: '',
        email: '',
        password: '',
        confirmPassword: '',
        ScientistCreateToken: '',
        role: 'Labeler',
    });
    const [loading, setLoading] = useState(false);
    const [loginSuccess, setLoginSuccess] = useState(false);

    const navigate = useNavigate();
    const { t } = useTranslation(['auth']);
    const { handleLogin, roles, isAuthenticated } = useAuth();
    const { addNotification } = useNotification();

    const redirectIfAuthenticated = useCallback(() => {
        if (isAuthenticated) {
            if (roles.includes('Scientist')) {
                navigate('/projects');
            } else if (roles.includes('Labeler')) {
                navigate('/labeler-video-groups');
            } else {
                addNotification(t('auth:error.no_access'), 'error');
            }
        }
    }, [isAuthenticated, roles, navigate, addNotification, t]);

    useEffect(() => {
        redirectIfAuthenticated();
    }, [redirectIfAuthenticated]);

    useEffect(() => {
        if (loginSuccess) {
            addNotification(t('auth:notification.login_success'), 'success');
            redirectIfAuthenticated();
            setLoginSuccess(false);
        }
    }, [loginSuccess, redirectIfAuthenticated, addNotification, t]);

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);

        if (!isLoginView && formData.password !== formData.confirmPassword) {
            addNotification(t('auth:error.password_mismatch'), 'error');
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
                    role: formData.role,
                    scientistCreateToken: formData.role === 'Scientist' ? formData.ScientistCreateToken : undefined,
                });
                setIsLoginView(true);
                setFormData({ ...formData, password: '', confirmPassword: '' });
                addNotification(t('auth:notification.registration_success'), 'success');
            }
        } catch (err: any) {
            addNotification(
                err.message || t(isLoginView ? 'auth:error.invalid_credentials' : 'auth:error.registration_failed'),
                'error'
            );
        } finally {
            setLoading(false);
        }
    };

    return {
        isLoginView,
        formData,
        loading,
        setIsLoginView,
        handleInputChange,
        handleSubmit,
    };
}
