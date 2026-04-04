# Component Examples

## YesNoQuestion Component

The Blazor `YesNoQuestion` component renders a binary choice for questions where `type` is `"yes_no"`.

### Example JSON

```json
{
  "id": "c9d3a1f7-2b4e-4c8f-9a1d-7e3b2c5d6e33",
  "key": "student",
  "label": "Are you a student?",
  "type": "yes_no",
  "required": true,
  "defaultValue": "true",
  "helpText": "Choose the option that best matches your answer."
}
```

### Expected UI

- A visible label above the control
- Two radio options: **Yes** and **No**
- Boolean state stored as `"true"` or `"false"`
- Optional helper text below the question
