# Components

## NumberQuestion Component

The `NumberQuestion` component renders a numeric field for questions where `Type` is set to `"number"`.

### Rendering behavior

- Displays the question label above the field
- Renders a native `<input type="number">`
- Uses the same card-style layout as the other Blazor question components
- Binds the current value so defaults and edits stay in sync

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
