using System.Text.Json.Serialization;

namespace FormFlow.Data.Models
{
    public class Option
    {
        [JsonPropertyName("label")]
        public required string Label { get; set; }

        [JsonPropertyName("value")]
        public required string Value { get; set; }
    }
}