/**
 * Common form validation utility functions
 * These can be used with the useBaseForm hook or standalone
 */

/**
 * Required field validation
 */
export const required = (message = 'This field is required') => {
  return (value) => {
    if (value === null || value === undefined || value === '' || 
        (typeof value === 'string' && !value.trim())) {
      return message;
    }
    return null;
  };
};

/**
 * Minimum length validation
 */
export const minLength = (min, message) => {
  return (value) => {
    if (value && value.length < min) {
      return message || `Must be at least ${min} characters long`;
    }
    return null;
  };
};

/**
 * Maximum length validation
 */
export const maxLength = (max, message) => {
  return (value) => {
    if (value && value.length > max) {
      return message || `Must be no more than ${max} characters long`;
    }
    return null;
  };
};

/**
 * Email validation
 */
export const email = (message = 'Please enter a valid email address') => {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return (value) => {
    if (value && !emailRegex.test(value)) {
      return message;
    }
    return null;
  };
};

/**
 * Hex color validation
 */
export const hexColor = (message = 'Please enter a valid hex color (e.g. #FF0000)') => {
  const hexRegex = /^#[0-9A-Fa-f]{6}$/;
  return (value) => {
    if (value && !hexRegex.test(value)) {
      return message;
    }
    return null;
  };
};

/**
 * Single character validation (for shortcuts)
 */
export const singleCharacter = (message = 'Must be a single character') => {
  return (value) => {
    if (value && value.length !== 1) {
      return message;
    }
    return null;
  };
};

/**
 * Numeric validation
 */
export const numeric = (message = 'Must be a valid number') => {
  return (value) => {
    if (value && isNaN(Number(value))) {
      return message;
    }
    return null;
  };
};

/**
 * Positive number validation
 */
export const positiveNumber = (message = 'Must be a positive number') => {
  return (value) => {
    if (value && (isNaN(Number(value)) || Number(value) <= 0)) {
      return message;
    }
    return null;
  };
};

/**
 * Custom pattern validation
 */
export const pattern = (regex, message = 'Invalid format') => {
  return (value) => {
    if (value && !regex.test(value)) {
      return message;
    }
    return null;
  };
};

/**
 * Custom validation function
 */
export const custom = (validationFn, message = 'Invalid value') => {
  return (value, formData) => {
    if (!validationFn(value, formData)) {
      return message;
    }
    return null;
  };
};

/**
 * Conditional validation - only validate if condition is true
 */
export const when = (condition, validator) => {
  return (value, formData) => {
    if (condition(formData)) {
      return validator(value, formData);
    }
    return null;
  };
};

/**
 * Validation rule builders for common form patterns
 */
export const ValidationRules = {
  /**
   * Standard name field (required, 1-100 chars)
   */
  name: (t) => [
    required(t('validation.name_required')),
    minLength(1, t('validation.name_too_short')),
    maxLength(100, t('validation.name_too_long'))
  ],

  /**
   * Standard description field (required, 1-500 chars)
   */
  description: (t) => [
    required(t('validation.description_required')),
    minLength(1, t('validation.description_too_short')),
    maxLength(500, t('validation.description_too_long'))
  ],

  /**
   * Email field
   */
  email: (t) => [
    required(t('validation.email_required')),
    email(t('validation.email_invalid'))
  ],

  /**
   * Hex color field
   */
  colorHex: (t) => [
    required(t('validation.color_required')),
    hexColor(t('validation.color_invalid'))
  ],

  /**
   * Single character shortcut
   */
  shortcut: (t) => [
    singleCharacter(t('validation.shortcut_invalid'))
  ],

  /**
   * Required selection field
   */
  requiredSelect: (fieldName, t) => [
    required(t(`validation.${fieldName}_required`))
  ]
};