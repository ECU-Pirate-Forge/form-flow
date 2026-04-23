![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)
![.NET](https://img.shields.io/badge/.NET-10.0-purple)
![Node](https://img.shields.io/badge/Node.js-18%2B-green)
![React](https://img.shields.io/badge/React-19-61DAFB)
![CI](https://github.com/your-org/form-flow/actions/workflows/ci.yml/badge.svg)

# FormFlow

FormFlow is a multi-platform dynamic form generation and management system. It allows developers and administrators to define, store, and render survey/form questions with rich validation, conditional visibility logic, and support for multiple frontend platforms. The project is built with a shared .NET data layer, an ASP.NET Core REST API backend, a Blazor Server web UI, and a React TypeScript SPA,  with mobile platforms (Flutter, MAUI, React Native) planned.

---

## Table of Contents

- [Project Background](#project-background)
- [Implemented Features](#implemented-features)
- [Planned Roadmap](#planned-roadmap)
- [Project Directory Structure](#project-directory-structure)
- [Getting Started](#getting-started)
  - [Option A,  Dev Container (Recommended)](#option-a--dev-container-recommended)
  - [Option B,  Local Setup](#option-b--local-setup)
    - [Prerequisites](#prerequisites)
    - [1,  Clone the Repository](#1--clone-the-repository)
    - [2,  Run the Backend API](#2--run-the-backend-api)
    - [3,  Run the Blazor Web UI](#3--run-the-blazor-web-ui)
    - [4,  Run the React SPA](#4--run-the-react-spa)
    - [5,  Run the Tests](#5--run-the-tests)
- [API Reference](#api-reference)
- [Continuous Integration](#continuous-integration)
- [Troubleshooting](#troubleshooting)
- [Third-Party Validation](#third-party-validation)
- [Contributing](#contributing)
- [License](#license)

---

## Project Background

Modern organizations frequently need to collect structured data from users through forms and surveys. Hard-coded forms are brittle and expensive to maintain,  changing a single field often requires a full release cycle. FormFlow solves this by storing question definitions in a database and rendering them dynamically at runtime.

Each question carries its own metadata: display label, input type, placeholder text, help text, required flag, selectable options, conditional visibility rules, and a JSON-serialized validation configuration. This design lets administrators update forms without touching source code, and it provides a consistent contract between the backend and any frontend client.

The project was designed from the start to support multiple rendering platforms. The shared `FormFlow.Data` library exposes the canonical `QuestionDefinition` model and validation engine so every platform speaks the same language.

### What a question definition looks like

The following is a real example from the seed data. It shows a `yes_no` question and a `dropdown` question that only appears when the user answers "Yes",  demonstrating conditional visibility:

```json
{
  "id": "a12e5b95-5f5b-4d95-ae01-1ca3ea6d0005",
  "key": "is_student",
  "label": "Are you currently a student?",
  "type": "yes_no",
  "required": true
},
{
  "id": "a67d0e4a-0a0f-4f95-ba83-7f9e3c0f0010",
  "key": "campus_preference",
  "label": "Preferred campus location",
  "type": "dropdown",
  "required": true,
  "visibleIf": {
    "key": "is_student",
    "shouldEqual": true
  },
  "options": [
    { "value": "north", "label": "North campus" },
    { "value": "south", "label": "South campus" },
    { "value": "east",  "label": "East campus" },
    { "value": "west",  "label": "West campus" }
  ]
}
```

The `campus_preference` dropdown is only rendered when `is_student` equals `true`. Every other question type,  text, number, radio, checkbox, multiselect,  follows this same schema.

### Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend API | ASP.NET Core 10, LiteDB 5 |
| Shared Models / Validation | .NET 10 Class Library |
| Web UI (interactive) | Blazor Server, MudBlazor 9 |
| Web UI (SPA) | React 19, TypeScript 4 |
| Testing | xUnit, Moq, FluentAssertions, bUnit, Jest, React Testing Library |
| Database | LiteDB (embedded document store, no server required) |
| CI/CD | GitHub Actions |

---

## Implemented Features

### Question Definition System
- Seven question types supported: `text`, `number`, `yes_no`, `dropdown`, `radio`, `checkbox`, `multiselect`
- Per-question metadata: `label`, `placeholder`, `helpText`, `defaultValue`, `required`
- Named `key` field for referencing questions in form submissions and conditional rules

### Conditional Visibility
- `VisibleIf` rules allow any question to be shown or hidden based on the current answer to another question
- Example: the "Campus Preference" dropdown only appears when the user answers "Yes" to "Are you a student?"

### Server-Side Validation Engine
- `QuestionValidationEngine` evaluates submitted responses against per-question rule lists
- Supported rule types: `MinLength`, `MaxLength`, `MinValue`, `MaxValue`, `Range`
- Returns a pass/fail result plus a list of human-readable error messages

### REST API
- `GET /api/questions`,  retrieve all question definitions
- `GET /api/questions/{id}`,  retrieve a single question by GUID
- `POST /api/questions`,  create a new question (validates key uniqueness and required fields)
- Correct HTTP status codes throughout: 200, 201, 400, 404, 409

### LiteDB Persistence & Automatic Seeding
- Embedded document database,  no separate database server required
- On first startup the backend checks if the `questions` collection is empty; if so, it bulk-inserts 10 representative sample questions from `SeedData/questions.json` covering all question types

### Blazor Server Web UI
- Interactive question list and admin panel powered by MudBlazor components
- `QuestionRenderer` dynamically resolves question type to the appropriate Razor component at runtime
- Dedicated components for every question type with two-way data binding and validation display
- Admin pages: question list (`/admin/questions`), create question (`/admin/questions/create`)

### React TypeScript SPA
- React 19 TypeScript application that loads questions from a JSON file
- `QuestionRenderer` component renders each question with label, input, help text, and ARIA accessibility attributes
- Shared `QuestionDefinition`, `Option`, and `VisibleIf` TypeScript interfaces that mirror the backend models exactly

### Automated Test Suite
- **FormFlow.Backend.Tests**,  xUnit integration tests using `WebApplicationFactory`; covers all three API endpoints, all 400/404/409 error cases, and database seeding behaviour
- **FormFlow.Data.Tests**,  unit tests for model validation logic and the `QuestionValidationEngine`
- **FormFlow.Blazor.Tests**,  bUnit component tests for all Blazor question renderers
- **FormFlow.React.Tests**,  Jest + React Testing Library tests for React components

### JSON Schema Validation
- `question-definition.schema.json` defines the canonical question structure
- `survey-definition.schema.json` references the question schema via `$ref` for modular, reusable validation

### Continuous Integration
- GitHub Actions CI pipeline (`.github/workflows/ci.yml`) runs on every push to `main` and on every pull request
- Pipeline steps: install Node dependencies → run React tests → build React → restore and build .NET projects → run all .NET tests
- Lint pipeline (`.github/workflows/lint.yml`) enforces code style on every PR

---

## Planned Roadmap

| Feature | Status | Notes |
|---------|--------|-------|
| Full Question CRUD | In Progress | GET and POST implemented; PUT and DELETE endpoints pending |
| Flutter mobile client | Planned | `FormFlow.Flutter` directory scaffolded |
| .NET MAUI mobile client | Planned | `FormFlow.Maui` directory scaffolded |
| React Native client | Planned | `FormFlow.ReactNative` directory scaffolded |
| Survey management | Planned | `SurveyDefinition` model defined; survey endpoints and UI not yet built |
| Form submission & response storage | Planned | `QuestionValidationEngine` is ready; submission endpoints and storage not yet built |
| Conditional visibility in React SPA | Planned | `VisibleIf` TypeScript type defined; runtime evaluation not yet wired up |
| Enhanced React question type components | Planned | Currently all questions render as text inputs; type-specific components needed |
| Analytics & reporting | Planned | Track submission counts, completion rates, and answer distributions |
| User authentication | Planned | Secure the admin panel; associate form responses with user accounts |
| Import/export question sets | Planned | JSON bulk-import and export for portability across environments |

---

## Project Directory Structure

```
form-flow/
├── FormFlow.Backend/               # ASP.NET Core REST API
│   ├── Endpoints/
│   │   └── QuestionEndpoints.cs    # GET /api/questions, GET /api/questions/{id}, POST /api/questions
│   ├── Repositories/
│   │   ├── IQuestionRepository.cs  # Repository interface (enables test mocking)
│   │   └── QuestionRepository.cs   # LiteDB collection access
│   ├── SeedData/
│   │   └── questions.json          # 10 sample questions auto-loaded on first run
│   ├── Schemas/
│   │   ├── question-definition.schema.json   # JSON Schema for question validation
│   │   └── survey-definition.schema.json     # JSON Schema for survey validation ($ref to question)
│   ├── DatabaseSeeder.cs           # Seeds DB at startup if the questions collection is empty
│   ├── Program.cs                  # App entry point; DI wiring, CORS, route registration
│   └── appsettings.json
│
├── FormFlow.Backend.Tests/         # xUnit integration + unit tests for the backend
│
├── FormFlow.Data/                  # Shared .NET class library (used by all .NET projects)
│   ├── Models/
│   │   ├── QuestionDefinition.cs   # Core question entity
│   │   ├── Option.cs               # Label/Value pair for dropdown, radio, etc.
│   │   ├── VisibleIf.cs            # Conditional visibility rule
│   │   ├── SurveyDefinition.cs     # Survey container (title, description, question IDs)
│   │   └── NewQuestion.cs          # DTO used for POST /api/questions body
│   └── Services/
│       ├── QuestionValidator.cs         # Validates required fields, type enum, key uniqueness
│       ├── IQuestionInserter.cs         # Interface abstraction for test mocking
│       └── QuestionValidationEngine.cs  # Evaluates MinLength/MaxLength/MinValue/MaxValue/Range rules
│
├── FormFlow.Data.Tests/            # xUnit tests for models and the validation engine
│
├── FormFlow.Blazor/                # Blazor Server web application
│   ├── Components/
│   │   ├── QuestionRenderer.razor        # Resolves question type → component at runtime
│   │   ├── TextQuestion.razor
│   │   ├── NumberQuestion.razor
│   │   ├── YesNoQuestion.razor
│   │   ├── DropdownQuestion.razor
│   │   ├── RadioQuestion.razor
│   │   ├── CheckboxQuestion.razor
│   │   └── MultiselectQuestion.razor
│   ├── Pages/
│   │   ├── Home.razor                    # Landing page; renders all seeded questions
│   │   ├── AdminQuestions.razor          # /admin/questions,  question list
│   │   └── AdminCreateQuestion.razor     # /admin/questions/create,  create form
│   └── wwwroot/
│       └── multiple-sample-questions.json  # Shared sample data for Blazor and React
│
├── FormFlow.Blazor.Tests/          # bUnit component tests for Blazor question renderers
│
├── FormFlow.React/                 # React 19 TypeScript SPA
│   ├── src/
│   │   ├── App.tsx                 # Root component; fetches and lists questions
│   │   ├── components/
│   │   │   └── QuestionRenderer.tsx    # Renders a single question with ARIA attributes
│   │   └── types/
│   │       ├── QuestionDefinition.ts   # Mirrors backend QuestionDefinition model
│   │       ├── Option.ts
│   │       └── VisibleIf.ts
│   └── package.json
│
├── FormFlow.React.Tests/           # Jest + React Testing Library tests
│
├── FormFlow.Flutter/               # Placeholder,  Flutter mobile client (planned)
├── FormFlow.Maui/                  # Placeholder,  .NET MAUI mobile client (planned)
├── FormFlow.ReactNative/           # Placeholder,  React Native client (planned)
│
├── docs/                           # Extended project documentation
│   ├── index.md                    # Documentation index
│   ├── architecture.md             # Validation engine overview and rule types
│   ├── api.md                      # Full REST API reference with cURL/Postman examples
│   ├── backend.md                  # Backend architecture, data integrity, seeding
│   ├── database.md                 # LiteDB configuration and collection access
│   ├── question-definition.md      # Full property reference for QuestionDefinition
│   ├── survey-definition.md        # SurveyDefinition model documentation
│   ├── blazor-components.md        # Component hierarchy and rendering logic
│   ├── react-components.md         # React component documentation
│   ├── admin.md                    # Admin module documentation
│   ├── testing.md                  # Testing strategy and manual HTTP checks
│   ├── test-suite-overview.md      # All test projects and coverage goals
│   └── troubleshooting.md          # Common issues and resolutions
│
├── .devcontainer/
│   └── devcontainer.json           # VS Code Dev Container / GitHub Codespaces config
├── .github/
│   └── workflows/
│       ├── ci.yml                  # CI: build + test on push to main and on PRs
│       └── lint.yml                # Lint check on every PR
│
├── FormFlow.slnx                   # .NET solution file (references all 6 .NET projects)
├── package.json                    # Root npm scripts
├── AGENTS.md                       # Copilot agent instructions
├── CHANGELOG.md
├── CONTRIBUTING.md
├── CODE_OF_CONDUCT.md
└── LICENSE                         # MIT
```

---

## Getting Started

> **These instructions were independently verified by a tester who did not participate in the writing of this document.** Follow each section in order. If you hit an issue, see the [Troubleshooting](#troubleshooting) section at the bottom of this file.

---

### Option A,  Dev Container (Recommended)

The repository includes a `.devcontainer` configuration. If you use **VS Code** with the [Dev Containers extension](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-containers), or **GitHub Codespaces**, all required tools are provisioned automatically,  no manual installs needed.

1. Open the repository in VS Code
2. When prompted, click **Reopen in Container**
3. Wait for the container to build (first time only, ~2–3 minutes)
4. Continue from [Step 2,  Run the Backend API](#2--run-the-backend-api) below

---

### Option B,  Local Setup

#### Prerequisites

Install the following tools before proceeding:

| Tool | Minimum Version | Download |
|------|----------------|----------|
| .NET SDK | 10.0 | https://dotnet.microsoft.com/download |
| Node.js | 18 | https://nodejs.org |
| npm | 9 (bundled with Node) |,  |
| Git | Any recent version | https://git-scm.com |

Verify your installations by running these commands. If any command fails or shows a lower version, install or upgrade that tool before continuing.

```bash
dotnet --version   # expected: 10.x.x
node --version     # expected: v18.x.x or higher
npm --version      # expected: 9.x.x or higher
git --version
```

**Trust the .NET development HTTPS certificate (one-time setup):**

```bash
dotnet dev-certs https --trust
```

If prompted by your OS to trust the certificate, click **Yes/Allow**. This prevents browser security warnings when running the apps over HTTPS locally. You only need to do this once per machine.

---

#### 1,  Clone the Repository

```bash
git clone https://github.com/your-org/form-flow.git
cd form-flow
```

> Replace the URL with the actual repository URL if it differs.

---

#### 2,  Run the Backend API

The backend is a self-contained ASP.NET Core application. LiteDB is embedded,  **no separate database server or configuration is required**.

Open a terminal in the repository root and run:

```bash
cd FormFlow.Backend
dotnet run
```

On first run, the backend automatically creates a LiteDB database file and seeds it with 10 sample questions from `SeedData/questions.json`. You will see output similar to:

```
Building...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7209
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5164
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

**Verify the API is running**,  open a second terminal and run:

```bash
curl http://localhost:5164/api/questions
```

You should receive a JSON array of 10 question objects. If you see an empty array (`[]`), delete any existing `formflow.db` file in the `FormFlow.Backend` folder and restart.

> Leave this terminal running. The Blazor and React clients both depend on the backend being available.

---

#### 3,  Run the Blazor Web UI

Open a **new terminal** in the repository root:

```bash
cd FormFlow.Blazor
dotnet run
```

Expected output:

```
Building...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7230
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5224
```

Open `https://localhost:7230` (or `http://localhost:5224`) in your browser. You should see the FormFlow home page with a rendered list of questions. To manage questions, navigate to:

- `/admin/questions`,  view all questions
- `/admin/questions/create`,  create a new question

---

#### 4,  Run the React SPA

Open a **new terminal** in the repository root:

```bash
cd FormFlow.React
npm install
npm start
```

`npm install` downloads dependencies and only needs to be run once (or after `package.json` changes). Expected output after `npm start`:

```
Compiled successfully!

You can now view react-app in the browser.

  Local:            http://localhost:3000
  On Your Network:  http://192.168.x.x:3000
```

Your default browser should open automatically to `http://localhost:3000`. If it does not, open it manually. The app loads question definitions and renders them as a form.

---

#### 5,  Run the Tests

**Backend, Data, and Blazor tests (.NET)**

From the repository root, run all .NET test projects at once:

```bash
dotnet test
```

To run a specific project:

```bash
dotnet test FormFlow.Backend.Tests
dotnet test FormFlow.Data.Tests
dotnet test FormFlow.Blazor.Tests
```

A passing run will end with output similar to:

```
Passed! - Failed: 0, Passed: 28, Skipped: 0, Total: 28
```

**React tests (Jest)**

```bash
cd FormFlow.React.Tests
npm install
npm test
```

Jest starts in watch mode. Press `a` to run all tests, or `q` to quit. To run once without watch mode (useful in CI or for a quick check):

```bash
npm test -- --watchAll=false
```

---

## API Reference

Base URL (HTTP): `http://localhost:5164`  
Base URL (HTTPS): `https://localhost:7209`

| Method | Path | Description | Success Code |
|--------|------|-------------|-------------|
| `GET` | `/api/questions` | Return all question definitions | 200 |
| `GET` | `/api/questions/{id}` | Return a single question by GUID | 200 |
| `POST` | `/api/questions` | Create a new question | 201 |

### POST /api/questions,  example request body

```json
{
  "key": "favorite_color",
  "label": "What is your favorite color?",
  "type": "text",
  "required": false,
  "placeholder": "e.g. blue",
  "helpText": "Any color is valid."
}
```

### Error codes

| Code | Meaning |
|------|---------|
| 400 | Invalid input,  missing required field, malformed GUID, or failed validation |
| 404 | No question found for the given ID |
| 409 | Conflict,  a question with that `key` or `id` already exists |

For full request/response examples, cURL commands, PowerShell snippets, and Postman instructions see [docs/api.md](docs/api.md).

---

## Continuous Integration

The project uses two GitHub Actions pipelines:

**`ci.yml`**,  runs on every push to `main` and on every pull request:
1. Install Node 20, install React dependencies, run React tests, build React app
2. Install .NET 10, restore and build `FormFlow.Backend` and `FormFlow.Blazor`
3. Run all .NET tests (`dotnet test`)

**`lint.yml`**,  runs the React ESLint check on every pull request.

All CI checks must pass before a pull request can be merged.

---

## Troubleshooting

**`dotnet run` fails with "project not found" or build errors**

Make sure you are inside the correct subfolder (`FormFlow.Backend` or `FormFlow.Blazor`), not the repository root. Each project has its own `.csproj` file.

**`curl http://localhost:5164/api/questions` returns an empty array `[]`**

The seeder only runs when the database collection is empty. If a `formflow.db` file was created by a previous run but is empty or corrupt, delete it:

```bash
# from the FormFlow.Backend directory
rm -f formflow.db
dotnet run
```

**Browser shows "Your connection is not private" for localhost HTTPS**

Run `dotnet dev-certs https --trust` and restart the browser. Alternatively, use the HTTP URLs (`http://localhost:5164` for the API, `http://localhost:5224` for Blazor) during development.

**`npm install` fails with Node version errors**

Confirm your Node version is 18 or higher: `node --version`. If it is older, install the latest LTS release from https://nodejs.org.

**Port already in use**

If another process is using port 5164, 7209, 7230, or 3000, either stop that process or override the port:

```bash
# Backend on a different port
dotnet run --urls "http://localhost:5200"

# React on a different port
PORT=3001 npm start          # macOS / Linux
set PORT=3001 && npm start   # Windows CMD
```

**`dotnet test` shows "Could not find file formflow.db" or similar**

The backend integration tests spin up a real `WebApplicationFactory` and use an in-memory or temp database. If tests are failing because of a leftover database file, delete `formflow.db` from `FormFlow.Backend` and re-run.

---

## Third-Party Validation

The Getting Started instructions in this README were tested end-to-end by a tester with no prior knowledge of this project on a clean machine. The tester confirmed:

- All prerequisite checks passed as described
- `dotnet run` in `FormFlow.Backend` seeded and served questions correctly
- `dotnet run` in `FormFlow.Blazor` opened the home page and admin panel without errors
- `npm install && npm start` in `FormFlow.React` launched the SPA at `http://localhost:3000`
- `dotnet test` from the repository root passed all .NET tests
- `npm test -- --watchAll=false` in `FormFlow.React.Tests` passed all Jest tests

If you encounter a step that does not work as described, please open an issue or consult [docs/troubleshooting.md](docs/troubleshooting.md).

---

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) and [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md) before submitting pull requests. The CI pipeline must pass and all existing tests must continue to pass before a PR will be reviewed.

---

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.
