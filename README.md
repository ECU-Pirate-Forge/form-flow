
# FormFlow

A dynamic form builder built with .NET 10. Admins define questions with types, validation, and conditional logic. The Blazor frontend renders them as interactive forms, backed by a minimal API and LiteDB for storage.

## Projects

| Project | Description |
|---|---|
| `FormFlow.Backend` | ASP.NET Core minimal API — question endpoints, LiteDB persistence |
| `FormFlow.Blazor` | Blazor Server frontend — question renderer and admin UI |
| `FormFlow.Data` | Shared models, validation, and services |
| `FormFlow.Backend.Tests` | xUnit integration tests for the API |
| `FormFlow.Blazor.Tests` | bUnit component tests for the Blazor UI |
| `FormFlow.Data.Tests` | Unit tests for shared models and validation |

## Quickstart

**Prerequisites:** .NET 10 SDK

```bash
git clone https://github.com/ECU-Pirate-Forge/form-flow.git
cd form-flow
```

**Start the backend API** (seeds LiteDB automatically on first run):
```bash
cd FormFlow.Backend
dotnet run
# Listening on https://localhost:5001
```

**Start the Blazor frontend** (in a second terminal):
```bash
cd FormFlow.Blazor
dotnet run
# Open https://localhost:5002 in your browser
```

No database install required — LiteDB is file-based and creates `formflow.db` on first run.

## Admin UI

Navigate to these routes in the Blazor app:

| Route | Description |
|---|---|
| `/admin/questions` | Question management list |
| `/admin/questions/create` | Create a new question |

## Question Types

| Type | Description | Requires Options |
|---|---|---|
| `text` | Single-line text input | No |
| `number` | Numeric input | No |
| `yes_no` | Boolean toggle | No |
| `dropdown` | Single-select dropdown | Yes |
| `radio` | Radio button group | Yes |
| `checkbox` | Single checkbox | No |
| `multiselect` | Multi-select list | Yes |

## API Reference

### GET /api/questions/{id}

Retrieves a single question by its GUID.

**Request**
```
GET /api/questions/b5d8f0e1-1c5b-4f7b-8cfa-6cab5f7fd001
```

**Response — 200 OK**
```json
{
  "id": "b5d8f0e1-1c5b-4f7b-8cfa-6cab5f7fd001",
  "key": "first_name",
  "label": "First Name",
  "type": "text",
  "required": true,
  "placeholder": "Enter your first name",
  "defaultValue": null,
  "helpText": null,
  "options": [],
  "visibleIf": null,
  "validationConfigs": null
}
```

**Response — 404 Not Found**

Returned when no question exists with the given ID.

```json
{}
```

**Response — 400 Bad Request**

Returned when the ID is empty, whitespace, or not a valid GUID.

```json
{
  "error": "Invalid question id. Provide a non-empty GUID value."
}
```

---

### POST /api/questions

Creates a new question.

**Request body**
```json
{
  "key": "study_level",
  "label": "What is your highest level of study?",
  "type": "dropdown",
  "required": true,
  "options": [
    { "label": "High school", "value": "high_school" },
    { "label": "Bachelor's degree", "value": "bachelor" },
    { "label": "Master's degree", "value": "master" },
    { "label": "PhD or higher", "value": "phd" }
  ]
}
```

**Response — 201 Created**

Returns the created question with its assigned `id`.

**Response — 400 Bad Request**

Returned when validation fails (e.g. missing required fields, option-based type with no options).

**Response — 409 Conflict**

Returned when a question with the same `key` or `id` already exists.

---

### Example: Option-based question JSON

```json
{
  "key": "contact_method",
  "label": "Preferred contact method",
  "type": "radio",
  "required": true,
  "options": [
    { "label": "Email", "value": "email" },
    { "label": "Phone", "value": "phone" },
    { "label": "Text message", "value": "text" }
  ]
}
```

### Example: Question with conditional visibility

```json
{
  "key": "campus_preference",
  "label": "Preferred campus location",
  "type": "dropdown",
  "required": true,
  "visibleIf": {
    "key": "is_student",
    "shouldEqual": true
  },
  "options": [
    { "label": "North campus", "value": "north" },
    { "label": "South campus", "value": "south" }
  ]
}
```

## Running Tests

```bash
# Backend integration tests
cd FormFlow.Backend.Tests
dotnet test

# Blazor bUnit component tests
cd FormFlow.Blazor.Tests
dotnet test

# Data model and validation tests
cd FormFlow.Data.Tests
dotnet test
```

## License

MIT License