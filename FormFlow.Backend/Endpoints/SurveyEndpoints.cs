using FormFlow.Data.Models;
using FormFlow.Backend.Repositories;

namespace FormFlow.Backend.Endpoints
{
    public static class SurveyEndpoints
    {
        public static void MapSurveyEndpoints(this IEndpointRouteBuilder app)
        {
            // GET All Surveys
            app.MapGet("/api/surveys", (ISurveyRepository repo) =>
            {
                var surveys = repo.Surveys.FindAll().ToList();
                return Results.Json(surveys);
            })
            .WithName("GetAllSurveys")
            .Produces<List<SurveyDefinition>>(StatusCodes.Status200OK);

            // GET survey by id
            app.MapGet("/api/surveys/{id}", (string id, ISurveyRepository repo) =>
            {
                if (!Guid.TryParse(id, out var parsedId))
                {
                    return Results.BadRequest(new
                    {
                        error = "Invalid survey id. Must be a GUID."
                    });
                }

                var survey = repo.Surveys.FindById(parsedId);

                if (survey is null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(survey);
            })
            .WithName("GetSurveyById")
            .Produces<SurveyDefinition>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
        }
    }
}