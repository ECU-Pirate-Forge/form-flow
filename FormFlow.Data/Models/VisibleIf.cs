using System.Text.Json.Serialization;

namespace FormFlow.Data.Models
{
    public class VisibleIf
    {
        public required string Key { get; set; }

        public bool ShouldEqual { get; set; }
    }
}