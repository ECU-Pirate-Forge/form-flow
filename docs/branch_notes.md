# Branch Notes and Schema Documentation

This document is intended to capture what has been worked on in the current feature branch and to record the shape of the data schema used by the application. It can be referenced when writing the pull request description or when explaining changes to teammates.

---

## 1. Summary of Work

- Implemented question insertion service on the backend.
- Added client-side validation logic and updated sample question.
- Updated data model for dynamic questions and schema definition.
- Adjusted database library and migrations to support new fields.
- Front‑end wire‑up: posting to the `/api/questions` endpoint; display error messages.
_ removed order property as order is not needed in reusuable question objects
- added conditional logic to options field


## 2. Data Schema (dynamic question)

The application uses a JSON Schema found at `backend/data/schemas/intialSchema.json`.  Below is an annotated version of the properties that you should be aware of when modifying or inserting questions.

```json
{
  "id": { "type": "string", "format": "uuid", "description": "Unique identifier for the question." },
  "key": { "type": "string", "description": "Machine-friendly field name." },
  "label": { "type": "string", "description": "Text shown to the user." },
  "type": { "type": "string", "enum": ["text", "number", "yes_no", "dropdown"], "description": "Control type." },
  "required": { "type": "boolean", "description": "Whether the question must be answered." },
  "placeholder": { "type": "string", "description": "Optional hint text." },
  "defaultValue": { "type": [any], "description": "Optional prefilled value." },
  "options": {
    "type": "array",
    "items": {
      "type": "object",
      "properties": {
        "value": { "type": ["string", "number"] },
        "label": { "type": "string" }
      },
      "required": ["value", "label"]
    },
    "description": "Array of choices (for dropdown)."
  },
  "validation": { "type": "object", "description": "Object containing validation rules." },
  "helpText": { "type": "string", "description": "Optional guidance for the user." }
}
```

### Required properties

- `id` – the question UUID.
- `key` – unique field key used in data submissions.
- `label` – the user-facing prompt.
- `type` – one of `text`, `number`, `yes_no`, or `dropdown`.

### Optional/conditional properties

- `required` – when `true`, the UI will mark the field required and the backend will reject missing values.
- `placeholder`, `defaultValue`, `helpText` – various UX helpers.
- `options` – used only for dropdown,radio,checkbox, multiselect types; must be an array of `{ value, label }` objects.
- `validation` – a free-form object; validation rules are implemented in `backend/data/schemas/validation/questionValidation.js` and the corresponding front‑end logic.  Add new rules here when expanding validation capabilities.


## 3. Practical Notes

- If you add or change a schema field, update both the JSON schema and any model classes (`models/questions.cs`) or TypeScript types.
- Run unit tests after changing validation logic (`dotnet test` and `npm test`).
- When pushing your branch, reference this document in your PR description so reviewers have context.


## 4. Writing the Pull Request

Copy a cleaned-up version of section **1. Summary of Work** into the PR body, then expand with details from sections 2 and 3 as needed.  Be sure to highlight:

1. **What** changed (schema, services, UI, etc.)
2. **Why** it was necessary (bug fix, new feature, refactor)
3. **How** you implemented it (high-level approach)
4. **Testing instructions** for reviewers (e.g., run migrations, hit endpoints, check validation)

---

Keep this file updated as the branch evolves so the final PR description can be assembled quickly.