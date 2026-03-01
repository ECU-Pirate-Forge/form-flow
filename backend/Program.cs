using FormFlow.Backend.Models;
using FormFlow.Backend.Repositories;
using LiteDB;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ILiteDatabase>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var databasePath = configuration.GetValue<string>("LiteDb:DatabasePath") ?? "formflow.db";
    return new LiteDatabase(databasePath);
});
builder.Services.AddSingleton<IFormResponseRepository, LiteDbFormResponseRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

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

public partial class Program
{
}
