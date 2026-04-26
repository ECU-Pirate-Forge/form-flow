# Database Reference

## Overview

FormFlow uses LiteDB as its embedded document database. The database file is `formflow.db` and is accessed through repository interfaces registered in dependency injection.

---

## Setup (`Program.cs`)

LiteDB is registered as a singleton using a factory function that reads the connection string from `appsettings.json`:

```csharp
builder.Services.AddSingleton<ILiteDatabase>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("LiteDb");
    return new LiteDatabase(connectionString);
});
```

Using a singleton ensures a single shared database connection for the lifetime of the application, which is the recommended pattern for LiteDB.

---

## Repositories

Repositories are the only way backend code should interact with the database. They hide LiteDB implementation details behind an interface, so endpoints and services depend on the contract, not the storage engine.

### QuestionRepository

Provides access to the `questions` collection.

**Interface:** `IQuestionRepository`
**Implementation:** `QuestionRepository`
**Collection type:** `ILiteCollection<QuestionDefinition>`
**Collection name:** `questions`

#### Methods

| Method | Description |
|---|---|
| `Insert(QuestionDefinition question)` | Inserts a new question and returns it |
| `FindById(Guid id)` | Returns a single question by its ID, or null if not found |
| `FindAll()` | Returns all questions in the collection |
| `FindOne(Expression<Func<QuestionDefinition, bool>> predicate)` | Returns the first question matching a predicate, or null |

#### Notes

- `Id` is indexed on startup via `EnsureIndex` for efficient lookups.

#### Registration

```csharp
builder.Services.AddSingleton<IQuestionRepository, QuestionRepository>();
```

#### Example usage in an endpoint

```csharp
app.MapGet("/api/questions/{id}", (string id, IQuestionRepository repository) =>
{
    if (!Guid.TryParse(id, out var parsedId))
        return Results.BadRequest();

    var question = repository.FindById(parsedId);
    return question is null ? Results.NotFound() : Results.Json(question);
});
```

---

### SurveyRepository

The SurveyRepository provides access to the LiteDB "surveys" collection.
It exposes a strongly typed `ILiteCollection<SurveyDefinition>` and is registered
in dependency injection as `ISurveyRepository`. This allows endpoints and services
to store and retrieve survey documents in a consistent and structured way.