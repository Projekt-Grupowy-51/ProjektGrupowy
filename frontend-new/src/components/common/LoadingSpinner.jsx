import React from 'react';
import PropTypes from 'prop-types';
import { useTranslation } from 'react-i18next';
import { Container } from '../ui';

const LoadingSpinner = ({ 
  message, 
  size = "default", 
  centered = true,
  className = "" 
}) => {
  const { t } = useTranslation('common');
  const displayMessage = message || t('loading');
  const sizeClass = size === "small" ? "spinner-border-sm" : "";
  const containerClass = centered ? "text-center py-4" : "";

  const spinner = (
    <div className={`spinner-border text-primary ${sizeClass}`} role="status">
      <span className="visually-hidden">{displayMessage}</span>
    </div>
  );

  if (centered) {
    return (
      <div className={`${containerClass} ${className}`}>
        {spinner}
        <div className="mt-2 text-muted">{displayMessage}</div>
      </div>
    );
  }

  return (
    <div className={className}>
      {spinner}
      <span className="ms-2 text-muted">{displayMessage}</span>
    </div>
  );
};

LoadingSpinner.propTypes = {
  message: PropTypes.string,
  size: PropTypes.oneOf(['default', 'small']),
  centered: PropTypes.bool,
  className: PropTypes.string
};

export default LoadingSpinner;