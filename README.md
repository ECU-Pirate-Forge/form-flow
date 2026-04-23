# FormFlow

FormFlow is a multi-platform dynamic form generation and management system. It allows developers and administrators to define, store, and render survey/form questions with rich validation, conditional visibility logic, and support for multiple frontend platforms. The project is built with a shared .NET data layer, an ASP.NET Core REST API backend, a Blazor Server web UI, and a React TypeScript SPA — with mobile platforms (Flutter, MAUI, React Native) planned.

---

## Table of Contents

- [Project Background](#project-background)
- [Implemented Features](#implemented-features)
- [Planned Roadmap](#planned-roadmap)
- [Project Directory Structure](#project-directory-structure)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [1 — Clone the Repository](#1--clone-the-repository)
  - [2 — Run the Backend API](#2--run-the-backend-api)
  - [3 — Run the Blazor Web UI](#3--run-the-blazor-web-ui)
  - [4 — Run the React SPA](#4--run-the-react-spa)
  - [5 — Run the Tests](#5--run-the-tests)
- [API Reference](#api-reference)
- [Contributing](#contributing)
- [License](#license)

---

## Project Background

Modern organizations frequently need to collect structured data from users through forms and surveys. Hard-coded forms are brittle and expensive to maintain — changing a single field often requires a full release cycle. FormFlow solves this by storing question definitions in a database and rendering them dynamically at runtime.

Each question carries its own metadata: display label, input type, placeholder text, help text, required flag, selectable options, conditional visibility rules, and a JSON-serialized validation configuration. This design lets administrators update forms without touching source code, and it provides a consistent contract between the backend and any frontend client.

The project was designed from the start to support multiple rendering platforms. The shared `FormFlow.Data` library exposes the canonical `QuestionDefinition` model and validation engine, so every platform speaks the same language.

**Tech Stack:**

| Layer | Technology |
|-------|-----------|
| Backend API | ASP.NET Core 10, LiteDB 5 |
| Shared Models / Validation | .NET 10 Class Library |
| Web UI (interactive) | Blazor Server, MudBlazor 9 |
| Web UI (SPA) | React 19, TypeScript 4 |
| Testing | xUnit, Moq, FluentAssertions, bUnit, Jest, React Testing Library |
| Database | LiteDB (embedded document store) |

---

## Implemented Features

### Question Definition System
- Seven question types: `text`, `number`, `yes_no`, `dropdown`, `radio`, `checkbox`, `multiselect`
- Per-question metadata: `label`, `placeholder`, `helpText`, `defaultValue`, `required`
- Named keys (`key` field) for referencing questions in form submissions and conditional rules

### Conditional Visibility
- `VisibleIf` rules allow any question to be shown or hidden based on the current answer to another question
- Example: a "Campus Preference" dropdown only appears when the user answers "Yes" to "Are you a student?"

### Server-Side Validation Engine
- `QuestionValidationEngine` evaluates submitted responses against per-question rule lists
- Supported rule types: `MinLength`, `MaxLength`, `MinValue`, `MaxValue`, `Range`
- Returns a pass/fail result plus a list of human-readable error messages

### REST API
- `GET /api/questions` — retrieve all question definitions
- `GET /api/questions/{id}` — retrieve a single question by GUID
- `POST /api/questions` — create a new question (validates key uniqueness and required fields)
- Proper HTTP status codes: 200, 201, 400, 404, 409

### LiteDB Persistence & Seeding
- Embedded document database (no separate database server required)
- On first startup the backend seeds the `questions` collection from `SeedData/questions.json` with 10 representative sample questions covering all question types

### Blazor Server Web UI
- Interactive question list and admin panel powered by MudBlazor components
- `QuestionRenderer` dynamically resolves question type to the appropriate Blazor component at runtime
- Dedicated Razor components for every question type with two-way data binding and validation display
- Admin pages: question list (`/admin/questions`), create question (`/admin/questions/create`)

### React SPA
- React 19 TypeScript application that loads questions from a JSON file
- `QuestionRenderer` component renders each question with label, input, help text, and ARIA accessibility attributes
- Shared `QuestionDefinition`, `Option`, and `VisibleIf` TypeScript interfaces that mirror the backend models

### Automated Test Suite
- **FormFlow.Backend.Tests** — xUnit integration tests using `WebApplicationFactory`; covers all three API endpoints, 404/400/409 error cases, database seeding
- **FormFlow.Data.Tests** — unit tests for model validation and the `QuestionValidationEngine`
- **FormFlow.Blazor.Tests** — bUnit component tests for Blazor renderers
- **FormFlow.React.Tests** — Jest + React Testing Library tests for React components

### JSON Schema Validation
- `question-definition.schema.json` defines the canonical question structure
- `survey-definition.schema.json` references the question schema via `$ref` for modular, reusable validation

---

## Planned Roadmap

| Feature | Status | Notes |
|---------|--------|-------|
| Flutter mobile client | Planned | `FormFlow.Flutter` directory scaffolded |
| .NET MAUI mobile client | Planned | `FormFlow.Maui` directory scaffolded |
| React Native client | Planned | `FormFlow.ReactNative` directory scaffolded |
| Full Question CRUD | In Progress | GET and POST implemented; PUT and DELETE endpoints pending |
| Survey management | Planned | `SurveyDefinition` model defined; survey endpoints and UI not yet built |
| Form submission & response storage | Planned | `QuestionValidationEngine` ready; submission endpoints and storage not yet built |
| Analytics & reporting | Planned | Track submission counts, completion rates, and answer distributions |
| User authentication | Planned | Secure the admin panel; associate form responses with user accounts |
| Conditional visibility in React SPA | Planned | `VisibleIf` model defined; runtime evaluation not yet wired up in React |
| Enhanced React question type components | Planned | Currently all questions render as text inputs; type-specific components needed |
| Import/export question sets | Planned | JSON bulk-import and export for portability |

---

## Project Directory Structure

```
form-flow/
├── FormFlow.Backend/               # ASP.NET Core REST API
│   ├── Endpoints/
│   │   └── QuestionEndpoints.cs    # GET/POST /api/questions routes
│   ├── Repositories/
│   │   ├── IQuestionRepository.cs
│   │   └── QuestionRepository.cs   # LiteDB collection access
│   ├── SeedData/
│   │   └── questions.json          # 10 sample questions loaded on first run
│   ├── Schemas/
│   │   ├── question-definition.schema.json
│   │   └── survey-definition.schema.json
│   ├── DatabaseSeeder.cs           # Runs once at startup if DB is empty
│   ├── Program.cs
│   └── appsettings.json
│
├── FormFlow.Backend.Tests/         # xUnit integration + unit tests for backend
│
├── FormFlow.Data/                  # Shared .NET class library
│   ├── Models/
│   │   ├── QuestionDefinition.cs   # Core question model
│   │   ├── Option.cs
│   │   ├── VisibleIf.cs
│   │   ├── SurveyDefinition.cs
│   │   └── NewQuestion.cs          # DTO for POST requests
│   └── Services/
│       ├── QuestionValidator.cs
│       ├── IQuestionInserter.cs
│       └── QuestionValidationEngine.cs  # Min/Max/Range rule evaluator
│
├── FormFlow.Data.Tests/            # xUnit tests for models and validation engine
│
├── FormFlow.Blazor/                # Blazor Server web application
│   ├── Components/
│   │   ├── QuestionRenderer.razor  # Dynamic type-to-component mapper
│   │   ├── TextQuestion.razor
│   │   ├── NumberQuestion.razor
│   │   ├── YesNoQuestion.razor
│   │   ├── DropdownQuestion.razor
│   │   ├── RadioQuestion.razor
│   │   ├── CheckboxQuestion.razor
│   │   └── MultiselectQuestion.razor
│   ├── Pages/
│   │   ├── Home.razor
│   │   ├── AdminQuestions.razor
│   │   └── AdminCreateQuestion.razor
│   └── wwwroot/
│       └── multiple-sample-questions.json
│
├── FormFlow.Blazor.Tests/          # bUnit component tests for Blazor
│
├── FormFlow.React/                 # React 19 TypeScript SPA
│   ├── src/
│   │   ├── App.tsx                 # Root component; loads questions from JSON
│   │   ├── components/
│   │   │   └── QuestionRenderer.tsx
│   │   └── types/
│   │       ├── QuestionDefinition.ts
│   │       ├── Option.ts
│   │       └── VisibleIf.ts
│   └── package.json
│
├── FormFlow.React.Tests/           # Jest + React Testing Library tests
│
├── FormFlow.Flutter/               # Placeholder — Flutter client (planned)
├── FormFlow.Maui/                  # Placeholder — .NET MAUI client (planned)
├── FormFlow.ReactNative/           # Placeholder — React Native client (planned)
│
├── docs/                           # Extended documentation
│   ├── index.md
│   ├── architecture.md
│   ├── api.md
│   ├── backend.md
│   ├── database.md
│   ├── question-definition.md
│   ├── survey-definition.md
│   ├── blazor-components.md
│   ├── react-components.md
│   ├── admin.md
│   ├── testing.md
│   ├── test-suite-overview.md
│   └── troubleshooting.md
│
├── FormFlow.slnx                   # .NET solution file
├── package.json                    # Root npm scripts
├── AGENTS.md
├── CHANGELOG.md
├── CONTRIBUTING.md
├── CODE_OF_CONDUCT.md
└── LICENSE
```

---

## Getting Started

These instructions have been tested on a clean machine. Follow each section in order.

### Prerequisites

Install the following tools before proceeding:

| Tool | Version | Download |
|------|---------|----------|
| .NET SDK | 10.0 or later | https://dotnet.microsoft.com/download |
| Node.js | 18 or later | https://nodejs.org |
| npm | 9 or later (bundled with Node) | — |
| Git | Any recent version | https://git-scm.com |

Verify your installations:

```bash
dotnet --version   # should print 10.x.x
node --version     # should print v18.x.x or higher
npm --version      # should print 9.x.x or higher
git --version
```

---

### 1 — Clone the Repository

```bash
git clone https://github.com/your-org/form-flow.git
cd form-flow
```

> Replace the URL above with the actual repository URL if it differs.

---

### 2 — Run the Backend API

The backend is a self-contained ASP.NET Core application. LiteDB is embedded — no database server setup is required.

```bash
cd FormFlow.Backend
dotnet run
```

On first run, the backend will automatically seed the database with 10 sample questions from `SeedData/questions.json`.

**Expected output:**

```
Building...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7209
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5164
```

**Verify the API is running:**

Open your browser or use curl to check:

```bash
curl http://localhost:5164/api/questions
```

You should receive a JSON array of question objects.

> **Note:** If your browser warns about the HTTPS certificate on `localhost:7209`, you can either trust the dev certificate (`dotnet dev-certs https --trust`) or use the HTTP URL `http://localhost:5164` for development.

Leave this terminal open. The backend must be running for the Blazor and React clients.

---

### 3 — Run the Blazor Web UI

Open a **new terminal** in the repository root:

```bash
cd FormFlow.Blazor
dotnet run
```

**Expected output:**

```
Building...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7230
```

Open `https://localhost:7230` in your browser. You should see the FormFlow home page with a rendered list of questions. Navigate to `/admin/questions` to view and manage questions through the admin panel.

---

### 4 — Run the React SPA

Open a **new terminal** in the repository root:

```bash
cd FormFlow.React
npm install
npm start
```

**Expected output:**

```
Compiled successfully!

You can now view react-app in the browser.

  Local:            http://localhost:3000
```

Your browser should open automatically to `http://localhost:3000`. The app loads question definitions and renders them.

---

### 5 — Run the Tests

#### Backend and Blazor tests (.NET)

From the repository root:

```bash
dotnet test
```

This runs all four .NET test projects at once: `FormFlow.Backend.Tests`, `FormFlow.Data.Tests`, `FormFlow.Blazor.Tests`, and a summary is printed to the console.

To run a single project:

```bash
dotnet test FormFlow.Backend.Tests
dotnet test FormFlow.Data.Tests
dotnet test FormFlow.Blazor.Tests
```

#### React tests (Jest)

```bash
cd FormFlow.React.Tests
npm install
npm test
```

Press `a` in the Jest watch menu to run all tests. Press `q` to quit.

To run a single pass (useful in CI):

```bash
npm test -- --watchAll=false
```

---

## API Reference

Base URL (HTTP): `http://localhost:5164`  
Base URL (HTTPS): `https://localhost:7209`

| Method | Path | Description |
|--------|------|-------------|
| `GET` | `/api/questions` | Return all question definitions |
| `GET` | `/api/questions/{id}` | Return a single question by GUID |
| `POST` | `/api/questions` | Create a new question |

See [docs/api.md](docs/api.md) for full request/response examples, status code descriptions, and field-level documentation.

---

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) and [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md) before submitting pull requests.

---

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.
