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

---

# MultiSelectQuestion Component

The `MultiSelectQuestion` component renders a list of checkboxes using MudBlazor’s `MudCheckBox<bool>`. It is used when a question’s `Type` is `"multiselect"`.

### When it is used

`QuestionRenderer` selects this component when:

```razor
@switch (Question.Type.ToLower())
{
    case "multiselect":
        <MultiSelectQuestion Question="Question" />
        break;
}
```

### Expected QuestionDefinition fields

| Field       | Required | Description                                           |
|-------------|----------|-------------------------------------------------------|
| `Label`     | Yes      | The text shown above the checkbox list.              |
| `Options`   | Yes      | A list of `{ Label, Value }` pairs.                 |
| `Required`  | No       | Adds a required indicator next to the label.         |
| `HelpText`  | No       | Optional helper text shown below the checkbox list.  |
| `DefaultValue` | No   | *(Not used)* — multi‑select always starts empty.      |
| `Placeholder`  | No   | *(Not used)* — checkboxes do not support placeholders.|

### Rendering behavior

- The component wraps its content in a `MudPaper` for consistent styling.
- A label is rendered at the top, with a required `*` if applicable.
- Each option is rendered as:
  - a `MudCheckBox<bool>`  
  - followed by the option’s label text
- The component maintains its own internal selection state using a `HashSet<string>`.
- When a checkbox is toggled:
  - the internal `_selected` collection is updated  
  - no value is written to `Question.Answer` (your current architecture keeps multi‑select local‑only)

### Example JSON

```json
{
  "id": "456",
  "key": "favorite_fruits",
  "label": "Favorite Fruits",
  "type": "multiselect",
  "required": false,
  "options": [
    { "label": "Apple", "value": "apple" },
    { "label": "Banana", "value": "banana" },
    { "label": "Cherry", "value": "cherry" }
  ],
  "helpText": "Pick all fruits you enjoy."
}
```

### Example UI

This JSON produces a checkbox list with:

- A label: **Favorite Fruits**
- Three checkboxes (Apple, Banana, Cherry)
- Optional helper text below the list
- No dropdown — each option is visible immediately

---
