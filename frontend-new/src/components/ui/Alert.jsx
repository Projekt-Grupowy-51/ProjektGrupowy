import React from 'react';
import PropTypes from 'prop-types';
import { useTranslation } from 'react-i18next';

const Alert = ({ 
  children, 
  variant = 'primary', 
  dismissible = false, 
  onClose,
  className = '',
  ...props 
}) => {
  const { t } = useTranslation('common');
  const getVariantClass = () => {
    const variants = {
      primary: 'alert-primary',
      secondary: 'alert-secondary',
      success: 'alert-success',
      danger: 'alert-danger',
      warning: 'alert-warning',
      info: 'alert-info',
      light: 'alert-light',
      dark: 'alert-dark'
    };
    return variants[variant] || variants.primary;
  };

  const classes = [
    'alert',
    getVariantClass(),
    dismissible ? 'alert-dismissible' : '',
    className
  ].filter(Boolean).join(' ');

  return (
    <div className={classes} role="alert" {...props}>
      {children}
      {dismissible && (
        <button
          type="button"
          className="btn-close"
          onClick={onClose}
          aria-label={t('buttons.close')}
        ></button>
      )}
    </div>
  );
};

Alert.propTypes = {
  children: PropTypes.node.isRequired,
  variant: PropTypes.oneOf([
    'primary', 'secondary', 'success', 'danger', 
    'warning', 'info', 'light', 'dark'
  ]),
  dismissible: PropTypes.bool,
  onClose: PropTypes.func,
  className: PropTypes.string
};

export default Alert;
