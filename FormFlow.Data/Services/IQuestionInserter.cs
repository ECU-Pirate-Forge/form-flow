using System;

namespace FormFlow.Data.Services
{
    /// <summary>
    /// Abstraction for inserting questions; allows mocking in tests
    /// </summary>
    public interface IQuestionInserter
    {
        /// <summary>
        /// Attempts to insert a question represented by raw JSON.
        /// Returns a result object with success state and any validation errors.
        /// </summary>
        // QuestionInserter.InsertResult InsertQuestionFromJson(string jsonData);
    }
}
