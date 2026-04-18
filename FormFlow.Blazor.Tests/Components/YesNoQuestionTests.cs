using Bunit;
using FluentAssertions;
using FormFlow.Blazor.Components.QuestionTypes;
using FormFlow.Data.Models;
using MudBlazor.Services;
using Xunit.Sdk;

namespace FormFlow.Blazor.Tests.Components;

public class YesNoQuestionTests
{
    [Fact]
    public async Task Renders_Without_Errors_And_Shows_Label()
    {
        await using var ctx = CreateContext();

        var question = CreateYesNoQuestion(defaultValue: null);

        var cut = ctx.Render<YesNoQuestion>(p => p.Add(x => x.Question, question));

        cut.Markup.Should().Contain("Are you a student?");
    }

    [Fact]
    public async Task Renders_Two_Radio_Options()
    {
        await using var ctx = CreateContext();

        var question = CreateYesNoQuestion(defaultValue: null);

        var cut = ctx.Render<YesNoQuestion>(p => p.Add(x => x.Question, question));

        var radios = cut.FindAll("input[type='radio']");
        radios.Should().HaveCount(2);

        cut.Markup.Should().Contain("Yes");
        cut.Markup.Should().Contain("No");
    }

    [Fact]
    public async Task Updates_Value_When_Selection_Changes()
    {
        await using var ctx = CreateContext();

        var question = CreateYesNoQuestion(defaultValue: null);

        var cut = ctx.Render<YesNoQuestion>(p => p.Add(x => x.Question, question));

        var radios = cut.FindAll("input[type='radio']");
        radios[0].Click();
        question.DefaultValue.Should().Be("true");

        radios[1].Click();
        question.DefaultValue.Should().Be("false");
    }

    private static BunitContext CreateContext()
    {
        var ctx = new BunitContext();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        ctx.Services.AddMudServices();
        return ctx;
    }

    private static QuestionDefinition CreateYesNoQuestion(string? defaultValue)
    {
        return new QuestionDefinition
        {
            Id = Guid.NewGuid(),
            Key = "student",
            Label = "Are you a student?",
            Type = "yes_no",
            Required = false,
            DefaultValue = defaultValue,
            HelpText = "Select yes or no."
        };
    }
}
