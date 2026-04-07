# **SurveyDefinition Model Documentation**

## Overview

`SurveyDefinition` represents the structure of a survey stored in LiteDB and exchanged through JSON.

It defines the survey’s metadata, the ordered list of questions it contains, and versioning information.

This model is used by:

* The admin panel (creating & editing surveys)
* The backend API (loading & saving surveys)
* The form renderer (displaying surveys to end users)
* Unit tests validating schema alignment

---

## **C# Model Definition**

```csharp
using LiteDB;

namespace FormFlow.Data.Models
{
    public class SurveyDefinition
    {
        [BsonId]
        public Guid Id { get; set; }

        public required string Title { get; set; }
        public required string Description { get; set; }
        public required List<Guid> QuestionIds { get; set; }
        public required DateTime CreatedAt { get; set; }

        public int? Version { get; set; }
    }
}
```

### Key Notes

* **Id** is the LiteDB primary key.
* **QuestionIds** preserves question order — the order of GUIDs is the order questions appear in the survey.
* All fields except `Version` are **required** and enforced by C# 11 `required` properties.
* Missing required fields during JSON deserialization will throw a `JsonException`.

---

## **JSON → C# Property Mapping**

| JSON Field      | C# Property     | Type           | Required | Notes                        |
| --------------- | --------------- | -------------- | -------- | ---------------------------- |
| `id`          | `Id`          | `Guid`       | Yes      | LiteDB document ID           |
| `title`       | `Title`       | `string`     | Yes      | Survey title                 |
| `description` | `Description` | `string`     | Yes      | Survey description           |
| `questionIds` | `QuestionIds` | `List<Guid>` | Yes      | Ordered list of question IDs |
| `createdAt`   | `CreatedAt`   | `DateTime`   | Yes      | ISO 8601 timestamp           |
| `version`     | `Version`     | `int?`       | No       | Optional version number      |

### Casing Behavior

* JSON uses  **camelCase** .
* C# uses  **PascalCase** .
* Deserialization succeeds because we enable:
  ```csharp
  PropertyNameCaseInsensitive = true
  ```

---

## **Deserializing a Survey**

To deserialize JSON into a `SurveyDefinition`, use:

```csharp
var options = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
};

var survey = JsonSerializer.Deserialize<SurveyDefinition>(jsonString, options);
```

### Why this is required

* Your JSON schema uses camelCase (`questionIds`, `createdAt`)
* Your C# model uses PascalCase (`QuestionIds`, `CreatedAt`)
* Without case-insensitive matching, System.Text.Json will throw a `JsonException` for missing required fields.

---

## **Validation Behavior**

### Required fields

Because the model uses C# 11 `required` properties:

```csharp
public required string Title { get; set; }
```

System.Text.Json enforces these at deserialization time.

If any required field is missing, the following occurs:

* Deserialization fails immediately
* A `JsonException` is thrown
* The exception message lists missing properties

Example:

```
JSON deserialization for type 'SurveyDefinition' was missing required properties including: 'Title', 'Description', 'QuestionIds', 'CreatedAt'.
```

---

## **Example Valid Survey JSON**

```json
{
  "id": "11111111-1111-1111-1111-111111111111",
  "title": "Customer Feedback",
  "description": "A simple survey.",
  "questionIds": [
    "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
    "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
  ],
  "createdAt": "2024-01-01T12:00:00Z",
  "version": 1
}
```

---
