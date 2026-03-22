using Bunit;
using Bunit.JSInterop;
using FluentAssertions;
using FormFlow.Data.Models;
using MudBlazor.Services;

namespace FormFlow.Tests;

public class QuestionRendererMultipleInstancesTests : TestContext
{
    public QuestionRendererMultipleInstancesTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddMudServices();
    }

    [Fact]
    public void MultipleInstancesInLoop_MaintainIndependentState()
    {
        var questions = new List<QuestionDefinition>
        {
            CreateTextQuestion("first", "First", "First placeholder", "first default"),
            CreateTextQuestion("second", "Second", "Second placeholder", "second default")
        };

        var cut = RenderComponent<QuestionRendererLoopHost>(parameters =>
            parameters.Add(p => p.Questions, questions));

        var inputs = cut.FindAll("input");
        inputs.Should().HaveCount(2);
        inputs[0].GetAttribute("value").Should().Be("first default");
        inputs[1].GetAttribute("value").Should().Be("second default");

        inputs[0].Change("changed first");

        inputs = cut.FindAll("input");
        inputs[0].GetAttribute("value").Should().Be("changed first");
        inputs[1].GetAttribute("value").Should().Be("second default");
    }

    private static QuestionDefinition CreateTextQuestion(
        string key,
        string label,
        string placeholder,
        string defaultValue)
    {
        return new QuestionDefinition
        {
            Id = Guid.NewGuid(),
            Key = key,
            Label = label,
            Type = "text",
            Required = false,
            Placeholder = placeholder,
            DefaultValue = defaultValue,
            Options = new List<Option>(),
            HelpText = string.Empty
        };
    }
}