using Bunit;
using FormFlow.Data.Models;
using MudBlazor;
using MudBlazor.Services;
using FormFlow.Blazor.Components.QuestionTypes;
using FluentAssertions;

namespace FormFlow.Blazor.Tests.Components;

public class RadioQuestionTests : BunitContext
{
    private void SetupServices()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddMudServices();
        // Render MudBlazor providers directly into the current context
        Render<MudThemeProvider>();
        Render<MudPopoverProvider>();
        Render<MudDialogProvider>();
        Render<MudSnackbarProvider>();
    }
    private static QuestionDefinition CreateRadioQuestion(bool required, string? defaultValue)
    {
        return new QuestionDefinition
        {
            Id = Guid.NewGuid(),
            Key = "favorite_language",
            Label = "Favorite language",
            Type = "radio",
            Required = required,
            Placeholder = "Choose a language",
            DefaultValue = defaultValue,
            HelpText = "Pick one option",
            Options = new List<Option>
            {
                new() { Label = "C#", Value = "cs" },
                new() { Label = "Prolog", Value = "pl" },
                new() { Label = "Q#", Value = "qs" }
            }
        };
    }
    [Fact]
    public async Task Renders_Label_Asterisk_And_HelpText()
    {
        //arrange
        SetupServices();
        var question = CreateRadioQuestion(required: true, defaultValue: null);

        //act
        var cut = Render<RadioQuestion>(p => p.Add(x => x.Question, question));
        //assert
        var renderedText = cut.Find("label").TextContent;
        renderedText.Should().Contain("Favorite language");
        renderedText.Should().Contain("*");

        await DisposeAsync();
    }
    [Fact]
    public async Task Renders_MudRadioGroup_And_Option_Components()
    {
        //arrange
        SetupServices();
        var question = CreateRadioQuestion(required: false, defaultValue: "cs");

        //act
        var cut = Render<RadioQuestion>(p => p.Add(x => x.Question, question));

        //assert
        cut.FindComponent<MudRadioGroup<string>>().Should().NotBeNull();

        var items = cut.FindComponents<MudRadio<string>>();
        items.Should().HaveCount(3);

        var content = cut.Find(".mud-radio-group").TextContent;
        content.Should().Contain("C#");

        await DisposeAsync();
    }
}