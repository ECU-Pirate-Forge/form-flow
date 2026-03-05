using FormFlow.Data.Models;
using FormFlow.Data.Services;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc; // for [FromServices]

namespace FormFlow.Backend.Endpoints
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
            var submitBuilder = group.MapPost("/", async (
                HttpRequest request,
                HttpResponse response,
                IQuestionInserter inserter) =>
            {
                try
                {
                    using var reader = new StreamReader(request.Body);
                    var jsonString = await reader.ReadToEndAsync();

                    var result = inserter.InsertQuestionFromJson(jsonString);
                    if (result == null)
                    {
                        // treat null as failure (typically from a misconfigured mock)
                        response.StatusCode = 400;
                        await response.WriteAsJsonAsync(new ErrorResponse
                        {
                            Error = "Validation Failed",
                            Message = "No result returned from inserter"
                        });
                        return;
                    }

                    if (!result.Success)
                    {
                        response.StatusCode = 400;
                        await response.WriteAsJsonAsync(new ErrorResponse
                        {
                            Error = "Validation Failed",
                            Message = result.Message,
                            Details = result.ValidationResult?.Errors.Select(e => new ErrorDetail
                            {
                                Field = e.Field,
                                Message = e.Message
                            }).ToList()
                        });
                        return;
                    }

                    response.StatusCode = 201;
                    response.Headers.Add("Location", $"/api/questions/{result.QuestionId}");
                    await response.WriteAsJsonAsync(new SubmitQuestionResponse
                    {
                        Success = true,
                        Message = result.Message,
                        QuestionId = result.QuestionId
                    });
                }
                catch (Exception ex)
                {
                    response.StatusCode = 500;
                    await response.WriteAsJsonAsync(new ErrorResponse
                    {
                        Error = "Server Error",
                        Message = ex.Message
                    });
                }
            });

            submitBuilder.WithName("SubmitQuestion");
            submitBuilder.WithDescription("Submit a new question with schema validation");

            submitBuilder.Produces<SubmitQuestionResponse>(StatusCodes.Status201Created);
            submitBuilder.Produces<ErrorResponse>(StatusCodes.Status400BadRequest);
            submitBuilder.Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);

            // POST: Submit question from JSON body (raw JSON)
            var validateBuilder = (RouteHandlerBuilder)group.MapPost("/validate", (Delegate)ValidateQuestion);

            validateBuilder.WithName("ValidateQuestion");
            validateBuilder.WithDescription("Validate a question without inserting it");

            validateBuilder.Produces<ValidateQuestionResponse>(StatusCodes.Status200OK);
            validateBuilder.Produces<ErrorResponse>(StatusCodes.Status400BadRequest);
        }


        /// <summary>
        /// Validates a question without inserting it into the database
        /// </summary>
        private static async Task ValidateQuestion(HttpContext context)
        {
            try
            {
                using var reader = new StreamReader(context.Request.Body);
                var jsonString = await reader.ReadToEndAsync();

                var validator = new QuestionValidator();
                var result = validator.ValidateJson(jsonString);

                if (!result.Valid)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsJsonAsync(new ValidateQuestionResponse
                    {
                        Valid = false,
                        Errors = result.Errors.Select(e => new ErrorDetail
                        {
                            Field = e.Field,
                            Message = e.Message
                        }).ToList()
                    });
                    return;
                }

                context.Response.StatusCode = 200;
                await context.Response.WriteAsJsonAsync(new ValidateQuestionResponse
                {
                    Valid = true,
                    Message = "Question is valid"
                });
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsJsonAsync(new ValidateQuestionResponse
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
