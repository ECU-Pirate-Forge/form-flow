using System.Text.Json.Serialization;

namespace FormFlow.Data.Models
{
    public class VisibleIf
    {
        [JsonPropertyName("key")]
        public required string Key { get; set; }

        [JsonPropertyName("shouldEqual")]
        public bool ShouldEqual { get; set; }
    }
}