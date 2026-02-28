# API Reference

## POST /api/responses

Saves a completed form response to LiteDB.

### Request Body (JSON)

```json
{
	"formId": "onboarding-2026",
	"userId": "user-123",
	"submittedAtUtc": "2026-02-27T12:00:00Z",
	"answers": {
		"q1": "Yes",
		"q2": "No"
	}
}
```

### Validation Rules

- `formId` is required and cannot be empty
- `userId` is required and cannot be empty
- `submittedAtUtc` is required
- `answers` is required and must contain at least one key/value pair

### Responses

- `201 Created` when the response is saved successfully
- `400 Bad Request` when validation fails

### Thunder Client / Postman Test

1. Start the API from repository root with `dotnet run --project backend/backend.csproj --urls http://localhost:5055`.
2. Create a `POST` request to `http://localhost:5055/api/responses`.
3. Add header `Content-Type: application/json`.
4. Paste the sample JSON body above and send.
5. Confirm status is `201 Created` and response body contains generated `id`.