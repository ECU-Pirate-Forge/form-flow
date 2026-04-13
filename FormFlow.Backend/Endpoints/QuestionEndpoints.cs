using FormFlow.Data.Models;
using FormFlow.Backend.Templates;

namespace FormFlow.Backend.Endpoints
{
    public static class QuestionEndpoints
    {
        public static void MapQuestionEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/questions", () =>
            {
                // basic validation
                //ensure unique id/key
                //insert into questions collection
                // return 201 response
            });

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