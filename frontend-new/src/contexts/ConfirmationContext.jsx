import React, { createContext, useContext, useState } from 'react';
import PropTypes from 'prop-types';

const ConfirmationContext = createContext();

export const useConfirmationContext = () => {
  const context = useContext(ConfirmationContext);
  if (!context) {
    throw new Error('useConfirmationContext must be used within ConfirmationProvider');
  }
  return context;
};

export const ConfirmationProvider = ({ children }) => {
  const [confirmation, setConfirmation] = useState({
    isOpen: false,
    title: '',
    message: '',
    variant: 'danger',
    confirmText: 'Potwierdź',
    cancelText: 'Anuluj',
    onConfirm: null,
    onCancel: null
  });

  const showConfirmation = (options) => {
    setConfirmation({
      isOpen: true,
      title: options.title || 'Potwierdź akcję',
      message: options.message || 'Czy na pewno chcesz kontynuować?',
      variant: options.variant || 'danger',
      confirmText: options.confirmText || 'Potwierdź',
      cancelText: options.cancelText || 'Anuluj',
      onConfirm: options.onConfirm || null,
      onCancel: options.onCancel || null
    });
  };

  const hideConfirmation = () => {
    setConfirmation(prev => ({
      ...prev,
      isOpen: false
    }));
  };

  const handleConfirm = () => {
    if (confirmation.onConfirm) {
      confirmation.onConfirm();
    }
    hideConfirmation();
  };

  const handleCancel = () => {
    if (confirmation.onCancel) {
      confirmation.onCancel();
    }
    hideConfirmation();
  };

  const value = {
    confirmation,
    showConfirmation,
    hideConfirmation,
    handleConfirm,
    handleCancel
  };

  return (
    <ConfirmationContext.Provider value={value}>
      {children}
    </ConfirmationContext.Provider>
  );
};

ConfirmationProvider.propTypes = {
  children: PropTypes.node.isRequired
};

export default ConfirmationContext;