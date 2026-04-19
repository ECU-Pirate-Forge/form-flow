# FormFlow Backend

## Questions API

### GET /api/questions/{id}

Returns a single question document from the LiteDB `questions` collection.

### Path Parameter

- `id` (string, required): Question ID as a GUID.

### Request Example

```bash
curl -X GET "https://localhost:7209/api/questions/b5d8f0e1-1c5b-4f7b-8cfa-6cab5f7fd001" \
  -H "Accept: application/json"
```

### 200 OK Example

```json
{
  "id": "b5d8f0e1-1c5b-4f7b-8cfa-6cab5f7fd001",
  "key": "first_name",
  "label": "First Name",
  "type": "text",
  "required": true,
  "placeholder": "Enter your first name",
  "defaultValue": null,
  "options": [],
  "visibleIf": null,
  "validationConfigs": null,
  "helpText": null
}
```

### 400 Bad Request Example

Returned when `id` is empty, whitespace, or not a valid GUID.

```json
{
  "error": "Invalid question id. Provide a non-empty GUID value."
}
```

### 404 Not Found

Returned when the `id` is valid but no question exists with that ID.

Response body is empty.

### Response Content Type

- `application/json` for successful responses.

## Notes

- This endpoint is read-only and uses `QuestionRepository`.
- No list, create, update, or delete operations are provided here.
