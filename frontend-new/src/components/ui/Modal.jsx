import React from 'react';
import PropTypes from 'prop-types';
import { useTranslation } from 'react-i18next';

const Modal = ({ 
  children, 
  show = false, 
  onHide, 
  title,
  size = '',
  centered = false,
  className = '',
  ...props 
}) => {
  const { t } = useTranslation('common');
  if (!show) return null;

  const getSizeClass = () => {
    const sizes = {
      sm: 'modal-sm',
      lg: 'modal-lg',
      xl: 'modal-xl'
    };
    return sizes[size] || '';
  };

  const modalClasses = [
    'modal',
    'fade',
    'show',
    className
  ].filter(Boolean).join(' ');

  const dialogClasses = [
    'modal-dialog',
    getSizeClass(),
    centered ? 'modal-dialog-centered' : ''
  ].filter(Boolean).join(' ');

  return (
    <div 
      className={modalClasses} 
      style={{ display: 'block' }}
      onClick={onHide}
      {...props}
    >
      <div className={dialogClasses}>
        <div className="modal-content" onClick={(e) => e.stopPropagation()}>
          {title && (
            <div className="modal-header">
              <h5 className="modal-title">{title}</h5>
              <button 
                type="button" 
                className="btn-close" 
                onClick={onHide}
                aria-label={t('buttons.close')}
              ></button>
            </div>
          )}
          {children}
        </div>
      </div>
    </div>
  );
};

const ModalBody = ({ children, className = '', ...props }) => {
  return (
    <div className={`modal-body ${className}`} {...props}>
      {children}
    </div>
  );
};

const ModalFooter = ({ children, className = '', ...props }) => {
  return (
    <div className={`modal-footer ${className}`} {...props}>
      {children}
    </div>
  );
};

Modal.Body = ModalBody;
Modal.Footer = ModalFooter;

Modal.propTypes = {
  children: PropTypes.node.isRequired,
  show: PropTypes.bool,
  onHide: PropTypes.func.isRequired,
  title: PropTypes.string,
  size: PropTypes.oneOf(['', 'sm', 'lg', 'xl']),
  centered: PropTypes.bool,
  className: PropTypes.string
};

ModalBody.propTypes = {
  children: PropTypes.node.isRequired,
  className: PropTypes.string
};

ModalFooter.propTypes = {
  children: PropTypes.node.isRequired,
  className: PropTypes.string
};

export default Modal;
