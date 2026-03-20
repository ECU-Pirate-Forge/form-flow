using LiteDB;
using System.Collections.Generic;
using FormFlow.Data.Validation;

namespace FormFlow.Data.Models
{
    public class QuestionDefinition
    {
        [BsonId] // Mark this property as the document ID for LiteDB
        public Guid Id { get; set; }
        public string Key { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public bool Required { get; set; }
        public string Placeholder { get; set; }
        public string DefaultValue { get; set; }
        public List<Option> Options { get; set; }
        public VisibleIf VisibleIf { get; set; }
        // changed to match ValidationRules class defined in validationRulesClass.cs
        public List<IValidationConfig> ValidationRules { get; set; } = new();
        public string HelpText { get; set; }
    }
}
