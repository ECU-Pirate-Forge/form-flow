/**
 * Question Validation Module
 * Validates question objects against the Question schema
 * Used by backend to enforce data integrity before database insertion
 */

const Ajv = require('ajv');
const ajv = new Ajv({ allErrors: true });

/**
 * JSON Schema for Question objects
 * Matches the C# Question model and intended question structure
 */
const schema = {
  "$schema": "http://json-schema.org/draft-07/schema#",
  "title": "Question Schema",
  "description": "Schema for defining questions in a form.",
  "type": "object",
  "properties": {
    "id": {
      "type": "string",
      "description": "Unique identifier for the question (UUID format)."
    },
    "key": {
      "type": "string",
      "description": "Machine-friendly key for referencing this question."
    },
    "label": {
      "type": "string",
      "description": "User-facing label displayed for the question."
    },
    "type": {
      "type": "string",
      "enum": ["text", "number", "yes_no", "dropdown", "radio", "checkbox", "multiselect"],
      "description": "Type of input control."
    },
    "required": {
      "type": "boolean",
      "description": "Whether this question must be answered."
    },
    "placeholder": {
      "type": "string",
      "description": "Placeholder text shown in the input field."
    },
    "defaultValue": {
      "description": "Default value for the question (string or number)."
    },
    "options": {
      "type": "array",
      "items": {
        "type": "object",
        "properties": {
          "value": {
            "type": "string",
            "description": "The stored value for this option."
          },
          "label": {
            "type": "string",
            "description": "The displayed label for this option."
          }
        },
        "required": ["value", "label"],
        "additionalProperties": false
      },
      "description": "Array of options for dropdown/radio/checkbox/multiselect types."
    },
    "visibleIf": {
      "type": ["object", "null"],
      "properties": {
        "key": {
          "type": "string",
          "description": "Key of the question that controls visibility."
        },
        "equals": {
          "type": "boolean",
          "description": "The value that must be matched for this question to be visible."
        }
      },
      "required": ["key", "equals"],
      "additionalProperties": false,
      "description": "Conditional visibility rule."
    },
    "validation": {
      "type": ["object", "null"],
      "properties": {
        "min_length": {
          "type": "integer",
          "minimum": 0,
          "description": "Minimum length for text fields."
        },
        "max_length": {
          "type": "integer",
          "minimum": 0,
          "description": "Maximum length for text fields."
        }
      },
      "additionalProperties": false,
      "description": "Validation rules applied to user input."
    },
    "helpText": {
      "type": "string",
      "description": "Optional guidance or help text for the user."
    }
  },
  "required": ["id", "key", "label", "type"],
  "additionalProperties": false,
  "if": {
    "properties": {
      "type": {
        "enum": ["dropdown", "radio", "checkbox", "multiselect"]
      }
    },
    "required": ["type"]
  },
  "then": {
    "required": ["options"]
  }
};

// Compile the schema for efficient validation
const validate = ajv.compile(schema);

/**
 * Validates a question object against the schema
 * @param {Object} question - The question object to validate
 * @returns {Object} Result object with valid flag and errors array
 */
function validateQuestion(question) {
  if (!question || typeof question !== 'object') {
    return {
      valid: false,
      errors: [
        {
          field: 'root',
          message: 'Question must be a non-null object'
        }
      ]
    };
  }

  const isValid = validate(question);

  if (!isValid) {
    // Transform AJV errors into a more user-friendly format
    const errors = validate.errors.map(error => ({
      field: error.instancePath || 'root',
      property: error.keyword,
      message: formatErrorMessage(error),
      details: error
    }));

    return {
      valid: false,
      errors: errors
    };
  }

  // Additional semantic validation
  const semanticErrors = performSemanticValidation(question);
  if (semanticErrors.length > 0) {
    return {
      valid: false,
      errors: semanticErrors
    };
  }

  return {
    valid: true,
    errors: []
  };
}

/**
 * Performs additional semantic validation beyond JSON schema
 * @param {Object} question - The question to validate
 * @returns {Array} Array of semantic validation errors
 */
function performSemanticValidation(question) {
  const errors = [];

  // Validate that options are present for choice-based types
  const choiceTypes = ['dropdown', 'radio', 'checkbox', 'multiselect'];
  if (choiceTypes.includes(question.type) && (!question.options || question.options.length === 0)) {
    errors.push({
      field: '/options',
      property: 'options_required',
      message: `Question type '${question.type}' requires at least one option`
    });
  }

  // Validate that text/number types don't have options
  if (['text', 'number'].includes(question.type) && question.options && question.options.length > 0) {
    errors.push({
      field: '/options',
      property: 'options_not_allowed',
      message: `Question type '${question.type}' should not have options`
    });
  }

  // Validate validation rules if present
  if (question.validation) {
    if (question.validation.min_length !== undefined && question.validation.max_length !== undefined) {
      if (question.validation.min_length > question.validation.max_length) {
        errors.push({
          field: '/validation',
          property: 'min_max_order',
          message: 'min_length cannot be greater than max_length'
        });
      }
    }
  }

  // Validate key format (should be snake_case or alphanumeric)
  if (question.key && !/^[a-z0-9_]+$/.test(question.key)) {
    errors.push({
      field: '/key',
      property: 'key_format',
      message: 'Key must contain only lowercase letters, numbers, and underscores'
    });
  }

  return errors;
}

/**
 * Formats AJV validation errors into readable messages
 * @param {Object} error - AJV error object
 * @returns {string} Formatted error message
 */
function formatErrorMessage(error) {
  const path = error.instancePath || 'root';
  
  switch (error.keyword) {
    case 'type':
      return `${path} must be of type ${error.params.type}`;
    case 'required':
      return `Missing required field: ${error.params.missingProperty}`;
    case 'enum':
      return `${path} must be one of: ${error.params.allowedValues.join(', ')}`;
    case 'additionalProperties':
      return `${path} contains unexpected property: ${error.params.additionalProperty}`;
    case 'minLength':
      return `${path} must be at least ${error.params.limit} characters long`;
    case 'maxLength':
      return `${path} must be at most ${error.params.limit} characters long`;
    case 'minimum':
      return `${path} must be at least ${error.params.limit}`;
    case 'format':
      return `${path} must be a valid ${error.params.format}`;
    default:
      return error.message || 'Validation failed';
  }
}

// Export functions for use by backend
module.exports = {
  validateQuestion,
  schema,
  ajv
};
