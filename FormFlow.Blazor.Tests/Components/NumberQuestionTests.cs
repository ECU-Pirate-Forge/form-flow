using Bunit;
using FluentAssertions;
using FormFlow.Blazor.Components.QuestionTypes;
using FormFlow.Data.Models;
using MudBlazor;
using MudBlazor.Services;

namespace FormFlow.Blazor.Tests.Components;

public class NumberQuestionTests
{
    [Fact]
    public async Task Renders_Without_Errors()
    {
        await using var ctx = CreateContext();

        var question = CreateNumberQuestion();

        var cut = ctx.Render<NumberQuestion>(p => p.Add(x => x.Question, question));

        cut.Markup.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Renders_Label()
    {
        await using var ctx = CreateContext();

        var question = CreateNumberQuestion();

        var cut = ctx.Render<NumberQuestion>(p => p.Add(x => x.Question, question));

        cut.Markup.Should().Contain("How many years of experience do you have?");
    }

    [Fact]
    public async Task Renders_Input_With_Type_Number()
    {
        await using var ctx = CreateContext();

        var question = CreateNumberQuestion();

        var cut = ctx.Render<NumberQuestion>(p => p.Add(x => x.Question, question));

        var input = cut.Find("input[type='number']");
        input.Should().NotBeNull();
        input.GetAttribute("value").Should().Be("5");
    }

    private static BunitContext CreateContext()
    {
        var ctx = new BunitContext();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        ctx.Services.AddMudServices();

        ctx.Render<MudThemeProvider>();
        ctx.Render<MudPopoverProvider>();
        ctx.Render<MudDialogProvider>();
        ctx.Render<MudSnackbarProvider>();

        return ctx;
    }

    private static QuestionDefinition CreateNumberQuestion()
    {
        return new QuestionDefinition
        {
            Id = Guid.NewGuid(),
            Key = "years_experience",
            Label = "How many years of experience do you have?",
            Type = "number",
            Required = false,
            DefaultValue = "5",
            Placeholder = "Enter a number",
            HelpText = "Use digits only."
        };
    }
}
