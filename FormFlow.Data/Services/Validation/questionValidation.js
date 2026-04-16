const Ajv = require('ajv');
const addFormats = require('ajv-formats');
const ajv = new Ajv({ allErrors: true });
addFormats(ajv);

// Constants derived from schema
const QUESTION_TYPES = ["text", "number", "yes_no", "dropdown", "radio", "checkbox", "multiselect"];
const CHOICE_TYPES = ["dropdown", "radio", "checkbox", "multiselect"];
const VALIDATION_TYPES = ["MinValue", "MaxValue", "MinLength", "MaxLength", "Range"];
const TEXT_TYPES = ["text"];
const NUMBER_TYPES = ["number"];
const VALIDATION_APPLICABLE_TYPES = TEXT_TYPES.concat(NUMBER_TYPES);

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
      "format": "uuid",
      "description": "Unique identifier for the question (UUID format)."
    },
    "key": {
      "type": "string",
      "minLength": 1,
      "description": "Machine-friendly key for referencing this question."
    },
    "label": {
      "type": "string",
      "minLength": 1,
      "description": "User-facing label displayed for the question."
    },
    "type": {
      "type": "string",
      "enum": QUESTION_TYPES,
      "description": "Type of input control."
    },
    "required": {
      "type": "boolean",
      "description": "Whether this question must be answered."
    },
    "placeholder": {
      "type": ["string", "null"],
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
        "shouldEqual": {
          "type": "boolean",
          "description": "The value that must be matched for this question to be visible."
        }
      },
      "required": ["key", "shouldEqual"],
      "additionalProperties": false,
      "description": "Conditional visibility rule."
    },
    "validationConfigs": {
      "type": ["array", "null"],
      "items": {
        "type": "object",
        "properties": {
          "validationType": {
            "type": "string",
            "enum": VALIDATION_TYPES,
            "description": "Type of validation rule."
          },
          "minValue": {
            "type": "integer",
            "description": "Minimum value for number fields."
          },
          "maxValue": {
            "type": "integer",
            "description": "Maximum value for number fields."
          },
          "minLength": {
            "type": "integer",
            "description": "Minimum length for text fields."
          },
          "maxLength": {
            "type": "integer",
            "description": "Maximum length for text fields."
          },
          "message": {
            "type": "string",
            "description": "Custom error message for the validation rule."
          }
        },
        "required": ["validationType"],
        "additionalProperties": false
      },
      "description": "Array of validation rules applied to user input."
    },
    "helpText": {
      "type": ["string", "null"],
      "description": "Optional guidance or help text for the user."
    }
  },
  "required": ["id", "key", "label", "type"],
  "additionalProperties": false,
  "if": {
    "properties": {
      "type": {
        "enum": CHOICE_TYPES
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
  if (CHOICE_TYPES.includes(question.type) && (!question.options || question.options.length === 0)) {
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

  // Validate validationConfigs if present
  if (question.validationConfigs && Array.isArray(question.validationConfigs)) {
    const minLengthRules = question.validationConfigs.filter(v => v.validationType === 'MinLength');
    const maxLengthRules = question.validationConfigs.filter(v => v.validationType === 'MaxLength');
    const minValueRules = question.validationConfigs.filter(v => v.validationType === 'MinValue');
    const maxValueRules = question.validationConfigs.filter(v => v.validationType === 'MaxValue');
    const rangeRules = question.validationConfigs.filter(v => v.validationType === 'Range');

    // Check MinLength and MaxLength consistency
    if (minLengthRules.length > 0 && maxLengthRules.length > 0) {
      const minLen = Math.max(...minLengthRules.map(r => r.minLength || 0));
      const maxLen = Math.min(...maxLengthRules.map(r => r.maxLength || Infinity));
      if (minLen > maxLen) {
        errors.push({
          field: '/validationConfigs',
          property: 'min_max_length_order',
          message: 'MinLength cannot be greater than MaxLength'
        });
      }
    }

    // Check MinValue and MaxValue consistency
    if (minValueRules.length > 0 && maxValueRules.length > 0) {
      const minVal = Math.max(...minValueRules.map(r => r.minValue || -Infinity));
      const maxVal = Math.min(...maxValueRules.map(r => r.maxValue || Infinity));
      if (minVal > maxVal) {
        errors.push({
          field: '/validationConfigs',
          property: 'min_max_value_order',
          message: 'MinValue cannot be greater than MaxValue'
        });
      }
    }

    // Check Range consistency
    rangeRules.forEach(rule => {
      if (rule.minValue !== undefined && rule.maxValue !== undefined && rule.minValue > rule.maxValue) {
        errors.push({
          field: '/validationConfigs',
          property: 'range_order',
          message: 'In Range validation, minValue cannot be greater than maxValue'
        });
      }
    });

    // Validate that validation types are appropriate for the question type
    question.validationConfigs.forEach((config, index) => {
      if (TEXT_TYPES.includes(question.type) && !['MinLength', 'MaxLength'].includes(config.validationType)) {
        errors.push({
          field: `/validationConfigs/${index}`,
          property: 'invalid_validation_type',
          message: `Validation type '${config.validationType}' is not applicable for text questions`
        });
      }
      if (NUMBER_TYPES.includes(question.type) && !['MinValue', 'MaxValue', 'Range'].includes(config.validationType)) {
        errors.push({
          field: `/validationConfigs/${index}`,
          property: 'invalid_validation_type',
          message: `Validation type '${config.validationType}' is not applicable for number questions`
        });
      }
      if (!VALIDATION_APPLICABLE_TYPES.includes(question.type)) {
        errors.push({
          field: `/validationConfigs/${index}`,
          property: 'validation_not_allowed',
          message: `Validation rules are not applicable for question type '${question.type}'`
        });
      }
    });
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
      if (path === '/key') {
        return 'Question key is required';
      }
      if (path === '/label') {
        return 'Question label is required';
      }
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