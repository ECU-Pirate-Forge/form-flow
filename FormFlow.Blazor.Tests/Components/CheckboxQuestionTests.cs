using Bunit;
using FluentAssertions;
using FormFlow.Blazor.Components.QuestionTypes;
using FormFlow.Data.Models;
using MudBlazor;
using MudBlazor.Services;

namespace FormFlow.Blazor.Tests.Components;

public class CheckboxQuestionTests
{
    [Fact]
    public async Task Renders_Label_Asterisk_And_HelpText()
    {
        await using var ctx = CreateContext();

        var question = CreateCheckboxQuestion(required: true, defaultValue: null);

        var cut = ctx.Render<CheckboxQuestion>(p => p.Add(x => x.Question, question));

        cut.Markup.Should().Contain("Receive newsletters");
        cut.Markup.Should().Contain("*");
        cut.Markup.Should().Contain("Optional subscription");
    }

    [Fact]
    public async Task Renders_MudCheckBox_Component()
    {
        await using var ctx = CreateContext();

        var question = CreateCheckboxQuestion(required: false, defaultValue: "true");

        var cut = ctx.Render<CheckboxQuestion>(p => p.Add(x => x.Question, question));

        // Assert checkbox exists
        cut.FindComponent<MudCheckBox<bool>>();

        // Assert label text appears
        cut.Markup.Should().Contain("Receive newsletters");
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

    private static QuestionDefinition CreateCheckboxQuestion(bool required, string? defaultValue)
    {
        return new QuestionDefinition
        {
            Id = Guid.NewGuid(),
            Key = "newsletter_consent",
            Label = "Receive newsletters",
            Type = "checkbox",
            Required = required,
            DefaultValue = defaultValue,
            HelpText = "Optional subscription"
        };
    }
}
