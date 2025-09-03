import { useAsyncOperation } from './useAsyncOperation.js';
import { useFormNavigation } from './useNavigation.js';

export const useFormSubmission = (submitOperation, successPath, cancelPath) => {
  const { execute, loading, error } = useAsyncOperation();
  const { handleSuccess, handleCancel } = useFormNavigation(successPath, cancelPath);

  const handleSubmit = async (formData) => {
    try {
      await execute(() => submitOperation(formData));
      handleSuccess();
    } catch (err) {
      console.error('Form submission failed:', err);
    }
  };

  return {
    handleSubmit,
    handleCancel,
    loading,
    error
  };
};