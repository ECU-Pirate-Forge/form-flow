const Ajv = require('ajv');
const ajv = new Ajv(); // Create an instance of the validator

// Define your schema 
const schema = {
  
  "$schema": "http://json-schema.org/draft-07/schema#",
  "type": "object",
  "title": "Dynamic Question",
  "description": "A basic question object for UI rendering and validation.",
  "properties": {
    "id": {
      "type": "string",
      "format": "uuid",
      "description": "Unique identifier for the question."
    },
    "key": {
      "type": "string",
      "description": "Machine-friendly field name."
    },
    "label": {
      "type": "string",
      "description": "Text shown to the user."
    },
    "type": {
      "type": "string",
      "enum": ["text", "number", "yes_no", "dropdown"],
      "description": "Control type."
    },
    "order": {
      "type": "integer",
      "description": "Display order."
    },
    "required": {
      "type": "boolean",
      "description": "Whether the question must be answered."
    },
    "placeholder": {
      "type": "string",
      "description": "Optional hint text."
    },
    "defaultValue": {
      "type": ["string", "number"],
      "description": "Optional prefilled value."
    },
    "options": {
      "type": "array",
      "items": {
        "type": "object",
        "properties": {
          "value": {
            "type": ["string", "number"]
          },
          "label": {
            "type": "string"
          }
        },
        "required": ["value", "label"]
      },
      "description": "Array of choices (for dropdown)."
    },
    "validation": {
      "type": "object",
      "description": "Object containing validation rules.",
      "properties": {}
    },
    "helpText": {
      "type": "string",
      "description": "Optional guidance for the user."
    }
  },
  "required": ["id", "key", "label", "type"]

};

// sample question object 
const question = {
  id: '1',
  questionText: 'What is your first name?',
  type: 'text',
  // can add more properties as needed
};

// Create a reference to the schema
const validate = ajv.compile(schema);

// Validate the question object
const valid = validate(question);

if (valid) {
  console.log('Question is valid!');
} else {
  console.log('Question is invalid:', validate.errors); // Print validation errors
}
