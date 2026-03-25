# 📘 **FormFlow Test Suite Overview**

This document explains the structure, purpose, and usage of all automated test projects in the FormFlow solution.

It exists to ensure future teammates understand the architecture and do not reintroduce mixed test projects or dependency conflicts.

---

## 🧱 Test Project Structure

```
FormFlow.Backend.Tests/
FormFlow.Blazor.Tests/
FormFlow.Data.Tests/
docs/
    TestSuiteOverview.md   ← this file
```

Each test project is isolated so it only references the dependencies it needs.

---

## 🎯 Purpose of Each Test Project

### **FormFlow.Backend.Tests**

Tests backend API endpoints using `WebApplicationFactory<Program>`.

Covers:

* Minimal API routes
* Status codes
* JSON responses
* Backend logic

### **FormFlow.Blazor.Tests**

Tests Blazor components using **bUnit** +  **MudBlazor** .

Covers:

* Component rendering
* UI behavior
* Multi‑instance rendering via `QuestionRendererLoopHost`
* Component logic

### **FormFlow.Data.Tests**

Tests data models and validation logic.

Covers:

* Schema alignment
* Serialization/deserialization
* Model validation rules

---

## 🧭 When to Add Tests to Each Project

| Type of Test           | Add To                  |
| ---------------------- | ----------------------- |
| API endpoint tests     | **Backend.Tests** |
| HTTP integration tests | **Backend.Tests** |
| Blazor component tests | **Blazor.Tests**  |
| MudBlazor UI tests     | **Blazor.Tests**  |
| Model validation tests | **Data.Tests**    |
| Schema alignment tests | **Data.Tests**    |

---

## ▶️ How to Run Tests

### **Using Visual Studio Test Explorer**

* Open **Test Explorer**
* Click **Run All Tests**
* View results grouped by project

### **Using the .NET CLI**

From the solution root:

```bash
dotnet test
```

To run a specific project:

```bash
dotnet test FormFlow.Backend.Tests
dotnet test FormFlow.Blazor.Tests
dotnet test FormFlow.Data.Tests
```

---

## 🏗️ How to Add New Test Projects

1. Create a new folder at the root:
   ```
   FormFlow.NewArea.Tests/
   ```
2. Create the project:
   ```bash
   dotnet new xunit -n FormFlow.NewArea.Tests -o FormFlow.NewArea.Tests
   ```
3. Add it to the solution:
   ```bash
   dotnet sln add FormFlow.NewArea.Tests/FormFlow.NewArea.Tests.csproj
   ```
4. Add the correct project references
5. Add the correct NuGet packages (see below)

---

## ⚙️ SDK Selection Rules

| Project Type           | SDK                         |
| ---------------------- | --------------------------- |
| Backend tests          | `Microsoft.NET.Sdk`       |
| Data tests             | `Microsoft.NET.Sdk`       |
| Blazor component tests | `Microsoft.NET.Sdk.Razor` |

---

## 📦 Required NuGet Packages

### **Backend Tests**

* xunit
* xunit.runner.visualstudio
* FluentAssertions
* Microsoft.NET.Test.Sdk
* coverlet.collector
* Microsoft.AspNetCore.Mvc.Testing

### **Blazor Tests**

* bunit
* bunit.mudblazor
* MudBlazor.Services
* xunit
* FluentAssertions
* Microsoft.NET.Test.Sdk
* coverlet.collector

### **Data Tests**

* xunit
* FluentAssertions
* Microsoft.NET.Test.Sdk
* coverlet.collector

---
