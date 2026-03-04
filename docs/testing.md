# Testing Backend Endpoints & JSON Validation

This document explains how to verify that the backend accepts only well‑formed, schema‑compliant question
objects and rejects invalid JSON before anything is written to the database.

---

## 🧪 Automated Tests

We add two kinds of tests in the `backend/tests` project:

1. **Unit tests** for the `QuestionValidator` class.  These confirm that the validator returns
   errors for null/empty input and handles JSON parsing problems.
2. **Integration tests** using `WebApplicationFactory` and `Moq` to exercise the HTTP API.
   The endpoint is wired with dependency injection so we can substitute a fake
   `IQuestionInserter` (which encapsulates validation + LiteDB logic) and assert
   on the HTTP response.

### Running the tests

```bash
cd backend/tests
# build & execute all tests
dotnet test
```

⚠️ The `backend` folder contains a `tests` sub‑folder.  The main `backend.csproj` is
configured to **exclude** `tests/**` so the test files don't get compiled into
production binaries.

### Example tests

#### Validator unit:
```csharp
[Fact]
public void Validate_NullQuestion_ReturnsInvalid() { ... }
```

#### Endpoint integration:
```csharp
[Fact]
public async Task Post_InvalidQuestion_ReturnsBadRequest()
{
    var mock = new Mock<IQuestionInserter>();
    mock.Setup(x => x.InsertQuestionFromJson(It.IsAny<string>()))
        .Returns(new QuestionInserter.InsertResult { Success = false, Message = "invalid" });

    var client = _factory.WithWebHostBuilder(builder =>
    {
        builder.ConfigureServices(s => s.AddSingleton(mock.Object));
    }).CreateClient();

    var response = await client.PostAsync("/api/questions", new StringContent("{\"label\":\"x\"}", Encoding.UTF8, "application/json"));
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    mock.Verify(x => x.InsertQuestionFromJson(It.IsAny<string>()), Times.Once);
}
```

Because the inserter is a mock, the database is never touched during the test, but
we still catch the `400 Bad Request` behavior when validation fails.

---

## 🔍 Manual HTTP Checks

When running the backend locally (e.g. `dotnet run --urls "http://localhost:5000"`),
you can exercise the endpoints using `curl` or Postman.

### Invalid payload
```bash
curl -i -X POST http://localhost:5000/api/questions \
  -H "Content-Type: application/json" \
  -d '{"label":"foo"}'
```
Expected response status **400** with a JSON body showing validation errors.

### Valid payload
```bash
curl -i -X POST http://localhost:5000/api/questions \
  -H "Content-Type: application/json" \
  -d '{
        "id":"<some-uuid>",
        "key":"q1",
        "label":"Your name",
        "type":"text",
        "required":true
      }'
```
Should return **201 Created** and a `Location` header pointing at `/api/questions/{id}`.

> In both cases the insertion logic only runs when validation succeeds.

### Validation-only endpoint
You can also POST to `/api/questions/validate` if you just want to check JSON
without touching the database.

---

## ✅ How validation works

1. `QuestionEndpoints` reads the raw request body and forwards it to
   `QuestionInserter` (via `InsertQuestionFromJson`).
2. `QuestionInserter` uses `QuestionValidator` which in turn calls a Node.js script
   (located under `backend/data/schemas/validation`) to run a JSON schema check.
3. If the validator reports `Valid == false`, the inserter returns a failure result
   and the endpoint responds with 400.
4. Only when `Success == true` does the code open LiteDB and insert the document.

The integration test above exercises the full path while mocking the inserter, so
no real database is touched during CI. The mock is configured to simulate both
success and failure cases.

---

## ⚠️ Tips & Troubleshooting

* If you add new properties to the `Question` model, update the JSON schema in
  `backend/data/schemas/validation/questionValidation.js` and add a regression test.
* Use `dotnet test` frequently; the CI pipeline should run these automatically.
* When debugging 500 errors in tests, inspect the response body (our sample tests
  print it out via `Console.WriteLine`).

With this setup you're confident that invalid JSON never reaches the database and
have both fast unit coverage and realistic endpoint integration tests.