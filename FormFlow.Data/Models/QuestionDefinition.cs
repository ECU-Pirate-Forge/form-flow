using LiteDB;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using FormFlow.Data.Validation;

namespace FormFlow.Data.Models
{
    public class QuestionDefinition
    {
        [BsonId]
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("key")]
        public required string Key { get; set; }

        [JsonPropertyName("label")]
        public required string Label { get; set; }

        [JsonPropertyName("type")]
        public required string Type { get; set; }

        [JsonPropertyName("required")]
        public bool Required { get; set; } = true;

        [JsonPropertyName("placeholder")]
        public string? Placeholder { get; set; }

        [JsonPropertyName("defaultValue")]
        public string? DefaultValue { get; set; }

        [JsonPropertyName("options")]
        public List<Option> Options { get; set; } = new();

        [JsonPropertyName("visibleIf")]
        public VisibleIf? VisibleIf { get; set; }

        [JsonPropertyName("validationConfigs")]
        public string? ValidationConfigs { get; set; }

        [JsonPropertyName("helpText")]
        public string? HelpText { get; set; }
    }
}
