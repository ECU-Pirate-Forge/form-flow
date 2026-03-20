// using System;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.Linq;
// using System.Text;
// using System.Text.Json;
// using FormFlow.Data.Models;

// namespace FormFlow.Data.Models
// {
//     /// <summary>
//     /// Validates Question objects using the JSON schema validation script
//     /// Provides detailed error information for API responses
//     /// </summary>
//     public class QuestionValidator
//     {
//         private readonly string _nodeScriptPath;
//         private const string ValidationScript = @"
// const validation = require('./validation');
// const input = JSON.parse(process.argv[1]);
// const result = validation.validateQuestion(input);
// console.log(JSON.stringify(result));
// ";

//         public QuestionValidator(string nodeScriptPath = "./data/schemas/validation/questionValidation.js")
//         {
//             _nodeScriptPath = nodeScriptPath;
//         }

//         /// <summary>
//         /// Validation result containing success status and any errors
//         /// </summary>
//         public class ValidationResult
//         {
//             public bool Valid { get; set; }
//             public List<ValidationError> Errors { get; set; } = new();

//             public ValidationResult(bool valid = true)
//             {
//                 Valid = valid;
//             }
//         }

//         /// <summary>
//         /// Represents a single validation error
//         /// </summary>
//         public class ValidationError
//         {
//             public string Field { get; set; }
//             public string Property { get; set; }
//             public string Message { get; set; }
//         }

//         /// <summary>
//         /// Validates a question object
//         /// </summary>
//         /// <param name="question">The question to validate</param>
//         /// <returns>ValidationResult with details about any validation failures</returns>
//         public ValidationResult Validate(QuestionDefinition question)
//         {
//             if (question == null)
//             {
//                 var result = new ValidationResult(false);
//                 result.Errors.Add(new ValidationError
//                 {
//                     Field = "root",
//                     Property = "null",
//                     Message = "Question object cannot be null"
//                 });
//                 return result;
//             }

//             try
//             {
//                 // Convert Question object to JSON for validation script
//                 var json = JsonSerializer.Serialize(question);

//                 // Call Node.js validation script
//                 var validationResult = ExecuteValidationScript(json);

//                 return validationResult;
//             }
//             catch (Exception ex)
//             {
//                 var result = new ValidationResult(false);
//                 result.Errors.Add(new ValidationError
//                 {
//                     Field = "root",
//                     Property = "exception",
//                     Message = $"Validation service error: {ex.Message}"
//                 });
//                 return result;
//             }
//         }

//         /// <summary>
//         /// Validates a JSON string directly
//         /// </summary>
//         public ValidationResult ValidateJson(string jsonData)
//         {
//             if (string.IsNullOrWhiteSpace(jsonData))
//             {
//                 var result = new ValidationResult(false);
//                 result.Errors.Add(new ValidationError
//                 {
//                     Field = "root",
//                     Property = "empty",
//                     Message = "Question JSON cannot be empty"
//                 });
//                 return result;
//             }

//             try
//             {
//                 // Validate that it's proper JSON first
//                 using (JsonDocument.Parse(jsonData)) { }

//                 var validationResult = ExecuteValidationScript(jsonData);
//                 return validationResult;
//             }
//             catch (JsonException ex)
//             {
//                 var result = new ValidationResult(false);
//                 result.Errors.Add(new ValidationError
//                 {
//                     Field = "root",
//                     Property = "json_parse",
//                     Message = $"Invalid JSON: {ex.Message}"
//                 });
//                 return result;
//             }
//             catch (Exception ex)
//             {
//                 var result = new ValidationResult(false);
//                 result.Errors.Add(new ValidationError
//                 {
//                     Field = "root",
//                     Property = "exception",
//                     Message = $"Validation service error: {ex.Message}"
//                 });
//                 return result;
//             }
//         }

//         /// <summary>
//         /// Executes the Node.js validation script
//         /// </summary>
//         private ValidationResult ExecuteValidationScript(string questionJson)
//         {
//             try
//             {
//                 // Create a temporary file to pass JSON safely
//                 string tempFile = Path.Combine(Path.GetTempPath(), $"question_{Guid.NewGuid()}.json");
//                 File.WriteAllText(tempFile, questionJson);

