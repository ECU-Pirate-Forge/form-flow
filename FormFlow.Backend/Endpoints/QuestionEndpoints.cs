using FormFlow.Data.Models;
using FormFlow.Backend.Templates;
using FormFlow.Backend.Repositories;
using System;

namespace FormFlow.Backend.Endpoints
{
    public static class QuestionEndpoints
    {
        public static void MapQuestionEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/questions", async (QuestionDefinition question, IQuestionRepository repository) =>
            {
                // Basic validation
                if (question == null)
                {
                    return Results.BadRequest("Question definition is required");
                }

                if (string.IsNullOrWhiteSpace(question.Key))
                {
                    return Results.BadRequest("Question key is required");
                }

                if (string.IsNullOrWhiteSpace(question.Label))
                {
                    return Results.BadRequest("Question label is required");
                }

                if (string.IsNullOrWhiteSpace(question.Type))
                {
                    return Results.BadRequest("Question type is required");
                }

                // Check if key is unique
                var existingQuestion = repository.Questions.FindOne(q => q.Key == question.Key);
                if (existingQuestion != null)
                {
                    return Results.Conflict($"A question with key '{question.Key}' already exists");
                }

                // Ensure unique id
                if (question.Id == Guid.Empty)
                {
                    question.Id = Guid.NewGuid();
                }

                // Insert into questions collection
                repository.Questions.Insert(question);

                // Return 201 Created
                return Results.Created($"/api/questions/{question.Id}", question);
            })
            .WithName("CreateQuestion")
            .Produces<QuestionDefinition>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict);

            app.MapGet("/api/questions/template", () =>
            {
                var question = SingleQuestionTemplate.Get();
                return Results.Json(question);
            })
            .WithName("GetQuestionTemplate")
            .Produces<QuestionDefinition>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status503ServiceUnavailable);
        }
    }
}