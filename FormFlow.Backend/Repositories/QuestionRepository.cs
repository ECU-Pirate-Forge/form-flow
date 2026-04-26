using System.Linq.Expressions;
using FormFlow.Data.Models;

using LiteDB;

using Question = FormFlow.Data.Models.QuestionDefinition;

namespace FormFlow.Backend.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        public ILiteCollection<Question> Questions { get; }

        public QuestionRepository(ILiteDatabase database)
        {
            Questions = database.GetCollection<Question>("questions");

            // Keep lookups by Id efficient and explicit even though Id is marked with [BsonId].
            Questions.EnsureIndex(q => q.Id, true);
        }
        public Question Insert(Question question)
        {
            Questions.Insert(question);
            return question;
        }
        public Question? FindById(Guid id)
        {
            return Questions.FindById(id);
        }

        public IEnumerable<Question> FindAll()
        {
            return Questions.FindAll();
        }

        public Question? FindOne(Expression<Func<Question, bool>> predicate)
        {
            return Questions.FindOne(predicate);
        }
    }
}