import React from 'react';
import PropTypes from 'prop-types';
import ActionButtons from './ActionButtons.jsx';

const DetailSection = ({
  title,
  icon,
  actions,
  children,
  className = '',
  showHeader = true,
  ...props
}) => {
  return (
    <div className={className} {...props}>
      {showHeader && (
        <div className="d-flex justify-content-between align-items-center mb-4">
          <h2 className="section-title mb-0">
            {icon && <i className={`${icon} me-2`}></i>}
            {title}
          </h2>
          {actions && (
            <ActionButtons>
              {actions}
            </ActionButtons>
          )}
        </div>
      )}
      {children}
    </div>
  );
};

DetailSection.propTypes = {
  title: PropTypes.string,
  icon: PropTypes.string,
  actions: PropTypes.node,
  children: PropTypes.node.isRequired,
  className: PropTypes.string,
  showHeader: PropTypes.bool
};

export default DetailSection;