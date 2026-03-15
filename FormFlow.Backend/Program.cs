using FormFlow.Backend.Models;
using FormFlow.Backend.Repositories;
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.MapQuestionEndpoints();

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