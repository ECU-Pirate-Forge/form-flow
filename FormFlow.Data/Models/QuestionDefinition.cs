using LiteDB;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using FormFlow.Data.Validation;

namespace FormFlow.Data.Models
{
    public class QuestionDefinition
    {
        [BsonId]
        public Guid Id { get; set; }

        public required string Key { get; set; }

        public required string Label { get; set; }

        public required string Type { get; set; }

        public bool Required { get; set; } = true;

        public string? Placeholder { get; set; }

        public string? DefaultValue { get; set; }

        public List<Option> Options { get; set; } = new();

        public VisibleIf? VisibleIf { get; set; }

        public string? ValidationConfigs { get; set; }

        public string? HelpText { get; set; }
    }
}
