import React from 'react';
import PropTypes from 'prop-types';
import { useTranslation } from 'react-i18next';
import { Alert, Button } from '../ui';

const ErrorAlert = ({ 
  error, 
  title = null,
  onRetry = null, 
  onDismiss = null,
  className = "" 
}) => {
  const { t } = useTranslation('common');
  
  if (!error) return null;

  const errorMessage = typeof error === 'string' ? error : error.message || t('error.defaultMessage');

  return (
    <Alert variant="danger" className={className}>
      <div className="d-flex align-items-start">
        <i className="fas fa-exclamation-triangle me-3 mt-1"></i>
        <div className="flex-grow-1">
          {title && <h5 className="alert-heading mb-2">{title}</h5>}
          <p className="mb-0">{errorMessage}</p>
          {(onRetry || onDismiss) && (
            <div className="mt-3 d-flex gap-2">
              {onRetry && (
                <Button 
                  size="sm" 
                  variant="outline-danger" 
                  onClick={onRetry}
                >
                  <i className="fas fa-redo me-1"></i>
                  {t('buttons.tryAgain')}
                </Button>
              )}
              {onDismiss && (
                <Button 
                  size="sm" 
                  variant="outline-secondary" 
                  onClick={onDismiss}
                >
                  {t('buttons.dismiss')}
                </Button>
              )}
            </div>
          )}
        </div>
      </div>
    </Alert>
  );
};

ErrorAlert.propTypes = {
  error: PropTypes.oneOfType([PropTypes.string, PropTypes.object]),
  title: PropTypes.string,
  onRetry: PropTypes.func,
  onDismiss: PropTypes.func,
  className: PropTypes.string
};

export default ErrorAlert;