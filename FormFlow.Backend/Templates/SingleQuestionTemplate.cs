using FormFlow.Data.Models;
using FormFlow.Data.Validation.Models;
using FormFlow.Data.Validation;

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
                    new MinLengthValidationConfig { MinLength = 1 },
                    new MaxLengthValidationConfig { MaxLength = 100 }
                },
                HelpText = "This is an example question."
            };
        }
    }
}