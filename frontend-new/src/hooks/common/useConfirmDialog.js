import { useState } from 'react';

export const useConfirmDialog = (defaultMessage = 'Are you sure?') => {
  const [isOpen, setIsOpen] = useState(false);
  const [message, setMessage] = useState(defaultMessage);
  const [onConfirmCallback, setOnConfirmCallback] = useState(null);

  const showConfirm = async (config) => {
    if (typeof config === 'string') {
      const confirmMessage = config;
      const onConfirm = arguments[1];
      setMessage(confirmMessage || defaultMessage);
      setOnConfirmCallback(() => onConfirm);
      setIsOpen(true);
      return;
    }

    const { title, message: confirmMessage, confirmText, cancelText, variant } = config;
    const displayMessage = `${title ? title + '\n' : ''}${confirmMessage || defaultMessage}`;
    return window.confirm(displayMessage);
  };

  const handleConfirm = () => {
    if (onConfirmCallback) {
      onConfirmCallback();
    }
    setIsOpen(false);
    setOnConfirmCallback(null);
  };

  const handleCancel = () => {
    setIsOpen(false);
    setOnConfirmCallback(null);
  };

  const confirmWithPrompt = async (confirmMessage, callback) => {
    const result = window.confirm(confirmMessage || defaultMessage);
    if (result && callback) {
      return await callback();
    }
    return result;
  };

  return {
    isOpen,
    message,
    showConfirm,
    handleConfirm,
    handleCancel,
    confirmWithPrompt
  };
};