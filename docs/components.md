# Components

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
