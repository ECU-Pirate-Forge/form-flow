using System.Linq.Expressions;
using FormFlow.Data.Models;

using LiteDB;

using Question = FormFlow.Data.Models.QuestionDefinition;

namespace FormFlow.Backend.Repositories
{
    public interface IQuestionRepository
    {
        ILiteCollection<Question> Questions { get; }
        Question Insert(Question question);
        Question? FindById(Guid id);
        IEnumerable<Question> FindAll();

        Question? FindOne(Expression<Func<Question, bool>> predicate);
    }
}