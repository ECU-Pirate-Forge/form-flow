# Loading Questions in Blazor

The Blazor app loads a list of questions from `wwwroot/questions.json` using `HttpClient.GetFromJsonAsync`.

- The JSON file contains an array of question objects following the shared schema.
- The list is loaded during `OnInitializedAsync`.
- The loaded list is stored in a `List<QuestionDefinition>` property.
- Rendering logic is handled in a separate task.
