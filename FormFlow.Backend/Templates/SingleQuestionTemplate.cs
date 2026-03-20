using FormFlow.Data.Models;

namespace FormFlow.Backend.Templates
{
    public static class SingleQuestionTemplate
    {
        public static QuestionDefinition Get()
        {
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
                ValidationRules = new List<IValidationConfig>
                {
                    MinLength = 1,
                    MaxLength = 100
                },
                HelpText = "This is an example question."
            };
        }
    }
}