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
  checked,
  multiple = false,
  accept,
  options = [],
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
    // Textarea
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

    // Checkbox
    if (type === 'checkbox') {
      return (
        <div className="form-check">
          <input
            type="checkbox"
            id={inputId}
            name={name}
            className="form-check-input"
            checked={checked}
            onChange={onChange}
            required={required}
            disabled={disabled}
            {...props}
          />
          {label && (
            <label className="form-check-label" htmlFor={inputId}>
              {label}
              {required && <span className="text-danger ms-1">*</span>}
            </label>
          )}
        </div>
      );
    }

    // Select
    if (type === 'select') {
      return (
        <select
          id={inputId}
          name={name}
          className={getInputClasses().replace('form-control', 'form-select')}
          value={value}
          onChange={onChange}
          required={required}
          disabled={disabled}
          {...props}
        >
          <option value="">Please select...</option>
          {options.map((option) => (
            <option key={option.value} value={option.value}>
              {option.label}
            </option>
          ))}
        </select>
      );
    }

    // File input
    if (type === 'file') {
      return (
        <input
          type="file"
          id={inputId}
          name={name}
          className={getInputClasses()}
          onChange={onChange}
          required={required}
          disabled={disabled}
          multiple={multiple}
          accept={accept}
          {...props}
        />
      );
    }

    // Color input
    if (type === 'color') {
      return (
        <input
          type="color"
          id={inputId}
          name={name}
          className={[getInputClasses(), 'form-control-color'].join(' ')}
          value={value}
          onChange={onChange}
          required={required}
          disabled={disabled}
          {...props}
        />
      );
    }

    // Standard input types
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
      {/* For checkbox, label is rendered inside renderInput() */}
      {label && type !== 'checkbox' && (
        <label htmlFor={inputId} className="form-label">
          {label}
          {required && <span className="text-danger ms-1">*</span>}
        </label>
      )}
      {renderInput()}
      {error && (
        <div className="invalid-feedback d-block">
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
  type: PropTypes.oneOf(['text', 'email', 'password', 'number', 'tel', 'url', 'search', 'textarea', 'checkbox', 'color', 'file', 'select', 'hidden']),
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
  rows: PropTypes.number,
  checked: PropTypes.bool, // for checkbox
  multiple: PropTypes.bool, // for file input
  accept: PropTypes.string, // for file input
  options: PropTypes.arrayOf(PropTypes.shape({ // for select
    value: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
    label: PropTypes.string
  }))
};

export default Input;
