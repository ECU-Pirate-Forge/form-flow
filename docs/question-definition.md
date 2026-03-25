# **QuestionDefinition Model Documentation**

This document describes the backend model used to represent a single question within the FormFlow system. The model mirrors the shared JSON schema used by the UI and API, ensuring consistent structure, validation, and behavior across the entire application.

---

## **Purpose of the Model**

The `QuestionDefinition` model provides a strongly typed representation of a form question. It enables:

* Consistent serialization/deserialization between backend and UI
* A unified structure for validation logic
* Predictable rendering behavior in the UI
* Safe storage in LiteDB
* Clear enforcement of required fields defined in the JSON schema

This model is the foundation for question creation, editing, storage, and runtime evaluation.

---

## **Model Overview**

### **QuestionDefinition**

Represents a single question in a dynamic form.

| Property              | Type             | Required           | Description                                           |
| --------------------- | ---------------- | ------------------ | ----------------------------------------------------- |
| `Id`                | `Guid`         | Yes                | Unique identifier for the question.                   |
| `Key`               | `string`       | Yes                | Unique key used to reference the question.            |
| `Label`             | `string`       | Yes                | Human‑readable label shown to the user.              |
| `Type`              | `string`       | Yes                | Input type (e.g., text, number, dropdown).            |
| `Required`          | `bool`         | No (default: true) | Whether the question must be answered.                |
| `Placeholder`       | `string?`      | No                 | Optional placeholder text.                            |
| `DefaultValue`      | `string?`      | No                 | Optional default value.                               |
| `HelpText`          | `string?`      | No                 | Optional guidance shown beneath the question.         |
| `Options`           | `List<Option>` | No                 | Selectable options for dropdown/radio/checkbox types. |
| `VisibleIf`         | `VisibleIf?`   | No                 | Conditional visibility rule.                          |
| `ValidationConfigs` | `string?`      | No                 | JSON‑serialized array of validation rule objects.    |

---

### **Option**

Represents a selectable option for dropdown, radio, checkbox, or multiselect questions.

| Property  | Type       | Required | Description                        |
| --------- | ---------- | -------- | ---------------------------------- |
| `Value` | `string` | Yes      | The submitted value when selected. |
| `Label` | `string` | Yes      | The text shown to the user.        |

---

### **VisibleIf**

Defines a conditional visibility rule for a question.

| Property        | Type       | Required | Description                                  |
| --------------- | ---------- | -------- | -------------------------------------------- |
| `Key`         | `string` | Yes      | The key of the controlling question.         |
| `ShouldEqual` | `bool`   | Yes      | The expected value that triggers visibility. |

This allows questions to appear only when another question’s answer matches a specific boolean value.

---

## **JSON Schema Alignment**

The model is designed to match the JSON schema exactly:

* Required fields (`id`, `key`, `label`, `type`) are marked with `required` in C#.
* Optional fields are nullable.
* `validationConfigs` remains a  **string** , as defined in the schema.
* `VisibleIf` and `Option` are represented as separate classes.
* JSON property names are preserved using `JsonPropertyName` where needed.

This ensures seamless communication between backend and frontend.

### **ValidationConfigs Internal Structure**

Although `validationConfigs` is stored and transmitted as a  **string** , the contents of that string must follow a well‑defined structure. Internally, the value must be a JSON array of validation rule objects with the following schema:

[
  {
    "validationType": "string",
    "minLength": "number (optional)",
    "maxLength": "number (optional)",
    "minValue": "number (optional)",
    "maxValue": "number (optional)",
    "pattern": "string (optional)",
    "values": "array of strings (optional)",
    "message": "string (optional)"
  }
]

This describes the **expected shape** of the validation rules without including real example data.

The backend validation engine deserializes this string and applies the rules accordingly.

---

## **Serialization & Deserialization**

The model supports round‑trip JSON serialization without data loss:

* All fields map directly to schema properties
* Optional fields are omitted when null
* Lists are initialized to avoid null references
* Nested objects serialize cleanly
* `ValidationConfigs` remains a raw JSON string for flexibility

Unit tests verify that:

* Valid JSON deserializes successfully
* Invalid JSON fails gracefully
* Required fields are enforced
* Visibility and options structures deserialize correctly

---

## **Usage in the System**

The `QuestionDefinition` model is used by:

* The API (for sending/receiving question definitions)
* The validation engine (to evaluate rules and required fields)
* The UI (to render dynamic forms)
* The database layer (LiteDB storage)

It serves as the single source of truth for question structure across the entire application.
