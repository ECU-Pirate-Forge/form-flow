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

# **Endpoints**

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

## Request

### **Method**

```
GET
```

### **URL**

```
/api/questions/{id}
```

Example:

```
/api/questions/b5d8f0e1-1c5b-4f7b-8cfa-6cab5f7fd001
```

### **Headers**

```
Accept: application/json
```

### **Authentication**

None required.

---

## Response

### **Status Codes**

| Status Code                         | Meaning                                 |
| ----------------------------------- | --------------------------------------- |
| **200 OK**                    | Question returned successfully          |
| **400 Bad Request**           | ID is empty, whitespace, or malformed   |
| **404 Not Found**             | No question exists for the provided ID  |

---

### **200 Response Body (JSON)**

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

### **400 Response Body (JSON)**

{
  "error": "Invalid question id. Provide a non-empty GUID value."
}

### **404 Response Body**

No response body.

### **Field Descriptions**

| Field                         | Type             | Description                                                                                                          |
| ----------------------------- | ---------------- | -------------------------------------------------------------------------------------------------------------------- |
| `id`                        | `string(Guid)` | Unique identifier for the question.                                                                                  |
| `key`                       | `string`       | Internal key used to reference this question in form submissions.                                                    |
| `label`                     | `string`       | The text displayed to the user.                                                                                      |
| `type`                      | `string`       | The question type (e.g.,`"text"`,`"number"`,`"dropdown"`)                                                      |
| `required`                  | `boolean`      | Whether the user must answer this question.                                                                          |
| `placeholder`               | `string`       | Placeholder text shown inside input fields                                                                           |
| `defaultValue`              | `any`          | Default value for the question (null if none).                                                                       |
| `options`                   | `array`        | List of selectable options (used for dropdown, radio, checkbox questions). Empty for text questions.                 |
| `visibleIf`                 | `object`       | Conditional visibility rules. Determines whether this question is shown based on another question’s value.          |
| `validationConfigs`         | `string`       | A JSON-serialized array of validation rule objects. Each rulei ncludes a `ValidationType` and optional parameters. |
| `validationRules.minLength` | `number`       | Minimum allowed length for text input.                                                                               |
| `validationRules.maxLength` | `number`       | Maximum allowed length for text input.                                                                               |
| `helpText`                  | `string`       | Additional guidance shown beneath the question                                                                       |

---

## Example Requests

### **cURL**

```bash
curl -X GET "https://localhost:7209/api/questions/b5d8f0e1-1c5b-4f7b-8cfa-6cab5f7fd001" \
     -H "Accept: application/json"
```

### **PowerShell**

```powershell
Invoke-RestMethod -Method GET `
  -Uri "https://localhost:7209/api/questions/b5d8f0e1-1c5b-4f7b-8cfa-6cab5f7fd001" `
  -Headers @{ Accept = "application/json" }
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

# Automated Test Coverage

The following integration tests validate this endpoint:

### `Get_QuestionById_Returns200AndRequestedQuestion`

* Ensures the endpoint returns **200 OK**
* Ensures the JSON body contains the requested question

### `Get_QuestionById_Returns404WhenQuestionDoesNotExist`

* Ensures unknown IDs return **404 Not Found**

### `Get_QuestionById_Returns400ForInvalidId`

* Ensures invalid IDs return **400 Bad Request**

### `Get_QuestionById_WithEmptyDatabase_Returns404AndDoesNotThrow`

* Ensures empty database lookups return **404 Not Found** and do not throw exceptions

### `Get_QuestionById_ProducesJson`

* Ensures the response `Content-Type` is `application/json; charset=utf-8`

These tests run using `WebApplicationFactory<Program>` to spin up the real backend.

---

# Notes

* This endpoint is intentionally read-only.
* Out of scope for this endpoint:
  * GET all questions
  * POST/PUT/DELETE
  * Survey hydration
  * Additional validation logic

---
