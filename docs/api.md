# API Reference

## POST /api/responses

Saves a completed form response to LiteDB.

### Request Body

```json
{
	"formId": "onboarding-form-2026",
	"answers": {
		"firstName": "Ada",
		"lastName": "Lovelace"
	},
	"submittedBy": "user-123"
}
```

### Validation Rules

- `formId` is required and cannot be empty.
- `answers` is required and must contain at least one key/value pair.
- Answer keys cannot be null, empty, or whitespace.

### Success Response

- Status: `201 Created`
- `Location` header: `/api/responses/{id}`

```json
{
	"id": "d5f89e7b8b954f5e94f116c8f6a6d08f",
	"formId": "onboarding-form-2026",
	"answers": {
		"firstName": "Ada",
		"lastName": "Lovelace"
	},
	"submittedBy": "user-123",
	"submittedAtUtc": "2026-02-28T21:10:34.4818158Z"
}
```

### Validation Error Response

- Status: `400 Bad Request`

```json
{
	"errors": {
		"formId": ["The formId field is required."],
		"answers": ["At least one answer is required."]
	}
}
```

## GET /api/responses/{id}

Fetches a previously saved response by id.

### Success Response

- Status: `200 OK`

```json
{
	"id": "d5f89e7b8b954f5e94f116c8f6a6d08f",
	"formId": "onboarding-form-2026",
	"answers": {
		"firstName": "Ada",
		"lastName": "Lovelace"
	},
	"submittedBy": "user-123",
	"submittedAtUtc": "2026-02-28T21:10:34.4818158Z"
}
```

### Not Found Response

- Status: `404 Not Found`