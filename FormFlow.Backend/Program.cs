
using FormFlow.Backend.Models;
using FormFlow.Backend.Repositories;
using FormFlow.Backend.Endpoints;
using FormFlow.Backend.Data;
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
builder.Services.AddSingleton<ILiteDbContext, LiteDbContext>();
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


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// temporary service look into IHostService or endpoints
// Initialize the database on startup
_ = app.Services.GetRequiredService<ILiteDbContext>();

app.MapQuestionEndpoints();

app.Run();

