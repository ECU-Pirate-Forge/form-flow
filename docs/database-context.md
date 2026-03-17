# LiteDbContext: Injecting and Using the Shared Database Instance

This guide explains how to use `ILiteDbContext` to access the database in your backend services and repositories.

## Overview

`ILiteDbContext` is a centralized interface that provides access to all database collections. It:
- **Centralizes** collection setup in one place
- **Hides** implementation details (magic strings, indexes)
- **Exposes** typed properties for each collection
- **Simplifies** dependency injection

## Basic Usage: Injecting the Context

### In a Repository

```csharp
using FormFlow.Backend.Data;
using FormFlow.Backend.Models;
using LiteDB;

namespace FormFlow.Backend.Repositories;

public class LiteDbFormResponseRepository : IFormResponseRepository
{
    private readonly ILiteCollection<FormResponse> collection;

    // Inject ILiteDbContext, not ILiteDatabase
    public LiteDbFormResponseRepository(ILiteDbContext context)
    {
        collection = context.FormResponses;
    }

    public Task<FormResponse> SaveAsync(FormResponse response, CancellationToken cancellationToken = default)
    {
        response.Id = Guid.NewGuid().ToString("N");
        response.SubmittedAtUtc = DateTime.UtcNow;
        collection.Insert(response);
        return Task.FromResult(response);
    }

    public Task<FormResponse?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        FormResponse? response = collection.FindById(id);
        return Task.FromResult<FormResponse?>(response);
    }
}
```

### In a Service

```csharp
using FormFlow.Backend.Data;

namespace FormFlow.Backend.Services;

public class FormResponseService
{
    private readonly ILiteDbContext _context;

    public FormResponseService(ILiteDbContext context)
    {
        _context = context;
    }

    public IEnumerable<FormResponse> GetResponsesByFormId(string formId)
    {
        return _context.FormResponses
            .Find(r => r.FormId == formId)
            .ToList();
    }
}
```

## Registering the Context in Program.cs

The context is already registered in `Program.cs`:

```csharp
builder.Services.AddSingleton<ILiteDbContext, LiteDbContext>();
```

This means any class that injects `ILiteDbContext` will receive the same instance (singleton pattern).

## Adding a New Collection

When you add a new entity type, follow these steps:

### Step 1: Create the Model
```csharp
// FormFlow.Backend/Models/Question.cs
namespace FormFlow.Backend.Models;

public class Question
{
    public string? Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}
```

### Step 2: Update ILiteDbContext Interface
```csharp
// FormFlow.Backend/Data/LiteDbContext.cs
public interface ILiteDbContext
{
    ILiteCollection<FormResponse> FormResponses { get; }
    ILiteCollection<Question> Questions { get; }  // ← Add this
}
```

### Step 3: Implement the Property in LiteDbContext
```csharp
public class LiteDbContext : ILiteDbContext
{
    public ILiteCollection<FormResponse> FormResponses { get; }
    public ILiteCollection<Question> Questions { get; }  // ← Add this

    public LiteDbContext(ILiteDatabase database)
    {
        FormResponses = database.GetCollection<FormResponse>("responses");
        FormResponses.EnsureIndex(r => r.FormId);
        FormResponses.EnsureIndex(r => r.SubmittedAtUtc);

        Questions = database.GetCollection<Question>("questions");  // ← Add setup
        Questions.EnsureIndex(q => q.Title);
    }
}
```

### Step 4: Use in Your Repository
```csharp
public class LiteDbQuestionRepository : IQuestionRepository
{
    private readonly ILiteCollection<Question> collection;

    public LiteDbQuestionRepository(ILiteDbContext context)
    {
        collection = context.Questions;
    }

    // Implement query methods...
}
```

## Key Benefits

- **No Magic Strings**: Use `context.FormResponses` instead of `database.GetCollection<FormResponse>("responses")`
- **Centralized Setup**: All collection initialization in one file
- **Consistent Access Pattern**: All collections accessed the same way
- **Easy to Test**: Mock `ILiteDbContext` instead of `ILiteDatabase`
- **Scalable**: Adding new collections is straightforward

## Example: Querying Collections

```csharp
public class ReportService
{
    private readonly ILiteDbContext _context;

    public ReportService(ILiteDbContext context)
    {
        _context = context;
    }

    public int GetResponseCountForForm(string formId)
    {
        return _context.FormResponses
            .Find(r => r.FormId == formId)
            .Count();
    }

    public List<FormResponse> GetRecentResponses(int days)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-days);
        return _context.FormResponses
            .Find(r => r.SubmittedAtUtc >= cutoffDate)
            .OrderByDescending(r => r.SubmittedAtUtc)
            .ToList();
    }
}
```

## Best Practices

1. **Always inject `ILiteDbContext`**, not `ILiteDatabase`
2. **Add indexes** for columns you query frequently (done in `LiteDbContext` constructor)
3. **Keep collection names consistent** — Define them once in `LiteDbContext`
4. **Extract repeated queries** into services or repository methods
5. **Test with a mock context** — Create a fake `ILiteDbContext` for unit tests
