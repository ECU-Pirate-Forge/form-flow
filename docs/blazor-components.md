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

| Field            | Required | Description                                    |
| ---------------- | -------- | ---------------------------------------------- |
| `Label`        | Yes      | The text shown above the dropdown.             |
| `Options`      | Yes      | A list of `{ Label, Value }` pairs.          |
| `DefaultValue` | No       | Pre‑selected value when the component loads.  |
| `Placeholder`  | No       | Text shown when no value is selected.          |
| `Required`     | No       | Adds a required indicator and validation.      |
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

## RadioQuestion.razor
The `RadioQuestion.razor` component renders a radio button group using MudBlazor's `MudRadioGroup` when a Questions `Type` is `"radio"`

### Usage
Within the parent component `QuestionRenderer.razor`
```
case "radio":
            <RadioQuestion Question="Question" />
            break;
```
### Rendering Behavior.
- For each `Option` in `Question`, we wrap the value in `<MudRadio></MudRadio>` button. 
- The MudRadio buttons are wrapped in a `<MudRadioGroup></MudRadioGroup>` parent wrapper component. This component binds the users selected option via two-way binding. e.g. ` @bind-SelectedOption="_value"`
- The binded value gets passed to the parent componenent `QuestionRender.cs` which triggers a re-render and pushes the new data onto the screen.
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
# CheckboxQuestion Component

The `CheckboxQuestion` component renders a boolean input using MudBlazor’s `MudCheckBox`. It is used when a question’s `Type` is `"checkbox"`.

### When it is used

```razor
@switch (Question.Type.ToLower())
{
    case "checkbox":
        <CheckboxQuestion Question="Question" />
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
| Field            | Required | Description                                                 |
| ---------------- | -------- | ----------------------------------------------------------- |
| `Label`        | Yes      | The text shown next to the checkbox.                        |
| `Required`     | No       | Adds a required `*` indicator next to the label.          |
| `HelpText`     | No       | Optional helper text shown below the checkbox.              |
| `DefaultValue` | No       | Not used for checkboxes; checkbox always initializes false. |

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
- The checkbox is rendered using `MudCheckBox<bool>`.
- The checkbox always initializes to `false` (unchecked).
- When the user toggles the checkbox, the component updates:
  - its internal `_value`
  - `Question.Answer` (so the parent form can collect responses)
- A required `*` indicator is shown next to the label when `Question.Required` is true.
- Optional helper text is displayed below the checkbox when provided.

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
  "id": "987",
  "key": "accept_terms",
  "label": "I agree to the terms and conditions",
  "type": "checkbox",
  "required": true,
  "helpText": "You must agree before continuing."
}
```

### Example UI

This JSON produces a checkbox list with:

- A label: **Favorite Fruits**
- Three checkboxes (Apple, Banana, Cherry)
- Optional helper text below the list
- No dropdown — each option is visible immediately

---
This JSON produces a checkbox with:

- A label: **I agree to the terms and conditions**
- A required `*` indicator
- An unchecked checkbox (always starts false)
- Helper text below the field

---

# CheckboxQuestion Component

The `CheckboxQuestion` component renders a boolean input using MudBlazor’s `MudCheckBox`. It is used when a question’s `Type` is `"checkbox"`.

### When it is used

    case "checkbox":
        <CheckboxQuestion Question="Question" />
        break;
}
```

### Expected QuestionDefinition fields

| Field            | Required | Description                                                 |
| ---------------- | -------- | ----------------------------------------------------------- |
| `Label`        | Yes      | The text shown next to the checkbox.                        |
| `Required`     | No       | Adds a required `*` indicator next to the label.          |
| `HelpText`     | No       | Optional helper text shown below the checkbox.              |
| `DefaultValue` | No       | Not used for checkboxes; checkbox always initializes false. |

### Rendering behavior

- The component wraps its content in a `MudPaper` for consistent styling.
- The checkbox is rendered using `MudCheckBox<bool>`.
- The checkbox always initializes to `false` (unchecked).
- When the user toggles the checkbox, the component updates:
  - its internal `_value`
  - `Question.Answer` (so the parent form can collect responses)
- A required `*` indicator is shown next to the label when `Question.Required` is true.
- Optional helper text is displayed below the checkbox when provided.

### Example JSON

```json
{
  "id": "987",
  "key": "accept_terms",
  "label": "I agree to the terms and conditions",
  "type": "checkbox",
  "required": true,
  "helpText": "You must agree before continuing."
}
```

### Example UI

This JSON produces a checkbox with:

- A label: **I agree to the terms and conditions**
- A required `*` indicator
- An unchecked checkbox (always starts false)
- Helper text below the field

---

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

---

## YesNoQuestion Component
`YesNoQuestion.razor` renders a binary choice when the `Type` is `"yes_no"`.

### When it is used
`QuestionRenderer` selects this component when:

```razor
@switch (Question.Type.ToLower())
{
    case "yes_no":
        <YesNoQuestion Question="Question" />
        break;
}
```

### Rendering Behavior
`YesNoQuestion` wraps the field in a `MudPaper`, shows the question label, and renders two radio options labeled **Yes** and **No**. The selected value is bound to an internal `bool?` and persisted back to `Question.DefaultValue` as `"true"` or `"false"`.

### Example JSON
```json
{
  "id": "c9d3a1f7-2b4e-4c8f-9a1d-7e3b2c5d6e33",
  "key": "student",
  "label": "Are you a student?",
  "type": "yes_no",
  "required": true,
  "defaultValue": "false",
  "helpText": "Choose one option."
}
```




