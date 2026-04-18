using FormFlow.Data.Models;
using FormFlow.Backend.Templates;
using FormFlow.Backend.Repositories;
using System;
using System.Security.Cryptography.X509Certificates;

namespace FormFlow.Backend.Endpoints
{

    public class QuestionValidation
    {
        public QuestionValidator Validator { get; } = new QuestionValidator();
    }

    public static class QuestionEndpoints
    {

        private static readonly QuestionValidation _validation = new QuestionValidation();
        public static void MapQuestionEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/questions", async (QuestionDefinition question, IQuestionRepository repository) =>
            {
                // Validate the question object
                var validationResult = _validation.Validator.Validate(question);
                if (!validationResult.Valid)
                {
                    return Results.BadRequest(new { errors = validationResult.Errors.Select(e => e.Message) });
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

                // Insert using repository
                


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