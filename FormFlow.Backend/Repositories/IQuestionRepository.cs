using FormFlow.Data.Models;

using LiteDB;

using Question = FormFlow.Data.Models.QuestionDefinition;

namespace FormFlow.Backend.Repositories
{
    public interface IQuestionRepository
    {
        ILiteCollection<Question> Questions { get; }
    }
}