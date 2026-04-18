using FormFlow.Data.Models;
using FormFlow.Backend.Templates;
using FormFlow.Backend.Repositories;


namespace FormFlow.Backend.Endpoints
{
    public static class QuestionEndpoints
    {
        public static void MapQuestionEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/questions", async (QuestionDefinition question, IQuestionRepository repository, QuestionValidator validator) =>
            {
                // Validate the question object
                var validationResult = validator.Validate(question);
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
                else
                {
                    // Check if id is unique
                    var existingById = repository.Questions.FindOne(q => q.Id == question.Id);
                    if (existingById != null)
                    {
                        return Results.Conflict($"A question with id '{question.Id}' already exists");
                    }
                }

                // Insert using repository
                try
                {
                    repository.Questions.Insert(question);
                    return Results.Created($"/api/questions/{question.Id}", question);
                }
                catch(ArgumentNullException)
                {
                    return Results.BadRequest(new { errors = new[] { "Invalid question data provided" } });
                }
                catch (Exception)
                {
                    return Results.StatusCode(StatusCodes.Status500InternalServerError);
                }
                
                


                
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