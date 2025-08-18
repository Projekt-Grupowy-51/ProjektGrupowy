import React from 'react';
import PropTypes from 'prop-types';

const Select = ({
                    label,
                    name,
                    value,
                    onChange,
                    options,
                    required = false,
                    disabled = false,
                    error,
                }) => {
    return (
        <div className="mb-3">
            {label && (
                <label htmlFor={name} className="form-label">
                    {label}
                </label>
            )}
            <select
                id={name}
                name={name}
                value={value}
                onChange={onChange}
                className={`form-select ${error ? 'is-invalid' : ''}`}
                required={required}
                disabled={disabled}
            >
                <option value="">{/* placeholder handled outside */}</option>
                {options.map((opt) => (
                    <option key={opt.value} value={opt.value}>
                        {opt.label}
                    </option>
                ))}
            </select>
            {error && <div className="invalid-feedback">{error}</div>}
        </div>
    );
};

Select.propTypes = {
    label: PropTypes.string,
    name: PropTypes.string.isRequired,
    value: PropTypes.oneOfType([PropTypes.string, PropTypes.number]).isRequired,
    onChange: PropTypes.func.isRequired,
    options: PropTypes.arrayOf(
        PropTypes.shape({
            value: PropTypes.oneOfType([PropTypes.string, PropTypes.number])
                .isRequired,
            label: PropTypes.string.isRequired,
        })
    ).isRequired,
    required: PropTypes.bool,
    disabled: PropTypes.bool,
    error: PropTypes.string,
};

export default Select;
