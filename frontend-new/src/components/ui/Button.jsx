import React from 'react';
import PropTypes from 'prop-types';

const Button = ({ 
  children, 
  variant = 'primary', 
  size = 'md', 
  type = 'button',
  disabled = false,
  loading = false,
  icon,
  onClick,
  className = '',
  ...props 
}) => {
  const getVariantClass = () => {
    const variants = {
      primary: 'btn-primary',
      secondary: 'btn-secondary',
      success: 'btn-success',
      danger: 'btn-danger',
      warning: 'btn-warning',
      info: 'btn-info',
      light: 'btn-light',
      dark: 'btn-dark',
      link: 'btn-link',
      outline: 'btn-primary',
      'outline-secondary': 'btn-outline-secondary',
      'outline-success': 'btn-outline-success',
      'outline-danger': 'btn-outline-danger',
      'outline-warning': 'btn-outline-warning',
      'outline-info': 'btn-outline-info',
      'outline-light': 'btn-outline-light',
      'outline-dark': 'btn-outline-dark',
    };
    return variants[variant] || variants.primary;
  };

  const getSizeClass = () => {
    const sizes = {
      sm: 'btn-sm',
      md: '',
      lg: 'btn-lg'
    };
    return sizes[size] || '';
  };

  const classes = [
    'btn',
    getVariantClass(),
    getSizeClass(),
    className
  ].filter(Boolean).join(' ');

  return (
    <button
      type={type}
      className={classes}
      disabled={disabled || loading}
      onClick={onClick}
      {...props}
    >
      {loading && (
        <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
      )}
      {icon && !loading && (
        <i className={`${icon} me-2`}></i>
      )}
      {children}
    </button>
  );
};

Button.propTypes = {
  children: PropTypes.node.isRequired,
  variant: PropTypes.oneOf([
    'primary', 'secondary', 'success', 'danger', 'warning', 'info', 'light', 'dark', 'link',
    'outline', 'outline-secondary', 'outline-success', 'outline-danger', 
    'outline-warning', 'outline-info', 'outline-light', 'outline-dark'
  ]),
  size: PropTypes.oneOf(['sm', 'md', 'lg']),
  type: PropTypes.oneOf(['button', 'submit', 'reset']),
  disabled: PropTypes.bool,
  loading: PropTypes.bool,
  icon: PropTypes.string,
  onClick: PropTypes.func,
  className: PropTypes.string
};

export default Button;
