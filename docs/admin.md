
# **Admin Module Documentation**

## Overview
The Admin Module provides pages and tools for managing FormFlow resources such as questions, forms, users, and submissions.  
Each admin feature is organized under a dedicated route within the `/admin` namespace.

This document grows as new admin pages are added.

---

# **Admin Routes**

## **1. Admin Dashboard**
**Route:**  
```
/admin
```

**Purpose:**  
Landing page for the admin area. Provides navigation to all admin modules.

**Status:**  
_Not implemented yet._

---

## **2. Question Management**
### **2.1 Questions List Page**
**Route:**  
```
/admin/questions
```

**Purpose:**  
Entry point for managing questions. Displays a placeholder header and a button for navigating to the question creation page.

**Current Features:**
- Loads successfully at `/admin/questions`
- Shows header: **“Question Management”**
- Contains a **Create New Question** button (navigation enabled once the create page exists)
- Placeholder text describing the module

**Future Enhancements:**
- Display list of existing questions  
- Add edit/delete actions  
- Add search/filter  
- Integrate with backend question API  

---

### **2.2 Create Question Page**
**Route:**  
```
/admin/questions/create
```

**Purpose:**  
Form for creating a new question.

**Status:**  
Implemented in the Blazor admin module.

**Field Reference (Create Question Form):**

1. **Label**
- Purpose: Human-readable question text shown to end users.
- Example: `Favorite programming language`
- Validation: Required.

2. **Key**
- Purpose: Stable internal identifier used in payloads, storage, and logic.
- Example: `favorite_language`
- Validation: Required.

3. **Type**
- Purpose: Defines which UI component renders the question.
- Supported values: `dropdown`, `text`, `yes_no`, `number`, `multiselect`, `checkbox`, `radio`
- Validation: Required.

4. **Required**
- Purpose: Controls whether the end user must answer the question.
- Type: Boolean toggle.
- Validation: Optional.

5. **Placeholder**
- Purpose: Hint text displayed inside an empty input/select when applicable.
- Type: Text.
- Validation: Optional.

6. **Default Value**
- Purpose: Pre-populated value shown before user input.
- Type: Text (interpreted by renderer/question type).
- Validation: Optional.

7. **Help Text**
- Purpose: Supplemental guidance shown near the question to clarify expected input.
- Type: Text.
- Validation: Optional.

**Validation Behavior:**
- Required validation is enforced only for `Label`, `Key`, and `Type`.
- Other fields are captured if provided, but they are not required for form submission.

**Current Limitation:**
- No client-side validation beyond the three required fields. Invalid data (e.g. bad format, empty options on an option-based type) is caught by the backend and surfaced as an error alert.
- No draft saving. The form does not persist if you navigate away before submitting.
- No network error handling. If the backend is unreachable the button will lock. Refresh the page to recover.
---
**How to Create a Question:**
 
This guide covers how an admin creates a new question using the admin form.
 
**Prerequisites**
 
Both the backend and Blazor front end must be running locally before using the admin form.

---
 
**Steps**
 
1. Navigate to the admin area and click **Create Question**.
2. Fill in the required fields — **Label**, **Key**, and **Type**. The Save button stays disabled until all three are provided.
3. Fill in any optional fields — Placeholder, Default Value, Help Text, and the Required toggle.
4. If the selected Type is `dropdown`, `radio`, `checkbox`, or `multiselect`, an **Options** section appears. Use **Add Option** to add label/value pairs. Use the trash icon to remove one.
5. Click **Create Question** to submit.
---
**After Submitting**
 
**On success:**
- A success alert appears confirming the question was created.
- The form clears, ready for another entry.

**On failure:**
- An error alert appears with the reason (e.g. duplicate key, validation error).
- The form stays filled so the admin can correct and resubmit.
---
