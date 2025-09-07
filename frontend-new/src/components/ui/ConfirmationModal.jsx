import React, { useEffect } from 'react';
import { createPortal } from 'react-dom';
import Modal from './Modal.jsx';
import Button from './Button.jsx';
import { useConfirmationContext } from '../../contexts/ConfirmationContext.jsx';

const ConfirmationModal = () => {
  const { confirmation, handleConfirm, handleCancel } = useConfirmationContext();

  // Handle Escape key
  useEffect(() => {
    const handleKeyDown = (event) => {
      if (event.key === 'Escape' && confirmation.isOpen) {
        handleCancel();
      }
      if (event.key === 'Enter' && confirmation.isOpen) {
        handleConfirm();
      }
    };

    if (confirmation.isOpen) {
      document.addEventListener('keydown', handleKeyDown);
      // Prevent body scroll when modal is open and add blur effect
      document.body.style.overflow = 'hidden';
      document.body.classList.add('modal-open-blur');
    }

    return () => {
      document.removeEventListener('keydown', handleKeyDown);
      document.body.style.overflow = 'unset';
      document.body.classList.remove('modal-open-blur');
    };
  }, [confirmation.isOpen, handleCancel, handleConfirm]);

  const getVariantIcon = () => {
    const icons = {
      danger: 'fas fa-exclamation-triangle text-danger',
      warning: 'fas fa-exclamation-circle text-warning',
      info: 'fas fa-info-circle text-info',
      success: 'fas fa-check-circle text-success'
    };
    return icons[confirmation.variant] || icons.danger;
  };

  const getConfirmButtonVariant = () => {
    const variants = {
      danger: 'danger',
      warning: 'warning', 
      info: 'primary',
      success: 'success'
    };
    return variants[confirmation.variant] || 'danger';
  };

  return createPortal(
    <Modal
      show={confirmation.isOpen}
      onHide={handleCancel}
      title={confirmation.title}
      centered={true}
      size="sm"
    >
      <Modal.Body>
        <div className="text-center mb-3">
          <i className={`${getVariantIcon()} fa-3x mb-3`}></i>
          <p className="mb-0">{confirmation.message}</p>
        </div>
      </Modal.Body>
      
      <Modal.Footer className="justify-content-center">
        <Button
          variant="outline-secondary"
          onClick={handleCancel}
          className="me-2"
        >
          {confirmation.cancelText}
        </Button>
        <Button
          variant={getConfirmButtonVariant()}
          onClick={handleConfirm}
          autoFocus
        >
          {confirmation.confirmText}
        </Button>
      </Modal.Footer>
    </Modal>,
    document.body
  );
};

export default ConfirmationModal;