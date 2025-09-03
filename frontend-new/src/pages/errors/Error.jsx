import React from 'react';
import { Container, Card } from '../../components/ui';

const Error = () => {
  return (
    <Container className="d-flex align-items-center justify-content-center min-vh-100">
      <Card className="text-center max-width-400">
        <Card.Body className="p-5">
          <div className="mb-4">
            <i className="fas fa-exclamation-triangle text-danger" style={{ fontSize: '4rem' }}></i>
          </div>
          <Card.Title level={1} className="display-4 fw-bold text-danger mb-3">
            Error
          </Card.Title>
          <Card.Title level={2} className="mb-3">
            Connection Issue
          </Card.Title>
          <p className="text-muted mb-0">
            We're having trouble connecting to the server. Please try again later.
          </p>
        </Card.Body>
      </Card>
    </Container>
  );
};

export default Error;