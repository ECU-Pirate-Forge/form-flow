using FormFlow.Data.Models;

namespace FormFlow.SeedData
{
    public static class SingleQuestionTemplate
    {
        public static Question Get()
        {
            return new Question
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
                ValidationRules = new ValidationRules
                {
                    MinLength = 1,
                    MaxLength = 100
                },
                HelpText = "This is an example question."
            };
        }
    }
}