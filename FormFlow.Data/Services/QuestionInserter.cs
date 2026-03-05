
using System;
using System.IO;
using FormFlow.Data.Models;
using FormFlow.Data.Services;
using Newtonsoft.Json;
using LiteDB;

namespace FormFlow.Data.Models
{
    /// <summary>
    /// Service for inserting questions into LiteDB with schema validation
    /// </summary>
    public class QuestionInserter : IQuestionInserter
    {
        private readonly QuestionValidator _validator;
        private readonly string _dbPath;

        public QuestionInserter(string dbPath = "Filename=questions.db;Connection=shared")
        {
            _dbPath = dbPath;
            _validator = new QuestionValidator();
        }

        /// <summary>
        /// Result of an insert operation
        /// </summary>
        public class InsertResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public Guid? QuestionId { get; set; }
            public QuestionValidator.ValidationResult ValidationResult { get; set; }
        }

        /// <summary>
        /// Inserts a question from a JSON file path
        /// Validates before insertion and returns detailed error information
        /// </summary>
        public InsertResult InsertQuestionFromFile(string jsonFilePath)
        {
            try
            {
                if (!File.Exists(jsonFilePath))
                {
                    return new InsertResult
                    {
                        Success = false,
                        Message = $"File not found: {jsonFilePath}"
                    };
                }

                // Load the JSON data from the file
                string jsonData = File.ReadAllText(jsonFilePath);

                // Deserialize the JSON into a Question object
                var question = JsonConvert.DeserializeObject<Question>(jsonData);
                if (question == null)
                {
                    return new InsertResult
                    {
                        Success = false,
                        Message = "No question data found in JSON file"
                    };
                }

                return InsertQuestion(question);
            }
            catch (JsonSerializationException ex)
            {
                return new InsertResult
                {
                    Success = false,
                    Message = $"Error deserializing JSON: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new InsertResult
                {
                    Success = false,
                    Message = $"Error reading file: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Inserts a question object directly
        /// Validates before insertion and returns detailed error information
        /// </summary>
        public InsertResult InsertQuestion(Question question)
        {
            if (question == null)
            {
                return new InsertResult
                {
                    Success = false,
                    Message = "Question object cannot be null"
                };
            }

            try
            {
                // Validate the question against the schema
                var validationResult = _validator.Validate(question);

                if (!validationResult.Valid)
                {
                    return new InsertResult
                    {
                        Success = false,
                        Message = _validator.GetErrorSummary(validationResult),
                        ValidationResult = validationResult
                    };
                }

                // Generate ID if not provided
                if (question.Id == Guid.Empty)
                {
                    question.Id = Guid.NewGuid();
                }

                // Connect to LiteDB and insert
                using (var db = new LiteDatabase(_dbPath))
                {
                    var questionsCollection = db.GetCollection<Question>("question_data");

                    // Insert the validated question
                    var id = questionsCollection.Insert(question);

                    return new InsertResult
                    {
                        Success = true,
                        Message = "Question inserted successfully",
                        QuestionId = (Guid?)id
                    };
                }
            }
            catch (Exception ex)
            {
                return new InsertResult
                {
                    Success = false,
                    Message = $"Error inserting question into database: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Inserts a question from JSON string
        /// Validates before insertion and returns detailed error information
        /// </summary>
        public InsertResult InsertQuestionFromJson(string jsonData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jsonData))
                {
                    return new InsertResult
                    {
                        Success = false,
                        Message = "JSON data cannot be empty"
                    };
                }

                var question = JsonConvert.DeserializeObject<Question>(jsonData);
                return InsertQuestion(question);
            }
            catch (JsonSerializationException ex)
            {
                return new InsertResult
                {
                    Success = false,
                    Message = $"Error deserializing JSON: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new InsertResult
                {
                    Success = false,
                    Message = $"Error processing JSON: {ex.Message}"
                };
            }
        }
    }
}
