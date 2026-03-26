# Backend and Schema Documentation

This document is intended to describe the projects backend.This includes: recording the shape of data schemas used by the application,

---

## 1. Summary of Work



## 2. Survey Schema 

The application uses a JSON Schema found at `FormFlow.Backend/Schemas/survey-definition.schema.json`. 

### Purpose and roles

The purpose of this schema is to serve as source for the structure of all survey data within the system.Its primary roles are:
 
- Data Integrity: Ensures that every survey contains essential metadata (surveyId, Title, Questions, Description, Created Date) before it hits the database.
- Business Logic Enforcement: Guarantees that a survey is never "empty" by enforcing a minimum of one question.
- Standardization: Provides a consistent JSON format for the frontend, backend, and third-party integrations to follow, reducing "undefined" errors during data parsing.

### Schema Relationships

This schema follows a Modular Design pattern. Rather than defining what a "Question" looks like inside the Survey schema, it uses a Relative Reference ($ref).

```json
 "questions": {
            "type": "array",
            "description": "List of questions in the survey.",
            "items": {
                "$ref": "question-definition.schema.json"
            },
            "minItems": 1
            
        },
```
The Survey schema is the "Parent," and the Question schema is the "Child. Any changes made to the question types (e.g., adding a "Free Response" option) only need to be updated in question-definition.schema.json. The Survey schema will automatically inherit these updates.When a Survey is validated, the engine "jumps" into the question schema to validate every object inside the questions array. If even one question fails the sub-schema validation, the entire Survey is marked as invalid.
\* Both files must remain in the same folder for $ref to work

### Valid Survey Example
Here is a valid example  of a survey json object:
```json
  "surveyId": "SRV-101",
  "title": "Customer Feedback 2026",
  "description": "A survey to understand user satisfaction.",
  "version": 1,
  "createdAt": "2026-03-26T14:30:00Z",
  "questions": [
    {
    "id": "3f4c2c2e-9f7b-4b7a-8c3e-2a4b9d9f1a11",
    "key": "full_name",
    "label": "What is your full name?",
    "type": "text",
    "placeholder": "Enter your name",
    "defaultValue": "",
    "validationRules": [
      { "validationType": "MinLength", "minLength": 1 },
      { "validationType": "MaxLength", "maxLength": 100}
    ]
  }
  ]
```
### Invalid Survey Example
Here is a invalid example of a survey json object:
```json
  "surveyId": "SRV-1012",
  "title": "Made UP bad example",
  "description": "invalid survey",
  "version": 1,
  "createdAt": "today",
  "questions": [
  ]
```
noticce the lack of question objects and improper formatting of when the survey was created.

---