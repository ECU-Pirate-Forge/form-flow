using FormFlow.Data.Models;

namespace FormFlow.Blazor.Tests.Admin;
public class SurveyFormState
{
    public NewSurvey Survey { get; } = new();
    public List<QuestionDefinition> SelectedQuestions { get; } = new();

    public bool IsValid { get; set; } = false;

    public bool CanSave =>
        IsValid && Survey.QuestionIds.Any();

    public void AddQuestion(QuestionDefinition q)
    {
        if (!Survey.QuestionIds.Contains(q.Id))
        {
            Survey.QuestionIds.Add(q.Id);
            SelectedQuestions.Add(q);
        }
    }

    public void RemoveQuestion(QuestionDefinition q)
    {
        Survey.QuestionIds.Remove(q.Id);
        SelectedQuestions.RemoveAll(x => x.Id == q.Id);
    }
}
