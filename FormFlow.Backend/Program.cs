using FormFlow.Backend.Models;
using FormFlow.Backend.Repositories;
using FormFlow.Backend.Endpoints;
using FormFlow.Data.Services;
using LiteDB;
using FormFlow.Data.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ILiteDatabase>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var databasePath = configuration.GetValue<string>("LiteDb:DatabasePath") ?? "formflow.db";
    return new LiteDatabase(databasePath);
});
builder.Services.AddSingleton<IFormResponseRepository, LiteDbFormResponseRepository>();

builder.Services.AddSingleton<IQuestionInserter, QuestionInserter>();

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// register inserter so endpoints can depend on it (facilitates testing)
builder.Services.AddSingleton<IQuestionInserter, QuestionInserter>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Map question endpoints
app.MapQuestionEndpoints();

app.MapPost("/api/responses", async (FormResponse request, IFormResponseRepository repository, CancellationToken cancellationToken) =>
{
    var validationErrors = ValidateFormResponse(request);
    if (validationErrors.Count > 0)
    {
        return Results.ValidationProblem(validationErrors);
    }

    var createdResponse = await repository.SaveAsync(request, cancellationToken);
    return Results.Created($"/api/responses/{createdResponse.Id}", createdResponse);
})
.WithName("CreateFormResponse");

app.MapGet("/api/responses/{id}", async (string id, IFormResponseRepository repository, CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(id))
    {
        return Results.BadRequest(new { message = "The id route parameter is required." });
    }

    var response = await repository.GetByIdAsync(id, cancellationToken);
    return response is null ? Results.NotFound() : Results.Ok(response);
})
.WithName("GetFormResponseById");

app.Run();

static Dictionary<string, string[]> ValidateFormResponse(FormResponse response)
{
    var errors = new Dictionary<string, string[]>();

    if (string.IsNullOrWhiteSpace(response.FormId))
    {
        errors["formId"] = ["The formId field is required."];
    }

    if (response.Answers is null || response.Answers.Count == 0)
    {
        errors["answers"] = ["At least one answer is required."];
    }
    else if (response.Answers.Keys.Any(string.IsNullOrWhiteSpace))
    {
        errors["answers"] = ["Answer keys cannot be null, empty, or whitespace."];
    }

    return errors;
}
