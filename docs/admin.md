
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
_Not implemented yet._  
Button on `/admin/questions` will link here once created.

**Future Enhancements:**
- Question type selector  
- Dynamic form fields based on type  
- Validation  
- Save to backend  

---
