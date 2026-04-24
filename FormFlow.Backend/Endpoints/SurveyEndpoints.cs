using FormFlow.Data.Models;
using FormFlow.Backend.Repositories;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

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

            app.MapPost("/api/surveys", (NewSurvey dto, ISurveyRepository repo) =>
            {
                if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Description))
                {
                    return Results.BadRequest(new { error = "Title is required. "});
                }

                if (dto.QuestionIds == null || dto.QuestionIds.Count == 0)
                {
                    return Results.BadRequest(new { error = "At least one question is required. "});
                }

                var survey = new SurveyDefinition
                {
                    Id = Guid.NewGuid(),
                    Title = dto.Title,
                    Description = dto.Description,
                    QuestionIds = dto.QuestionIds,
                    CreatedAt = DateTime.UtcNow
                };

                repo.Insert(survey);

                return Results.Created($"/api/surveys/{survey.Id}", survey);
            })
            .WithName("CreateSurvey")
            .Produces<SurveyDefinition>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);
        }
    }
}