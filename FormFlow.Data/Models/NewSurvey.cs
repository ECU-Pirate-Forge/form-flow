using LiteDB;

namespace FormFlow.Data.Models
{
    public class NewSurvey
    {
        [BsonId]
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<Guid> QuestionIds { get; set; } = [];
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? Version { get; set; }
    }
}