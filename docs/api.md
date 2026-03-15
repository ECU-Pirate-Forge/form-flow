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

## GET `/api/questions/template`

Retrieve a default template question object.

This endpoint is used to provide a starter question for form creation workflows.

---

## Request

### **Method**

```
GET
```

### **URL**

```
/api/questions/template
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
| **200 OK**                    | Template question returned successfully |
| **500 Internal Server Error** | Unexpected server error                 |

---

### **Response Body (JSON)**

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "key": "exampleKey",
  "label": "Example question",
  "type": "text",
  "required": true,
  "placeholder": "Enter value",
  "defaultValue": null,
  "options": [],
  "visibleIf": {
    "questionKey": null,
    "equals": false
  },
  "validationRules": {
    "minLength": 1,
    "maxLength": 100
  },
  "helpText": "This is an example question."
}
```

### **Field Descriptions**

| Field                         | Type             | Description                                                                                                 |
| ----------------------------- | ---------------- | ----------------------------------------------------------------------------------------------------------- |
| `id`                        | `string(Guid)` | Unique identifier for the question. Template questions typically use an empty GUID.                         |
| `key`                       | `string`       | Internal key used to reference this question in form submissions.                                           |
| `label`                     | `string`       | The text displayed to the user.                                                                             |
| `type`                      | `string`       | The question type (e.g.,`"text"`,`"number"`,`"dropdown"`)                                             |
| `required`                  | `boolean`      | Whether the user must answer this question.                                                                 |
| `placeholder`               | `string`       | Placeholder text shown inside input fields                                                                  |
| `defaultValue`              | `any`          | Default value for the question (null if none).                                                              |
| `options`                   | `array`        | List of selectable options (used for dropdown, radio, checkbox questions). Empty for text questions.        |
| `visibleIf`                 | `object`       | Conditional visibility rules. Determines whether this question is shown based on another question’s value. |
| `validationRules`           | `object`       | Validation constraints for the question.                                                                    |
| `validationRules.minLength` | `number`       | Minimum allowed length for text input.                                                                      |
| `validationRules.maxLength` | `number`       | Maximum allowed length for text input.                                                                      |
| `helpText`                  | `string`       | Additional guidance shown beneath the question                                                              |

---

## Example Requests

### **cURL**

```bash
curl -X GET "https://localhost:7209/api/questions/template" \
     -H "Accept: application/json"
```

### **PowerShell**

```powershell
Invoke-RestMethod -Method GET `
  -Uri "https://localhost:7209/api/questions/template" `
  -Headers @{ Accept = "application/json" }
```

### **Postman Instructions**

1. Open Postman
2. Create a new **GET** request
3. Enter the URL:
   ```
   https://localhost:7209/api/questions/template
   ```
4. Set header:
   ```
   Accept: application/json
   ```
5. Click **Send**

**Expected Response:**

* Status: `200 OK`
* Body: JSON object with `label` and `type`
* Content-Type: `application/json; charset=utf-8`

---

# Automated Test Coverage

The following integration tests validate this endpoint:

### `Get_TemplateQuestion_Returns200AndValidQuestion`

* Ensures the endpoint returns **200 OK**
* Ensures the JSON body contains non‑empty `label` and `type`

### `Get_TemplateQuestion_ProducesJson`

* Ensures the response `Content-Type` is `application/json; charset=utf-8`

These tests run using `WebApplicationFactory<Program>` to spin up the real backend.

---

# Notes

* This endpoint currently returns a static template question.
* Future enhancements may include:
  * Dynamic question templates
  * Multiple question types
  * User‑defined templates
  * Database‑driven question generation

---
