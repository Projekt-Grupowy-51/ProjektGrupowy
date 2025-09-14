import React from 'react';
import { useTranslation } from 'react-i18next';
import { Container, Card } from '../../components/ui';

const Forbidden = () => {
  const { t } = useTranslation('errors');

  return (
    <Container className="d-flex align-items-center justify-content-center min-vh-100">
      <Card className="text-center max-width-400">
        <Card.Body className="p-5">
          <div className="mb-4">
            <i className="fas fa-ban text-danger" style={{ fontSize: '4rem' }}></i>
          </div>
          <Card.Title level={1} className="display-1 fw-bold text-danger mb-3">
            403
          </Card.Title>
          <Card.Title level={2} className="mb-3">
            {t('forbidden.title')}
          </Card.Title>
          <p className="text-muted mb-0">
            {t('forbidden.message')}
          </p>
        </Card.Body>
      </Card>
    </Container>
  );
};

export default Forbidden;