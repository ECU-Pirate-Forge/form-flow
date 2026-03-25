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
    }
}