import React from 'react';
import { Container, Card } from '../../components/ui';

const NotFound = () => {
  return (
    <Container className="d-flex align-items-center justify-content-center min-vh-100">
      <Card className="text-center max-width-400">
        <Card.Body className="p-5">
          <div className="mb-4">
            <i className="fas fa-search text-warning" style={{ fontSize: '4rem' }}></i>
          </div>
          <Card.Title level={1} className="display-1 fw-bold text-warning mb-3">
            404
          </Card.Title>
          <Card.Title level={2} className="mb-3">
            Page Not Found
          </Card.Title>
          <p className="text-muted mb-0">
            The page you're looking for doesn't exist or has been moved.
          </p>
        </Card.Body>
      </Card>
    </Container>
  );
};

export default NotFound;