# API Reference

Document endpoints and examples.

---

## Base URL (Local Development)

```
http://localhost:5164
```

If running with the HTTPS profile:

```
https://localhost:7209
```

---

# **Question Endpoints**

---

## GET `/api/questions/{id}`

Retrieve one question document by ID from the LiteDB `questions` collection.

This endpoint is used by frontend clients to load and render real question data.

## POST `/api/questions`

Maps `NewQuestion` to `QuestionDefinition` and uses `QuestionRepository.cs` to insert into `LiteDb`

## GET `/api/questions`

This endpoint uses `QuestionRepository.cs` to find all the questions in the `questions` collection. 
It then produces a list of all the questions in the database.

---

# **Survey Endpoints**

## **GET /api/surveys**

Retrieve all surveys.

**Summary**
Returns a list of all saved surveys.

**Responses**

**200 OK**
**Response Body**

```json
[
  {
    "surveyId": "c1b2e3d4-5678-49ab-9cde-1234567890ab",
    "title": "Employee Satisfaction Survey",
    "description": "Quarterly feedback survey",
    "questionIds": [
      "11111111-2222-3333-4444-555555555555",
      "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"
    ],
    "createdAt": "2024-04-23T18:32:10.123Z"
  }
]
```

---

## **GET /api/surveys/{id}**

Retrieve a single survey by its ID.

**Parameters**

| Name   | In   | Type          | Required | Description                     |
| ------ | ---- | ------------- | -------- | ------------------------------- |
| `id` | path | string (uuid) | Yes      | The survey’s unique identifier |

---

**Responses**

**200 OK**
**Response Body**

```json
{
  "surveyId": "c1b2e3d4-5678-49ab-9cde-1234567890ab",
  "title": "Employee Satisfaction Survey",
  "description": "Quarterly feedback survey",
  "questionIds": [
    "11111111-2222-3333-4444-555555555555",
    "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"
  ],
  "createdAt": "2024-04-23T18:32:10.123Z"
}
```

---

**400 Bad Request**
Returned when the provided ID is not a valid GUID.

**Response Body**

```json
{
  "error": "Invalid survey id. Must be a GUID."
}
```

---

**404 Not Found**
Returned when no survey exists with the given ID.

**Response Body**

```json
{
  "error": "Survey not found."
}
```

### **Postman Instructions**

1. Open Postman
2. Create a new **GET** request
3. Enter the URL:
   ```
    https://localhost:7209/api/questions/b5d8f0e1-1c5b-4f7b-8cfa-6cab5f7fd001
   ```
4. Set header:
   ```
   Accept: application/json
   ```
5. Click **Send**

**Expected Response:**

* Status: `200 OK`
* Body: JSON object matching `QuestionDefinition`
* Content-Type: `application/json; charset=utf-8`

---

Absolutely — and now that your POST endpoint behavior is stable and we know exactly how it works, I can generate **clean, professional, API‑ready documentation** for it.

I’ll follow the same structure you’ve been using in your project:

- Endpoint summary
- Request schema
- Response schema
- Status codes
- Example request
- Example response

This will drop directly into your `/docs` folder or your API.md.

---

## **POST /api/surveys**

**Endpoint**

```
POST /api/surveys - Create a New Survey
```

Creates a new survey using the data provided by the client.
The server generates the survey ID and timestamp.

---

**Request Body (NewSurvey)**

```json
{
  "title": "string",
  "description": "string",
  "questionIds": ["guid"]
}
```

**Field Descriptions**

| Field           | Type           | Required | Description                                      |
| --------------- | -------------- | -------- | ------------------------------------------------ |
| `title`       | string         | yes      | The title of the survey.                         |
| `description` | string         | yes      | A short description of the survey’s purpose.    |
| `questionIds` | array of GUIDs | yes      | The list of question IDs included in the survey. |

---

**Response Body (SurveyDefinition)**

```json
{
  "id": "guid",
  "title": "string",
  "description": "string",
  "questionIds": ["guid"],
  "createdAt": "2024-01-01T00:00:00Z"
}
```

**Field Descriptions**

| Field           | Type           | Description                                         |
| --------------- | -------------- | --------------------------------------------------- |
| `id`          | GUID           | Server‑generated unique identifier for the survey. |
| `title`       | string         | Title of the survey.                                |
| `description` | string         | Description of the survey.                          |
| `questionIds` | array of GUIDs | IDs of questions included in the survey.            |
| `createdAt`   | datetime       | Server‑generated timestamp of creation.            |

---

**Status Codes**

| Code                                | Meaning                                                                |
| ----------------------------------- | ---------------------------------------------------------------------- |
| **201 Created**               | Survey successfully created. Response includes the full survey object. |
| **400 Bad Request**           | Invalid input (e.g., malformed JSON, missing required fields).         |
| **500 Internal Server Error** | Unexpected server error.                                               |

---

**Example Request**

```http
POST /api/surveys
Content-Type: application/json

{
  "title": "Employee Satisfaction Survey",
  "description": "Quarterly feedback survey",
  "questionIds": [
    "c1f6d9c3-4b8e-4c1f-9e1f-2a1a1b1c1d1e",
    "b2e7f0a4-5c9f-4d2f-8e2f-3b2b2c2d2e2f"
  ]
}
```

---

**Example Response (201 Created)**

```http
HTTP/1.1 201 Created
Location: /api/surveys/7f3c2a1b-9d4e-4c2f-8b1e-123456789abc
Content-Type: application/json

{
  "id": "7f3c2a1b-9d4e-4c2f-8b1e-123456789abc",
  "title": "Employee Satisfaction Survey",
  "description": "Quarterly feedback survey",
  "questionIds": [
    "c1f6d9c3-4b8e-4c1f-9e1f-2a1a1b1c1d1e",
    "b2e7f0a4-5c9f-4d2f-8e2f-3b2b2c2d2e2f"
  ],
  "createdAt": "2024-01-01T14:23:11.123Z"
}
```

---

**Behavior Summary**

- The client provides only editable fields (`title`, `description`, `questionIds`).
- The server generates:
  - `id`
  - `createdAt`
- The repository’s `Insert()` method is called to persist the survey.
- The endpoint returns **201 Created** with the full survey object.

---
