using LiteDB;
using FormFlow.Data.Models;

namespace FormFlow.Backend.Repositories
{
    public class SurveyRepository : ISurveyRepository
    {
        public ILiteCollection<SurveyDefinition> Surveys { get; }

        public SurveyRepository(ILiteDatabase db)
        {
            Surveys = db.GetCollection<SurveyDefinition>("surveys");

            Surveys.EnsureIndex(s => s.Id, true);
        }
    }

    public interface ISurveyRepository
    {
        ILiteCollection<SurveyDefinition> Surveys { get; }
    }
}