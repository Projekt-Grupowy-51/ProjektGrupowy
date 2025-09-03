import React from 'react';
import { useKeycloak } from '../../hooks/useKeycloak.js';
import { Container, Card, Button } from '../ui';
import { LoadingSpinner } from '../common';

const AuthGuard = ({ children, fallback = null }) => {
  const { isAuthenticated, isLoading, login } = useKeycloak();

  if (isLoading) {
    return (
      <Container className="d-flex justify-content-center align-items-center vh-100">
        <LoadingSpinner message="Initializing authentication..." />
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
              Authentication Required
            </Card.Title>
          </Card.Header>
          <Card.Body>
            <p className="text-muted mb-4">
              Please log in to access VidMark platform.
            </p>
            <Button 
              variant="primary" 
              onClick={login}
              icon="fas fa-sign-in-alt"
            >
              Login with Keycloak
            </Button>
          </Card.Body>
        </Card>
      </Container>
    );
  }

  return children;
};

export default AuthGuard;