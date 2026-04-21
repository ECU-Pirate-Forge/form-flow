using FormFlow.Backend.Repositories;
using FormFlow.Data.Models;

namespace FormFlow.Backend.Endpoints
{
    public static class QuestionEndpoints
    {
        public static void MapQuestionEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/questions/{id}", (string id, IQuestionRepository repository) =>
            {
                if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out var parsedId))
                {
                    return Results.BadRequest(new
                    {
                        error = "Invalid question id. Provide a non-empty GUID value."
                    });
                }

                var question = repository.Questions.FindById(parsedId);

                if (question is null)
                {
                    return Results.NotFound();
                }

                return Results.Json(question);
            })
            .WithName("GetQuestionById")
            .Produces<QuestionDefinition>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

            app.MapGet("/api/questions",(IQuestionRepository repository) =>
            {
                return Results.Json(repository.Questions.FindAll().ToList());
            })
            .WithName("GetAllQuestions")
            .Produces<List<QuestionDefinition>>(StatusCodes.Status200OK);
            
            app.MapPost("/api/questions", async (QuestionDefinition question, IQuestionRepository repository, QuestionValidator validator) =>
            {
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
                catch (ArgumentNullException)
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


        }
    }
}