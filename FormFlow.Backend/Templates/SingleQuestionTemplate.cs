using FormFlow.Data.Models;
using FormFlow.Data.Validation.Models;
using FormFlow.Data.Validation;

namespace FormFlow.Backend.Templates
{
    public static class SingleQuestionTemplate
    {
        public static QuestionDefinition Get()
        {
            var validationConfigs = new List<object>
            {
                new { ValidationType = "MinLength", MinLength = 1 },
                new { ValidationType = "MaxLength", MaxLength = 100 }
            };

            var validationConfigsJson = System.Text.Json.JsonSerializer.Serialize(validationConfigs);

            return new QuestionDefinition
            {
                Id = Guid.Empty,
                Key = "exampleKey",
                Label = "Example question",
                Type = "text",
                Required = true,
                Placeholder = "Enter value",
                DefaultValue = null,
                Options = new List<Option>(),
                VisibleIf = default,
                ValidationConfigs = validationConfigsJson,
                HelpText = "This is an example question."
            };
        }
    }
}