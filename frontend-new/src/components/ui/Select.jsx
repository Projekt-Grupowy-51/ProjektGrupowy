import React from 'react';
import PropTypes from 'prop-types';

const Select = ({
                    label,
                    name,
                    value,
                    onChange,
                    options = [],
                    required = false,
                    disabled = false,
                    error,
                    size = '',
                    className = '',
                    ...props
                }) => {
    const getSizeClass = () => {
        return size === 'sm' ? 'form-select-sm' : size === 'lg' ? 'form-select-lg' : '';
    };

    const selectElement = (
        <select
            id={name}
            name={name}
            value={value}
            onChange={onChange}
            className={`form-select ${getSizeClass()} ${error ? 'is-invalid' : ''} ${className}`.trim()}
            required={required}
            disabled={disabled}
            {...props}
        >
            {options.map((opt) => (
                <option key={opt.value} value={opt.value}>
                    {opt.label}
                </option>
            ))}
        </select>
    );

    if (!label) {
        return selectElement;
    }

    return (
        <div className="mb-3">
            <label htmlFor={name} className="form-label">
                {label}
            </label>
            {selectElement}
            {error && <div className="invalid-feedback">{error}</div>}
        </div>
    );
};

Select.propTypes = {
    label: PropTypes.string,
    name: PropTypes.string,
    value: PropTypes.oneOfType([PropTypes.string, PropTypes.number]).isRequired,
    onChange: PropTypes.func.isRequired,
    options: PropTypes.arrayOf(
        PropTypes.shape({
            value: PropTypes.oneOfType([PropTypes.string, PropTypes.number])
                .isRequired,
            label: PropTypes.string.isRequired,
        })
    ),
    required: PropTypes.bool,
    disabled: PropTypes.bool,
    error: PropTypes.string,
    size: PropTypes.oneOf(['', 'sm', 'lg']),
    className: PropTypes.string,
};

export default Select;
