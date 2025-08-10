import React from 'react';
import PropTypes from 'prop-types';

const Input = ({
  label,
  type = 'text',
  id,
  name,
  value,
  onChange,
  placeholder,
  required = false,
  disabled = false,
  error,
  helpText,
  size = 'md',
  className = '',
  rows = 3,
  ...props
}) => {
  const inputId = id || name;
  
  const getSizeClass = () => {
    const sizes = {
      sm: 'form-control-sm',
      md: '',
      lg: 'form-control-lg'
    };
    return sizes[size] || '';
  };

  const getInputClasses = () => {
    return [
      'form-control',
      getSizeClass(),
      error ? 'is-invalid' : '',
      className
    ].filter(Boolean).join(' ');
  };

  const renderInput = () => {
    if (type === 'textarea') {
      return (
        <textarea
          id={inputId}
          name={name}
          className={getInputClasses()}
          value={value}
          onChange={onChange}
          placeholder={placeholder}
          required={required}
          disabled={disabled}
          rows={rows}
          {...props}
        />
      );
    }

    return (
      <input
        type={type}
        id={inputId}
        name={name}
        className={getInputClasses()}
        value={value}
        onChange={onChange}
        placeholder={placeholder}
        required={required}
        disabled={disabled}
        {...props}
      />
    );
  };

  return (
    <div className="mb-3">
      {label && (
        <label htmlFor={inputId} className="form-label">
          {label}
          {required && <span className="text-danger ms-1">*</span>}
        </label>
      )}
      {renderInput()}
      {error && (
        <div className="invalid-feedback">
          {error}
        </div>
      )}
      {helpText && !error && (
        <div className="form-text">
          {helpText}
        </div>
      )}
    </div>
  );
};

Input.propTypes = {
  label: PropTypes.string,
  type: PropTypes.oneOf(['text', 'email', 'password', 'number', 'tel', 'url', 'search', 'textarea']),
  id: PropTypes.string,
  name: PropTypes.string.isRequired,
  value: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
  onChange: PropTypes.func.isRequired,
  placeholder: PropTypes.string,
  required: PropTypes.bool,
  disabled: PropTypes.bool,
  error: PropTypes.string,
  helpText: PropTypes.string,
  size: PropTypes.oneOf(['sm', 'md', 'lg']),
  className: PropTypes.string,
  rows: PropTypes.number
};

export default Input;
