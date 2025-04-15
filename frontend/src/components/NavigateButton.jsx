import React from 'react';
import { useNavigate } from 'react-router-dom';

const NavigateButton = ({ path, actionType = 'Back', className = '', value, style = {} }) => {
    const navigate = useNavigate();

    const handleClick = () => {
        if (path) {
            navigate(path);
        } else {
            navigate(-1);
        }
    };

    // Set up button text and class based on action type
    let buttonText = value || actionType;
    let buttonClass = 'btn ';
    let iconClass = '';

    switch (actionType) {
        case 'Add':
            buttonClass += 'btn-primary';
            iconClass = 'fa-plus-circle';
            break;
        case 'Edit':
            buttonClass += 'btn-warning';
            iconClass = 'fa-edit';
            break;
        case 'Details':
            buttonClass += 'btn-info';
            iconClass = 'fa-info-circle';
            break;
        case 'Back':
        default:
            buttonClass += 'btn-secondary';
            iconClass = 'fa-arrow-left';
            break;
    }

    buttonClass += ` ${className}`;

    return (
        <button 
            type="button"
            className={buttonClass}
            onClick={handleClick}
            style={style}
        >
            <i className={`fas ${iconClass} me-2`}></i>
            {buttonText}
        </button>
    );
};

export default NavigateButton;
