using System.Linq.Expressions;
using LiteDB;
using Question = FormFlow.Data.Models.QuestionDefinition;

namespace FormFlow.Backend.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly ILiteCollection<Question> _questions;

        public QuestionRepository(ILiteDatabase database)
        {
            _questions = database.GetCollection<Question>("questions");

            // Keep lookups by Id efficient and explicit even though Id is marked with [BsonId].
            _questions.EnsureIndex(q => q.Id, true);
        }
        public Question Insert(Question question)
        {
            _questions.Insert(question);
            return question;
        }
        public Question? FindById(Guid id)
        {
            return _questions.FindById(id);
        }

        public IEnumerable<Question> FindAll()
        {
            return _questions.FindAll();
        }
        public Question? FindOne(Expression<Func<Question, bool>> predicate)
        {
            return _questions.FindOne(predicate);
        }
    }
}