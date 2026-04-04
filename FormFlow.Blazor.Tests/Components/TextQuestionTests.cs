using Bunit;
using Xunit;
using FluentAssertions;
using MudBlazor;
using MudBlazor.Services;
using FormFlow.Blazor.Components.QuestionTypes;
using FormFlow.Data.Models;
using Microsoft.Extensions.DependencyInjection;

namespace FormFlow.Blazor.Tests.Components;

public class TextQuestionTests : BunitContext
{

    private void SetupServices()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddMudServices();
        Services.AddSingleton(TimeProvider.System);

        // Render MudBlazor providers directly into the current context
        Render<MudThemeProvider>();
        Render<MudPopoverProvider>();
        Render<MudDialogProvider>();
        Render<MudSnackbarProvider>();
    }
    [Fact]
    public async Task Should_Render_Label_And_HelpText_WhenProvided()
    {
        SetupServices();
        var question = CreateTextQuestion("");
        var cut = Render<TextQuestion>(p => p.Add(x => x.Question, question));

        cut.Markup.Should().Contain("Favorite Programmming Language");
        cut.Markup.Should().Contain("Python, C#, JavaScript, etc.");
        await DisposeAsync();
    }
    [Fact]
    public async Task Should_Render_Asterisk_When_Required()
    {
        SetupServices();
        var question = CreateTextQuestion("");
        question.Required = true;

        var cut = Render<TextQuestion>(p => p.Add(x => x.Question, question));

        cut.Markup.Should().Contain("Favorite Programmming Language");
        cut.Markup.Should().Contain("*");

        await DisposeAsync();
    }

    [Fact]
    public async Task Should_UpdateValue_When_Input_Changes()
    {
        SetupServices();
        var question = CreateTextQuestion("");
        var cut = Render<TextQuestion>(p => p.Add(x => x.Question, question));

        var input = cut.Find("input");
        input.Change("C#");

        input.GetAttribute("value").Should().Be("C#");
        await DisposeAsync();
    }

    private static QuestionDefinition CreateTextQuestion(string? defaultValue)
    {
        return new QuestionDefinition
        {
            Id = Guid.NewGuid(),
            Key = "favorite_language",
            Label = "Favorite Programmming Language",
            Type = "text",
            DefaultValue = defaultValue,
            HelpText = "Python, C#, JavaScript, etc.",

        };
    }
}