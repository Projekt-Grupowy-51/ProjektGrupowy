import React from 'react';
import PropTypes from 'prop-types';

const PageHeader = ({
  title,
  subtitle,
  icon,
  actions,
  className = '',
  ...props
}) => {
  return (
    <div className={`d-flex justify-content-between align-items-center mb-4 ${className}`} {...props}>
      <div>
        <h1 className="mb-0">
          {icon && <i className={`${icon} me-2`}></i>}
          {title}
        </h1>
        {subtitle && <p className="text-muted mb-0">{subtitle}</p>}
      </div>
      {actions && (
        <div className="d-flex gap-2">
          {actions}
        </div>
      )}
    </div>
  );
};

PageHeader.propTypes = {
  title: PropTypes.string.isRequired,
  subtitle: PropTypes.string,
  icon: PropTypes.string,
  actions: PropTypes.node,
  className: PropTypes.string
};

export default PageHeader;