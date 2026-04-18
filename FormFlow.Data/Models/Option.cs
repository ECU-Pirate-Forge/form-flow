using System.Text.Json.Serialization;

namespace FormFlow.Data.Models
{
    public class Option
    {
        public required string Label { get; set; }

        public required string Value { get; set; }
    }
}