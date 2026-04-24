using LiteDB;

namespace FormFlow.Data.Models
{
    public class NewSurvey
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<Guid> QuestionIds { get; set; } = [];
        //public int? Version { get; set; }
    }
}