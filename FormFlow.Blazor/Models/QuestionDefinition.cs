using Microsoft.AspNetCore.SignalR;
using Microsoft.Net.Http.Headers;

namespace FormFlow.Blazor.Models
{
    public class QuestionDefinition
    {
        public string? Id { get; set; }
        public string? Label { get; set; }
        public string? Type { get; set; }

        public string? Placeholder { get; set; }
        public bool? Required { get; set; }
        public string? DefaultValue { get; set; }
        public string? HelpText { get; set; }
    }
}