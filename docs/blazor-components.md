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

Here you go — a **clean, polished, ready‑to‑paste documentation block** for your **CheckboxQuestion** component, written to perfectly match the structure, tone, and formatting of your existing `DropdownQuestion` docs.

---

# CheckboxQuestion Component

The `CheckboxQuestion` component renders a boolean input using MudBlazor’s `MudCheckBox`. It is used when a question’s `Type` is `"checkbox"`.

### When it is used

`QuestionRenderer` selects this component when:

```razor
@switch (Question.Type.ToLower())
{
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
