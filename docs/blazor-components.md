## DropdownQuestion Component

The `DropdownQuestion` component renders a single‑select dropdown using MudBlazor’s `MudSelect`. It is used when a question’s `Type` is `"dropdown"`.

### When it is used

`QuestionRenderer` selects this component when:

```razor
@switch (Question.Type.ToLower())
{
    case "dropdown":
        <DropdownQuestion Question="Question" />
        break;
}
```

### Expected QuestionDefinition fields

| Field          | Required | Description |
|----------------|----------|-------------|
| `Label`        | Yes      | The text shown above the dropdown. |
| `Options`      | Yes      | A list of `{ Label, Value }` pairs. |
| `DefaultValue` | No       | Pre‑selected value when the component loads. |
| `Placeholder`  | No       | Text shown when no value is selected. |
| `Required`     | No       | Adds a required indicator and validation. |
| `HelpText`     | No       | Optional helper text shown below the dropdown. |

### Rendering behavior

- The component wraps its content in a `MudPaper` for consistent styling.
- The dropdown is rendered using `MudSelect<string>`.
- Each option is rendered as a `MudSelectItem<string>`.
- If `DefaultValue` is provided, the component initializes its internal value accordingly.
- When the user selects an option, the component updates:
  - its internal `_value`
  - `Question.Answer` (so the parent form can collect responses)

### Example JSON

```json
{
  "id": "123",
  "key": "favorite_color",
  "label": "Favorite Color",
  "type": "dropdown",
  "placeholder": "Choose a color",
  "required": true,
  "defaultValue": "blue",
  "options": [
    { "label": "Red", "value": "red" },
    { "label": "Blue", "value": "blue" },
    { "label": "Green", "value": "green" }
  ],
  "helpText": "Select the color you like most."
}
```

### Example UI

This JSON produces a dropdown with:

- A label: **Favorite Color**
- A required `*` indicator
- A placeholder: *Choose a color*
- Three selectable options
- Helper text below the field

## TextQuestion Component
`TextQuestion.razor` is a component that renders a text box under a question when the `Type` is `"text"`. A Question is defined at `QuestionDefinition.cs`.

### When to used
`QuestionRenderer` selects this component when:

```razor
@switch (Question.Type.ToLower())
{
    case "text":
        <TextQuestion Question="Question" />
        break;
}
```
### Rendering Behavior
`TextQuestion` encloses the component in a `MudPaper`. Inside the Paper a label is rendered. An option asterisk is rendered if the question is required. A `MudTextField` is rendered and bounded to the componeent's internal state(`_value`). The input field also provides a placeholder and helper text. When a user types inpt the input field, the value is updated to reflect that input

## NumberQuestion Component

The `NumberQuestion` component renders a numeric input for questions where `Type` is `"number"`. It is used when the form should accept numeric values through a native HTML number field.

### When it is used

`QuestionRenderer` selects this component when:

```razor
@switch (Question.Type.ToLower())
{
    case "number":
        <NumberQuestion Question="Question" />
        break;
}
```

### Expected `QuestionDefinition` fields

| Field | Required | Description |
|-------|----------|-------------|
| `Label` | Yes | The text shown above the numeric input. |
| `DefaultValue` | No | Initial numeric text shown when the component loads. |
| `Placeholder` | No | Hint text shown when no value has been entered. |
| `Required` | No | Adds a required indicator and `required` attribute. |
| `HelpText` | No | Optional helper text shown below the field. |

### Rendering behavior

- The component wraps its content in a `MudPaper` for consistent styling.
- The label is rendered above the field, with a red `*` when the question is required.
- The input is rendered as a native `<input type="number">`.
- The component binds its internal `_value` using `@bind` with `oninput` so typed values update immediately.
- If `DefaultValue` is provided, the component initializes the field with that value.
- If `HelpText` is provided, it is displayed below the input.

### Example JSON

```json
{
  "id": "123",
  "key": "years_experience",
  "label": "How many years of experience do you have?",
  "type": "number",
  "placeholder": "Enter a number",
  "defaultValue": "5",
  "helpText": "Use digits only.",
  "required": false
}
```

### Example UI

This JSON produces a number input with:

- A label: **How many years of experience do you have?**
- An initial value of `5`
- A placeholder: *Enter a number*
- Helper text below the field
