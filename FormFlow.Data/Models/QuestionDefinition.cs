using LiteDB;
using System.Collections.Generic;
using FormFlow.Data.Validation;

namespace FormFlow.Data.Models
{
    public class QuestionDefinition
    {
        [BsonId] // Mark this property as the document ID for LiteDB
        public Guid Id { get; set; }
        public required string Key { get; set; }
        public required string Label { get; set; }
        public required string Type { get; set; }

        //default to required unless explicitly set
        public bool Required { get; set; } = true;
        public string? Placeholder { get; set; }
        public string? DefaultValue { get; set; }
        public List<Option> Options { get; set; } = new();
        public VisibleIf? VisibleIf { get; set; }
        public string? ValidationConfigs { get; set; }
        public string? HelpText { get; set; }
    }
}
