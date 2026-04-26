using System.Linq.Expressions;

using Question = FormFlow.Data.Models.QuestionDefinition;

namespace FormFlow.Backend.Repositories
{
    public interface IQuestionRepository
    {
        Question Insert(Question question);
        Question? FindById(Guid id);
        IEnumerable<Question> FindAll();

        Question? FindOne(Expression<Func<Question, bool>> predicate);
    }
}