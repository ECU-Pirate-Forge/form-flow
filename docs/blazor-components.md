# Documentation for Blazor components.


## Components
Here we will describe the multiple components used in our blazor webpage

### TextQuestion.razor
This component adds a Text box under a question- A question is defined at QuestionDefinition.cs. The text box contains a label and a value. The value is empty and awaits for any changes made to the text field. This value is captured as a generic object. The object(value) is turned to a string and sent back to the parent component- QuestionRenderer.cs
### Basic usage
If you want to render only the text box here is an example of the component.
```
<TextQuestion Label="Super Important Question" Value="" />
```