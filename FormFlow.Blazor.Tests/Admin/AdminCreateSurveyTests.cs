using FluentAssertions;
using FormFlow.Data.Models;
using Xunit;

namespace FormFlow.Blazor.Tests.Admin;

public class AdminCreateSurveyTests
{
    [Fact]
    public void CanSave_False_WhenFormInvalid()
    {
        var state = new SurveyFormState();
        state.IsValid = false;

        state.CanSave.Should().BeFalse();
    }

    [Fact]
    public void CanSave_False_WhenNoQuestionsSelected()
    {
        var state = new SurveyFormState();
        state.IsValid = true;

        state.CanSave.Should().BeFalse();
    }

    [Fact]
    public void CanSave_True_WhenValid_And_HasQuestions()
    {
        var state = new SurveyFormState();
        state.IsValid = true;

        var q = new QuestionDefinition { Id = Guid.NewGuid(), Key = "Q1", Label = "Q1", Type = "text" };
        state.AddQuestion(q);

        state.CanSave.Should().BeTrue();
    }

    [Fact]
    public void AddQuestion_Adds_Only_Once()
    {
        var state = new SurveyFormState();
        var q = new QuestionDefinition { Id = Guid.NewGuid(), Key = "Q1", Label = "Q1", Type = "text" };

        state.AddQuestion(q);
        state.AddQuestion(q);

        state.SelectedQuestions.Should().HaveCount(1);
        state.Survey.QuestionIds.Should().HaveCount(1);
    }

    [Fact]
    public void RemoveQuestion_Removes_Correct_Question()
    {
        var state = new SurveyFormState();

        var q1 = new QuestionDefinition { Id = Guid.NewGuid(), Key = "Q1", Label = "Q1", Type = "text" };
        var q2 = new QuestionDefinition { Id = Guid.NewGuid(), Key = "Q2", Label = "Q2", Type = "number" };

        state.AddQuestion(q1);
        state.AddQuestion(q2);

        state.RemoveQuestion(q1);

        state.SelectedQuestions.Should().ContainSingle().Which.Id.Should().Be(q2.Id);
        state.Survey.QuestionIds.Should().ContainSingle().Which.Should().Be(q2.Id);
    }

    [Fact]
    public void AddQuestion_Populates_SelectedQuestions()
    {
        var state = new SurveyFormState();
        var q = new QuestionDefinition { Id = Guid.NewGuid(), Key = "age", Label = "Age", Type = "number" };

        state.AddQuestion(q);

        state.SelectedQuestions.Should().ContainSingle();
        state.SelectedQuestions[0].Label.Should().Be("Age");
    }

    [Fact]
    public void RemoveQuestion_Updates_SelectedQuestions()
    {
        var state = new SurveyFormState();
        var q = new QuestionDefinition { Id = Guid.NewGuid(), Key = "city", Label = "City", Type = "text" };

        state.AddQuestion(q);
        state.RemoveQuestion(q);

        state.SelectedQuestions.Should().BeEmpty();
    }
}
