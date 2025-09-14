import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useKeycloak } from '../../hooks/useKeycloak.js';
import { Container, Card, Button } from '../ui';
import { LoadingSpinner } from '../common';

const AuthGuard = ({ children, fallback = null }) => {
    const { isAuthenticated, isLoading, login, user } = useKeycloak();
    const navigate = useNavigate();
    const { t } = useTranslation('common');


    useEffect(() => {
        if (isAuthenticated && user) {

            switch (user.userRole) {
                case "Scientist":
                    navigate("/projects");
                    break;
                case "Labeler":
                    navigate("/labeler-video-groups");
                    break;
                default:
                    navigate("/forbidden");
            }
        }
    }, [isAuthenticated, user, navigate]);

    if (isLoading) {
        return (
            <Container className="d-flex justify-content-center align-items-center vh-100">
                <LoadingSpinner message={t('auth.initializing')} />
            </Container>
        );
    }

    if (!isAuthenticated) {
        return fallback || (
            <Container className="d-flex justify-content-center align-items-center vh-100">
                <Card className="text-center" style={{ maxWidth: '400px', width: '100%' }}>
                    <Card.Header>
                        <Card.Title level={3}>
                            <i className="fas fa-lock me-2"></i>
                            {t('auth.required_title')}
                        </Card.Title>
                    </Card.Header>
                    <Card.Body>
                        <p className="text-muted mb-4">
                            {t('auth.required_message')}
                        </p>
                        <Button
                            variant="primary"
                            onClick={login}
                            icon="fas fa-sign-in-alt"
                        >
                            {t('auth.login_button')}
                        </Button>
                    </Card.Body>
                </Card>
            </Container>
        );
    }

    return children;
};

export default AuthGuard;
