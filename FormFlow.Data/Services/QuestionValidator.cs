using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using FormFlow.Data.Models;

namespace FormFlow.Data.Models
{
    /// <summary>
    /// Validates Question objects using the JSON schema validation script
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
            if (question == null)
            {
                var result = new ValidationResult(false);
                result.Errors.Add(new ValidationError
                {
                    Field = "root",
                    Property = "null",
                    Message = "Question object cannot be null"
                });
                return result;
            }

            try
            {
                

                return validationResult;
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
                // Validate that it's proper JSON first
                using (JsonDocument.Parse(jsonData)) { }

                
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
