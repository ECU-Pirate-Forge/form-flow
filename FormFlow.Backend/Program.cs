using FormFlow.Backend.Endpoints;
using FormFlow.Data.Models;
using FormFlow.Data.Services;

using LiteDB;



var builder = WebApplication.CreateBuilder(args);

// Register LiteDB as a shared service
builder.Services.AddSingleton<ILiteDatabase>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("LiteDb") ?? "Filename=formflow.db;Connection=shared";
    return new LiteDatabase(connectionString);
});

// Register the QuestionInserter service
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