
using FormFlow.Data.Models;

namespace FormFlow.Blazor.Services
{
    public interface IQuestionService
    {
        Task<List<QuestionDefinition>?> GetAllQuestionsAsync();
        Task<(bool Success, string? Error)> CreateQuestionAsync(NewQuestion newQuestion);
    }
}