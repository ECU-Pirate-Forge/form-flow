using LiteDB;

namespace FormFlow.Data.Models
{
    public class NewSurvey
    {
        [BsonId]
        public Guid Id { get; set; }

        public required string Title { get; set; } = string.Empty;
        public required string Description { get; set; } = string.Empty;
        public required List<Guid> QuestionIds { get; set; } = [];
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? Version { get; set; }
    }
}