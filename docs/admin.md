
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
- Submit action currently validates client-side form state, but backend persistence integration is still pending.

---
