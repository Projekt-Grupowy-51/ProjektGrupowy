import React from 'react';
import PropTypes from 'prop-types';
import { Container } from '../ui';

const LoadingSpinner = ({ 
  message = "Loading...", 
  size = "default", 
  centered = true,
  className = "" 
}) => {
  const sizeClass = size === "small" ? "spinner-border-sm" : "";
  const containerClass = centered ? "text-center py-4" : "";

  const spinner = (
    <div className={`spinner-border text-primary ${sizeClass}`} role="status">
      <span className="visually-hidden">{message}</span>
    </div>
  );

  if (centered) {
    return (
      <div className={`${containerClass} ${className}`}>
        {spinner}
        {message && <div className="mt-2 text-muted">{message}</div>}
      </div>
    );
  }

  return (
    <div className={className}>
      {spinner}
      {message && <span className="ms-2 text-muted">{message}</span>}
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