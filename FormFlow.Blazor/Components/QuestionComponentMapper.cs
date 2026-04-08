using FormFlow.Blazor.Components.QuestionTypes;

namespace FormFlow.Blazor.Components;

public static class QuestionComponentMapper
{
    public static Type? Resolve(string? type) =>
        type?.ToLower() switch

        {
            "dropdown" => typeof(DropdownQuestion),
            "text" => typeof(TextQuestion),
            "yes_no" => typeof(YesNoQuestion),
            "number" => typeof(NumberQuestion),
            "multiselect" => typeof(MultiselectQuestion),
            "checkbox" => typeof(CheckboxQuestion),
            "radio" => typeof(RadioQuestion),
            _ => null
        };
}
