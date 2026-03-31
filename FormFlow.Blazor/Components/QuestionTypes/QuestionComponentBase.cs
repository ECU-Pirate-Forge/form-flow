using FormFlow.Data.Models;
using Microsoft.AspNetCore.Components;

public abstract class QuestionComponentBase : ComponentBase
{
    [Parameter, EditorRequired]
    public QuestionDefinition Question { get; set; } = default!;
}