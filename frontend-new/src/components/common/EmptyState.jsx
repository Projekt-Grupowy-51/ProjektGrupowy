import React from 'react';
import PropTypes from 'prop-types';
import { Alert, Button } from '../ui';

const EmptyState = ({ 
  icon = "fas fa-inbox",
  title = "No data found",
  message = "There are no items to display",
  actionText = null,
  onAction = null,
  variant = "info",
  className = ""
}) => {
  return (
    <Alert variant={variant} className={className}>
      <div className="text-center py-3">
        <i className={`${icon} fs-1 text-muted opacity-50 mb-3`}></i>
        <h5 className="mb-2">{title}</h5>
        <p className="text-muted mb-0">{message}</p>
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