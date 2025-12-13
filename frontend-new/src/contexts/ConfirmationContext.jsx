import React, { createContext, useContext, useState } from 'react';
import PropTypes from 'prop-types';
import { useTranslation } from 'react-i18next';

const ConfirmationContext = createContext();

export const useConfirmationContext = () => {
  const context = useContext(ConfirmationContext);
  if (!context) {
    throw new Error('useConfirmationContext must be used within ConfirmationProvider');
  }
  return context;
};

export const ConfirmationProvider = ({ children }) => {
  const { t } = useTranslation('common');
  const [confirmation, setConfirmation] = useState({
    isOpen: false,
    title: '',
    message: '',
    variant: 'danger',
    confirmText: t('deleteConfirmation.confirm'),
    cancelText: t('deleteConfirmation.cancel'),
    onConfirm: null,
    onCancel: null
  });

  const showConfirmation = (options) => {
    setConfirmation({
      isOpen: true,
      title: options.title || t('deleteConfirmation.title'),
      message: options.message || t('confirmationContext.default_message'),
      variant: options.variant || 'danger',
      confirmText: options.confirmText || t('deleteConfirmation.confirm'),
      cancelText: options.cancelText || t('deleteConfirmation.cancel'),
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