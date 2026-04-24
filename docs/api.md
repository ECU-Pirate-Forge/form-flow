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

Creates a new question and uses `QuestionRepository.cs` to insert into `LiteDb`

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

| Name | In | Type | Required | Description |
|------|----|------|----------|-------------|
| `id` | path | string (uuid) | Yes | The survey’s unique identifier |

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

---

