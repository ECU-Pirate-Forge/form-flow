namespace FormFlow.Data.Models
{
    public class NewQuestion
    {
        public string Label { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool Required { get; set; } = true;
        public string? Placeholder { get; set; }
        public string? DefaultValue { get; set; }
        public string? HelpText { get; set; }
        public List<Option> Options { get; set; } = new();
    }
}