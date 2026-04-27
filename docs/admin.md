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
- A green success alert appears below the page title confirming the question was created, including the question label (e.g. "Question 'Age' created successfully.").
- The form clears and is ready for another entry.
- The alert can be manually dismissed using the close icon.

**On failure:**
- A red error alert appears below the page title with the reason returned from the server (e.g. "409: A question with key 'age' already exists").
- The form stays filled so the admin can correct and resubmit.
- The alert can be manually dismissed using the close icon.

**Note:** Both alerts are mutually exclusive — a new submit attempt clears any previous alert before the next result appears.

---

**## 3. Survey Management**

**### 3.1 Create Survey Page**

**Route:**

`/admin/surveys/create`

**Purpose:**
Provides an interface for admins to create a new survey by entering metadata and selecting questions from the question bank.

**Status:**
Implemented in the Blazor admin module.

Absolutely, Maysun — I can fold **AdminCreateSurvey** into your Admin Module Documentation so it sits naturally alongside the other admin pages. I’ll keep the structure, tone, and formatting consistent with the rest of your document, and I’ll make sure it reflects the actual behavior of your component (question selection, validation, save‑button logic, etc.).

Here is the updated documentation section with **AdminCreateSurvey** added cleanly and professionally.

---

**Page Structure & Behavior**

**Survey Metadata Fields**
The top section of the page contains a form for entering basic survey information:

1. **Survey Title**

   - Required
   - Displayed to end users as the survey’s main heading
   - Validation: Must not be empty
2. **Description**

   - Required
   - Provides context or instructions for the survey
   - Validation: Must not be empty

Validation is handled through MudBlazor’s `MudForm` and updates the internal form state.

---

**Question Bank**
Below the metadata fields, the page displays a scrollable list of all available questions retrieved from the backend.

Each question entry shows:

- **Label** (e.g., “Age”, “Favorite Color”)
- **Type** (e.g., text, number, dropdown)
- An **Add** button

**Add Button Behavior:**

- When clicked, the question is added to the survey’s selected list.
- Once added, the button changes to **“Added”** and becomes disabled.
- Prevents duplicate selection.

---

**Selected Questions**
A separate list shows all questions chosen for the survey.

Each entry includes:

- The question label
- A **Remove** button

**Remove Button Behavior:**

- Removes the question from the selected list
- Re‑enables the “Add” button in the question bank
- Keeps the UI in sync with the internal survey state

---

**Save Survey Button**

The **Save Survey** button is located at the bottom of the page.

**Enable/Disable Logic**
The button becomes enabled only when:

1. The form is valid (`Title` and `Description` are filled), **and**
2. At least one question has been selected.

This logic is enforced through the component’s internal state and MudBlazor form validation.

**Current Behavior**

- The button is disabled until all requirements are met.
- The save action currently logs the survey model and navigates back to `/admin/surveys`.
- Backend persistence integration is planned for a future update.

---

**Future Enhancements**

- Persist surveys to backend storage
- Add survey editing functionality
- Add preview mode
- Add question ordering (drag‑and‑drop)
- Add survey activation/deactivation controls

---

## **3. Survey List Page**

**Route:** `/admin/surveys`
**Component:** `AdminSurveysList.razor`
**Purpose:** Displays all existing surveys and provides navigation to preview each one.

**Overview**
The Survey List Page allows administrators to view all surveys that have been created in the system. Each survey entry includes its title, description, and question count, along with a “Preview” action that navigates to the survey preview page.

This page serves as the central hub for managing and reviewing surveys.

---

**Features**

- Loads all surveys from the backend via `GET /api/surveys`
- Displays survey metadata in a MudTable:
  - Title
  - Description
  - Number of questions
- Provides a **Preview** button for each survey
- Handles empty states when no surveys exist
- Integrated into the Admin Layout and sidebar navigation

---

**Backend Integration**
**API Endpoint**

```
GET /api/surveys
```

**Data Model**
Each survey is returned as a `SurveyDefinition`:

- `Id` (string / GUID)
- `Title`
- `Description`
- `QuestionIds` (list of question IDs)

---

**UI Behavior**

- On initialization, the component fetches all surveys.
- If the API returns an empty list, the page displays:**“No surveys found.”**
- Each row includes a **Preview** button:
  - Navigates to `/admin/surveys/{id}/preview`
  - Uses absolute navigation (`/admin/...`) to avoid relative path issues

---

**Navigation Flow**

- From the sidebar: **Surveys → Survey List**
- From the Question Bank: admins may create a survey, then return here to view it
- From this page: clicking **Preview** opens the read‑only preview page

---

**Error Handling**

- If the API request fails, the page displays a generic error message
- The table does not render until data is loaded
- Loading state is handled via MudBlazor progress indicators (if implemented)

---

## **4. Survey Preview Page**  
**Route:** `/admin/surveys/{id}/preview`  
**Component:** `AdminSurveyPreview.razor`  
**Purpose:** Allows administrators to view a fully rendered, read‑only preview of a survey before publishing or sharing it.

---

**Overview**
The Survey Preview Page displays a complete, user‑facing rendering of a survey.  
It loads the survey definition and all associated questions, then renders each question using the shared `QuestionRenderer` component.

This page is used by admins to verify:

- Survey title and metadata  
- Question order  
- Question text and configuration  
- Conditional visibility (if implemented)  
- Rendering consistency with the public survey experience  

---

**Features**
- Loads a single survey definition via:  
  `GET /api/surveys/{id}`
- Loads each question referenced by the survey via:  
  `GET /api/questions/{questionId}`
- Displays:
  - Survey title  
  - All questions in the correct order  
  - Each question rendered through `QuestionRenderer`
- Shows a loading spinner while data is being fetched
- Gracefully handles missing or invalid survey IDs
- Read‑only preview (no editing or answering)

---

**Backend Integration**

**API Endpoints**
```
GET /api/surveys/{id}
GET /api/questions/{questionId}
```

**Data Models**
**SurveyDefinition**
- `Id` (Guid)  
- `Title`  
- `Description`  
- `QuestionIds` (List<Guid>)  
- `CreatedAt`

**QuestionDefinition**
- `Id`  
- `Label`  
- `Key`  
- `Type`  
- `Required`  
- `Options` (if applicable)  
- `Placeholder`  
- `HelpText`  

---

**UI Behavior**
- On initialization:
  - Fetches the survey definition
  - Fetches each referenced question
- While loading:
  - Displays a `MudProgressCircular` spinner
- After loading:
  - Renders the survey title (`MudText Typo="h4"`)
  - Renders each question using `<QuestionRenderer />`
- If the survey is not found:
  - Spinner remains visible (or can be replaced with an error message if desired)

---

**Navigation Flow**
- From Survey List Page → **Preview**
- From Preview Page → (optional future buttons)
  - Back to Survey List
  - Publish Survey
  - Edit Survey

---

**Error Handling**
- If the survey request returns 404:
  - Page stays in loading state (current behavior)
- If any question fails to load:
  - That question is skipped
- No UI crashes — the page always renders safely

---


