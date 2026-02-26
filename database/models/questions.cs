using LiteDB;
using System.Collections.Generic;

namespace database.models
{
    public class Question
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public int Order { get; set; }
        public bool Required { get; set; }
        public string Placeholder { get; set; }
        public string DefaultValue { get; set; }
        public List<string> Options { get; set; }
        public object VisibleIf { get; set; }
        public Dictionary<string, int> Validation { get; set; }
        public string HelpText { get; set; }
    }
}