//                 try
//                 {
//                     // Escape paths for shell
//                     string scriptPath = _nodeScriptPath.Replace("'", "'\\''");
//                     string tempPath = tempFile.Replace("'", "'\\''");

//                     var processInfo = new ProcessStartInfo
//                     {
//                         FileName = "/bin/sh",
//                         Arguments = $"-c \"node -e \\\"const validation = require('{scriptPath}'); const fs = require('fs'); const input = JSON.parse(fs.readFileSync('{tempPath}', 'utf-8')); const result = validation.validateQuestion(input); console.log(JSON.stringify(result));\\\"\"",
//                         UseShellExecute = false,
//                         RedirectStandardOutput = true,
//                         RedirectStandardError = true,
//                         CreateNoWindow = true
//                     };

//                     using (var process = Process.Start(processInfo))
//                     {
//                         if (process == null)
//                         {
//                             throw new InvalidOperationException("Failed to start validation process");
//                         }

//                         var output = process.StandardOutput.ReadToEnd();
//                         var error = process.StandardError.ReadToEnd();
//                         process.WaitForExit(5000); // 5 second timeout

//                         if (!string.IsNullOrWhiteSpace(error))
//                         {
//                             var result = new ValidationResult(false);
//                             result.Errors.Add(new ValidationError
//                             {
//                                 Field = "root",
//                                 Property = "node_error",
//                                 Message = $"Node.js error: {error}"
//                             });
//                             return result;
//                         }

//                         // Parse the validation result from Node.js
//                         return ParseValidationOutput(output);
//                     }
//                 }
//                 finally
//                 {
//                     // Clean up temp file
//                     try
//                     {
//                         File.Delete(tempFile);
//                     }
//                     catch { }
//                 }
//             }
//             catch (Exception ex)
//             {
//                 var result = new ValidationResult(false);
//                 result.Errors.Add(new ValidationError
//                 {
//                     Field = "root",
//                     Property = "execution_error",
//                     Message = $"Failed to execute validation: {ex.Message}"
//                 });
//                 return result;
//             }
//         }


//         /// <summary>
//         /// Parses the validation output from Node.js
//         /// </summary>
//         private ValidationResult ParseValidationOutput(string output)
//         {
//             try
//             {
//                 using (JsonDocument doc = JsonDocument.Parse(output))
//                 {
//                     var root = doc.RootElement;

//                     var result = new ValidationResult(root.GetProperty("valid").GetBoolean());

//                     if (root.TryGetProperty("errors", out var errorsElement) && errorsElement.ValueKind == JsonValueKind.Array)
//                     {
//                         foreach (var errorElement in errorsElement.EnumerateArray())
//                         {
//                             var error = new ValidationError();

//                             if (errorElement.TryGetProperty("field", out var fieldElement))
//                                 error.Field = fieldElement.GetString() ?? "unknown";

//                             if (errorElement.TryGetProperty("property", out var propElement))
//                                 error.Property = propElement.GetString() ?? "unknown";

//                             if (errorElement.TryGetProperty("message", out var msgElement))
//                                 error.Message = msgElement.GetString() ?? "Unknown error";

//                             result.Errors.Add(error);
//                         }
//                     }

//                     return result;
//                 }
//             }
//             catch (JsonException ex)
//             {
//                 var result = new ValidationResult(false);
//                 result.Errors.Add(new ValidationError
//                 {
//                     Field = "root",
//                     Property = "parse_error",
//                     Message = $"Failed to parse validation response: {ex.Message}"
//                 });
//                 return result;
//             }
//         }

//         /// <summary>
//         /// Gets a human-readable error summary
//         /// </summary>
//         public string GetErrorSummary(ValidationResult result)
//         {
//             if (result.Valid)
//                 return "Question is valid";

//             var sb = new StringBuilder("Validation failed:\n");
//             foreach (var error in result.Errors)
//             {
//                 sb.AppendLine($"  • {error.Field}: {error.Message}");
//             }

//             return sb.ToString();
//         }
//     }
// }
