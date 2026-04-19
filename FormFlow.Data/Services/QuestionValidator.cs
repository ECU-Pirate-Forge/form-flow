
using System.Text;
using System.Text.Json;
using System.Linq;

namespace FormFlow.Data.Models
{
    /// <summary>
    /// Provides detailed error information for API responses
    /// </summary>
    public class QuestionValidator
    {
        /// <summary>
        /// Validation result containing success status and any errors
        /// </summary>
        public class ValidationResult
        {
            public bool Valid { get; set; }
            public List<ValidationError> Errors { get; set; } = new();

            public ValidationResult(bool valid = true)
            {
                Valid = valid;
            }
        }

        /// <summary>
        /// Represents a single validation error
        /// </summary>
        public class ValidationError
        {
            public string? Field { get; set; }
            public string? Property { get; set; }
            public string? Message { get; set; }
        }

        /// <summary>
        /// Validates a question object
        /// </summary>
        /// <param name="question">The question to validate</param>
        /// <returns>ValidationResult with details about any validation failures</returns>
        public ValidationResult Validate(QuestionDefinition question)
        {
            var result = new ValidationResult(true);
            if (question == null)
            {
                result.Valid = false;
                result.Errors.Add(new ValidationError
                {
                    Field = "root",
                    Property = "null",
                    Message = "Question object cannot be null"
                });
                return result;
            }

            if (string.IsNullOrWhiteSpace(question.Key))
            {
                result.Valid = false;
                result.Errors.Add(new ValidationError
                {
                    Field = "key",
                    Property = "required",
                    Message = "Question key is required and cannot be empty"
                });
            }
            if (string.IsNullOrWhiteSpace(question.Label))
            {
                result.Valid = false;
                result.Errors.Add(new ValidationError
                {
                    Field = "label",
                    Property = "required",
                    Message = "Question label is required and cannot be empty"
                });
            }
            if (string.IsNullOrWhiteSpace(question.Type))
            {
                result.Valid = false;
                result.Errors.Add(new ValidationError
                {
                    Field = "type",
                    Property = "required",
                    Message = "Question type is required and cannot be empty"
                });
            }
            else
            {
                var validTypes = new[] { "text", "number", "yes_no", "dropdown", "radio", "checkbox", "multiselect" };

                if (!validTypes.Contains(question.Type.ToLower()))
                {
                    result.Valid = false;
                    result.Errors.Add(new ValidationError
                    {
                        Field = "type",
                        Property = "enum",
                        Message = $"Question type must be one of: {string.Join(", ", validTypes)}"
                    });
                }
                else
                {
                    var choiceTypes = new[] { "dropdown", "radio", "checkbox", "multiselect" };
                    if (choiceTypes.Contains(question.Type.ToLower()))
                    {
                        if (question.Options == null || !question.Options.Any())
                        {
                            result.Valid = false;
                            result.Errors.Add(new ValidationError
                            {
                                Field = "options",
                                Property = "required",
                                Message = $"Question type '{question.Type}' requires at least one option"
                            });
                        }
                    }

                }
            }
            return result;

        }

        /// <summary>
        /// Validates a JSON string directly
        /// </summary>
        public ValidationResult ValidateJson(string jsonData)
        {
            if (string.IsNullOrWhiteSpace(jsonData))
            {
                var result = new ValidationResult(false);
                result.Errors.Add(new ValidationError
                {
                    Field = "root",
                    Property = "empty",
                    Message = "Question JSON cannot be empty"
                });
                return result;
            }

            try
            {
                var question = JsonSerializer.Deserialize<QuestionDefinition>(jsonData);
                if (question == null)
                {
                    var result = new ValidationResult(false);
                    result.Errors.Add(new ValidationError
                    {
                        Field = "root",
                        Property = "null",
                        Message = "Deserialized question is null"
                    });
                    return result;
                }
                return Validate(question);

            }
            catch (JsonException ex)
            {
                var result = new ValidationResult(false);
                result.Errors.Add(new ValidationError
                {
                    Field = "root",
                    Property = "json_parse",
                    Message = $"Invalid JSON: {ex.Message}"
                });
                return result;
            }
            catch (Exception ex)
            {
                var result = new ValidationResult(false);
                result.Errors.Add(new ValidationError
                {
                    Field = "root",
                    Property = "exception",
                    Message = $"Validation service error: {ex.Message}"
                });
                return result;
            }
        }
        /// <summary>
        /// Gets a human-readable error summary
        /// </summary>
        public string GetErrorSummary(ValidationResult result)
        {
            if (result.Valid)
                return "Question is valid";

            var sb = new StringBuilder("Validation failed:\n");
            foreach (var error in result.Errors)
            {
                sb.AppendLine($"  • {error.Field}: {error.Message}");
            }

            return sb.ToString();
        }
    }
}
