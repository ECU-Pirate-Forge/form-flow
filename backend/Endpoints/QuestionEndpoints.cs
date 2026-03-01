using database.models;
using database.services;
using System.Text.Json;

namespace backend.Endpoints
{
    /// <summary>
    /// API endpoints for managing questions with validation
    /// </summary>
    public static class QuestionEndpoints
    {
        public static void MapQuestionEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/questions")
                .WithName("Questions")
                .WithOpenApi();

            // POST: Submit a new question (with validation)
            // create route builder first so we keep the concrete type
            var submitBuilder = (RouteHandlerBuilder)group.MapPost("/", SubmitQuestion);

            submitBuilder.WithName("SubmitQuestion");
            submitBuilder.WithDescription("Submit a new question with schema validation");

            submitBuilder.Produces<SubmitQuestionResponse>(StatusCodes.Status201Created);
            submitBuilder.Produces<ErrorResponse>(StatusCodes.Status400BadRequest);
            submitBuilder.Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);

            // POST: Submit question from JSON body (raw JSON)
            var validateBuilder = (RouteHandlerBuilder)group.MapPost("/validate", ValidateQuestion);

            validateBuilder.WithName("ValidateQuestion");
            validateBuilder.WithDescription("Validate a question without inserting it");

            validateBuilder.Produces<ValidateQuestionResponse>(StatusCodes.Status200OK);
            validateBuilder.Produces<ErrorResponse>(StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Submits and inserts a new question with validation
        /// </summary>
        private static async Task<IResult> SubmitQuestion(HttpContext context)
        {
            try
            {
                // read raw body as string
                using var reader = new StreamReader(context.Request.Body);
                var jsonString = await reader.ReadToEndAsync();

                var inserter = new QuestionInserter();
                var result = inserter.InsertQuestionFromJson(jsonString);

                if (!result.Success)
                {
                    return Results.BadRequest(new ErrorResponse
                    {
                        Error = "Validation Failed",
                        Message = result.Message,
                        Details = result.ValidationResult?.Errors.Select(e => new ErrorDetail
                        {
                            Field = e.Field,
                            Message = e.Message
                        }).ToList()
                    });
                }

                return Results.Created(
                    $"/api/questions/{result.QuestionId}",
                    new SubmitQuestionResponse
                    {
                        Success = true,
                        Message = result.Message,
                        QuestionId = result.QuestionId
                    }
                );
            }
            catch (Exception ex)
            {
                // return an error object with explicit status using Json helper
                return Results.Json(new ErrorResponse
                {
                    Error = "Server Error",
                    Message = ex.Message
                }, statusCode: 500);
            }
        }

        /// <summary>
        /// Validates a question without inserting it into the database
        /// </summary>
        private static async Task<IResult> ValidateQuestion(HttpContext context)
        {
            try
            {
                using var reader = new StreamReader(context.Request.Body);
                var jsonString = await reader.ReadToEndAsync();

                var validator = new QuestionValidator();
                var result = validator.ValidateJson(jsonString);

                if (!result.Valid)
                {
                    return Results.BadRequest(new ValidateQuestionResponse
                    {
                        Valid = false,
                        Errors = result.Errors.Select(e => new ErrorDetail
                        {
                            Field = e.Field,
                            Message = e.Message
                        }).ToList()
                    });
                }

                return Results.Ok(new ValidateQuestionResponse
                {
                    Valid = true,
                    Message = "Question is valid"
                });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new ValidateQuestionResponse
                {
                    Valid = false,
                    Message = $"Error during validation: {ex.Message}"
                });
            }
        }
    }

    /// <summary>
    /// Standard error response format
    /// </summary>
    public class ErrorResponse
    {
        public string Error { get; set; }
        public string Message { get; set; }
        public List<ErrorDetail> Details { get; set; }
    }

    /// <summary>
    /// Individual error detail
    /// </summary>
    public class ErrorDetail
    {
        public string Field { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    /// Response for question submission
    /// </summary>
    public class SubmitQuestionResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Guid? QuestionId { get; set; }
    }

    /// <summary>
    /// Response for question validation
    /// </summary>
    public class ValidateQuestionResponse
    {
        public bool Valid { get; set; }
        public string Message { get; set; }
        public List<ErrorDetail> Errors { get; set; }
    }
}
