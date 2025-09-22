import { useState, useCallback } from 'react';

/**
 * Base form hook that handles common form functionality:
 * - Form state management with initialData merging
 * - Form field change handling with error clearing
 * - Form validation framework
 * - Form submission handling
 */
export const useBaseForm = (initialData = {}, validationRules = {}) => {
  const [formData, setFormData] = useState(initialData);
  const [errors, setErrors] = useState({});
  const [isSubmitting, setIsSubmitting] = useState(false);

  /**
   * Handle form field changes with automatic error clearing
   */
  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    const fieldValue = type === 'checkbox' ? checked : value;
    
    setFormData(prev => ({
      ...prev,
      [name]: fieldValue
    }));

    // Clear error when user starts typing/interacting
    if (errors[name]) {
      setErrors(prev => ({
        ...prev,
        [name]: ''
      }));
    }
  };

  /**
   * Set form data programmatically
   */
  const setField = (name, value) => {
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  /**
   * Set multiple fields at once
   */
  const setFields = (fields) => {
    setFormData(prev => ({
      ...prev,
      ...fields
    }));
  };

  /**
   * Set field error
   */
  const setFieldError = (name, error) => {
    setErrors(prev => ({
      ...prev,
      [name]: error
    }));
  };

  /**
   * Clear specific field error
   */
  const clearFieldError = (name) => {
    setErrors(prev => {
      const newErrors = { ...prev };
      delete newErrors[name];
      return newErrors;
    });
  };

  /**
   * Clear all errors
   */
  const clearErrors = () => {
    setErrors({});
  };

  /**
   * Validate form using provided validation rules
   */
  const validate = () => {
    const newErrors = {};
    
    Object.keys(validationRules).forEach(fieldName => {
      const rules = validationRules[fieldName];
      const fieldValue = formData[fieldName];
      
      // Run each validation rule for this field
      rules.forEach(rule => {
        if (!newErrors[fieldName]) { // Only set first error
          const error = rule(fieldValue, formData);
          if (error) {
            newErrors[fieldName] = error;
          }
        }
      });
    });

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  /**
   * Handle form submission with validation
   */
  const handleSubmit = (onSubmit) => {
    return async (e) => {
      e.preventDefault();
      
      if (!validate()) {
        return;
      }

      setIsSubmitting(true);
      try {
        await onSubmit(formData);
      } catch (error) {
        throw error;
      } finally {
        setIsSubmitting(false);
      }
    };
  };

  /**
   * Reset form to initial state
   */
  const resetForm = () => {
    setFormData(initialData);
    setErrors({});
    setIsSubmitting(false);
  };

  /**
   * Reset form to new initial data
   */
  const resetToData = useCallback((newData) => {
    setFormData(newData);
    setErrors({});
    setIsSubmitting(false);
  }, []);

  return {
    // State
    formData,
    errors,
    isSubmitting,
    
    // Handlers
    handleChange,
    handleSubmit,
    
    // Utilities
    setField,
    setFields,
    setFieldError,
    clearFieldError,
    clearErrors,
    validate,
    resetForm,
    resetToData,
    
    // Computed
    hasErrors: Object.keys(errors).length > 0,
    isValid: Object.keys(errors).length === 0
  };
};