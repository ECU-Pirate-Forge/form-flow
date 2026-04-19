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
        }
    }
}