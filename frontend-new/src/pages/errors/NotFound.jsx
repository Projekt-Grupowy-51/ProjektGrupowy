import React from 'react';
import { useTranslation } from 'react-i18next';
import { Container, Card } from '../../components/ui';

const NotFound = () => {
  const { t } = useTranslation('errors');

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
            {t('notFound.title')}
          </Card.Title>
          <p className="text-muted mb-0">
            {t('notFound.message')}
          </p>
        </Card.Body>
      </Card>
    </Container>
  );
};

export default NotFound;