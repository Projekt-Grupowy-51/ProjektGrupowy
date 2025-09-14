import React from 'react';
import PropTypes from 'prop-types';
import { useTranslation } from 'react-i18next';
import { Alert, Button } from '../ui';

const EmptyState = ({ 
  icon = "fas fa-inbox",
  title,
  message,
  actionText = null,
  onAction = null,
  variant = "info",
  className = ""
}) => {
  const { t } = useTranslation('common');
  const displayTitle = title || t('emptyState.defaultTitle');
  const displayMessage = message || t('emptyState.defaultMessage');
  return (
    <Alert variant={variant} className={className}>
      <div className="text-center py-3">
        <i className={`${icon} fs-1 text-muted opacity-50 mb-3`}></i>
        <h5 className="mb-2">{displayTitle}</h5>
        <p className="text-muted mb-0">{displayMessage}</p>
        {actionText && onAction && (
          <div className="mt-3">
            <Button 
              variant="primary" 
              onClick={onAction}
              size="sm"
            >
              {actionText}
            </Button>
          </div>
        )}
      </div>
    </Alert>
  );
};

EmptyState.propTypes = {
  icon: PropTypes.string,
  title: PropTypes.string,
  message: PropTypes.string,
  actionText: PropTypes.string,
  onAction: PropTypes.func,
  variant: PropTypes.oneOf(['info', 'warning', 'light']),
  className: PropTypes.string
};

export default EmptyState;