using Bunit;
using FluentAssertions;
using FormFlow.Blazor.Components.QuestionTypes;
using FormFlow.Data.Models;
using MudBlazor;
using MudBlazor.Services;

namespace FormFlow.Blazor.Tests.Components;

public class DropdownQuestionTests
{
    [Fact]
    public async Task Renders_Label_Asterisk_And_HelpText()
    {
        await using var ctx = CreateContext();

        var question = CreateDropdownQuestion(required: true, defaultValue: null);

        var cut = ctx.Render<DropdownQuestion>(p => p.Add(x => x.Question, question));

        cut.Markup.Should().Contain("Favorite language");
        cut.Markup.Should().Contain("*");
        cut.Markup.Should().Contain("Pick one option");
    }

    [Fact]
    public async Task Renders_MudSelect_And_Option_Components()
    {
        await using var ctx = CreateContext();

        var question = CreateDropdownQuestion(required: false, defaultValue: "cs");

        var cut = ctx.Render<DropdownQuestion>(p => p.Add(x => x.Question, question));

        cut.FindComponent<MudSelect<string>>();

        var items = cut.FindComponents<MudSelectItem<string>>();
        items.Should().HaveCount(3);

        cut.Markup.Should().Contain("C#");
        cut.Markup.Should().Contain("Choose a language");
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

    private static QuestionDefinition CreateDropdownQuestion(bool required, string? defaultValue)
    {
        return new QuestionDefinition
        {
            Id = Guid.NewGuid(),
            Key = "favorite_language",
            Label = "Favorite language",
            Type = "dropdown",
            Required = required,
            Placeholder = "Choose a language",
            DefaultValue = defaultValue,
            HelpText = "Pick one option",
            Options = new List<Option>
            {
                new() { Label = "C#", Value = "cs" },
                new() { Label = "JavaScript", Value = "js" },
                new() { Label = "Python", Value = "py" }
            }
        };
    }
}