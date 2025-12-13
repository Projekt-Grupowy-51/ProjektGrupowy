import { useConfirmationContext } from '../contexts/ConfirmationContext.jsx';

/**
 * Hook for showing confirmation dialogs
 * @returns {Object} Object containing showConfirmation function
 */
export const useConfirmation = () => {
  const { showConfirmation } = useConfirmationContext();

  /**
   * Show confirmation dialog
   * @param {Object} options - Configuration options
   * @param {string} options.title - Modal title
   * @param {string} options.message - Confirmation message
   * @param {string} options.variant - Visual variant (danger, warning, info, success)
   * @param {string} options.confirmText - Confirm button text
   * @param {string} options.cancelText - Cancel button text
   * @param {function} options.onConfirm - Function to execute on confirm
   * @param {function} options.onCancel - Function to execute on cancel
   */
  const confirm = (options) => {
    if (typeof options === 'string') {
      // Simple usage: confirm('Are you sure?')
      return showConfirmation({
        message: options
      });
    }

    return showConfirmation(options);
  };

  /**
   * Show delete confirmation dialog with predefined settings
   * @param {Object} options - Configuration options
   * @param {string} options.itemName - Name of item being deleted
   * @param {function} options.onConfirm - Function to execute on confirm
   */
  const confirmDelete = (options) => {
    const itemName = options.itemName || 'ten element';
    
    return showConfirmation({
      title: 'Potwierdź usunięcie',
      message: `Czy na pewno chcesz usunąć ${itemName}? Ta operacja jest nieodwracalna.`,
      variant: 'danger',
      confirmText: 'Usuń',
      cancelText: 'Anuluj',
      onConfirm: options.onConfirm,
      onCancel: options.onCancel
    });
  };

  /**
   * Show warning confirmation dialog
   * @param {Object} options - Configuration options
   */
  const confirmWarning = (options) => {
    return showConfirmation({
      variant: 'warning',
      confirmText: 'Kontynuuj',
      ...options
    });
  };

  return {
    confirm,
    confirmDelete,
    confirmWarning,
    showConfirmation
  };
};