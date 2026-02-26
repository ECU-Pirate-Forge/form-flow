# UI Templates: Running React and Blazor

This guide explains how to run the React and Blazor (MudBlazor) UI templates so you can verify that each one loads the sample JSON question and renders it using its respective `QuestionRenderer` component.

## React Template

### Location

`/web/react-app`

### Prerequisites

- Node.js and npm installed

### Steps to Run

1. Open a terminal.
2. Navigate to the React project folder:

   ```
   cd web/react-app
   ```
3. Install dependencies (first time only):

   ```
   npm install
   ```
4. Start the development server:

   ```
   npm start
   ```
5. Open the app in your browser:
   `http://localhost:3000`

### What You Should See

- The React app loads `sample-question.json` from the `public` folder.
- The question label appears.
- A text input renders using the `QuestionRenderer` component.
- Refreshing the page continues to work.
- The app uses `REACT_APP_API_URL` for future API integration.

## Blazor (MudBlazor) Template

### Location

`/web/blazor`

### Prerequisites

- .NET SDK installed

### Steps to Run

1. Open a terminal.
2. Navigate to the Blazor project folder:

   ```
   cd web/blazor
   ```
3. Run the Blazor app:

   ```
   dotnet run
   ```
4. Open the app in your browser:

   `http://localhost:5224`


### What You Should See

- The Blazor app loads the same `sample-question.json` from `wwwroot`.
- The question label appears.
- A MudBlazor text input renders using `QuestionRenderer.razor`.
- Refreshing the page continues to work.
- The app uses configuration settings for the API base URL.

## Shared Requirements

Both UI templates:

- Use the same JSON structure defined in PBI 1.1.
- Render only a text input for Sprint 1.
- Are placed in clearly labeled folders:
  `/web/react` `/web/blazor`
- Serve as starter templates for future dynamic form rendering.
