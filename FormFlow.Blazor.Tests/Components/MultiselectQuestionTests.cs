using Bunit;
using FluentAssertions;
using FormFlow.Blazor.Components.QuestionTypes;
using FormFlow.Data.Models;
using MudBlazor;
using MudBlazor.Services;

namespace FormFlow.Blazor.Tests.Components;

public class MultiSelectQuestionTests
{
    [Fact]
    public async Task Renders_Label_Asterisk_And_HelpText()
    {
        await using var ctx = CreateContext();

        var question = CreateMultiSelectQuestion(required: true);

        var cut = ctx.Render<MultiselectQuestion>(p => p.Add(x => x.Question, question));

        cut.Markup.Should().Contain("Favorite fruits");
        cut.Markup.Should().Contain("*");
        cut.Markup.Should().Contain("Pick all that apply");
    }

    [Fact]
    public async Task Renders_Checkboxes_And_Option_Labels()
    {
        await using var ctx = CreateContext();

        var question = CreateMultiSelectQuestion(required: false);

        var cut = ctx.Render<MultiselectQuestion>(p => p.Add(x => x.Question, question));

        // Should render 3 MudCheckBox components
        var checkboxes = cut.FindComponents<MudCheckBox<bool>>();
        checkboxes.Should().HaveCount(3);

        cut.Markup.Should().Contain("Apple");
        cut.Markup.Should().Contain("Banana");
        cut.Markup.Should().Contain("Cherry");
    }

    [Fact]
    public async Task Toggling_Checkboxes_Updates_Local_State()
    {
        await using var ctx = CreateContext();

        var question = CreateMultiSelectQuestion(required: false);

        var cut = ctx.Render<MultiselectQuestion>(p => p.Add(x => x.Question, question));

        var checkboxes = cut.FindComponents<MudCheckBox<bool>>();

        // Toggle first checkbox ON
       await cut.InvokeAsync(() => checkboxes[0].Instance.ValueChanged.InvokeAsync(true));

        // Toggle second checkbox ON
        await cut.InvokeAsync(() => checkboxes[1].Instance.ValueChanged.InvokeAsync(true));

        // Toggle first checkbox OFF
        await cut.InvokeAsync(() => checkboxes[0].Instance.ValueChanged.InvokeAsync(false));

        // Access the component's internal state
        var instance = cut.Instance;

        instance.GetType()
                .GetField("_selected", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .GetValue(instance)
                .Should()
                .BeEquivalentTo(new List<string> { "banana" });
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

    private static QuestionDefinition CreateMultiSelectQuestion(bool required)
    {
        return new QuestionDefinition
        {
            Id = Guid.NewGuid(),
            Key = "favorite_fruits",
            Label = "Favorite fruits",
            Type = "multiselect",
            Required = required,
            HelpText = "Pick all that apply",
            Options = new List<Option>
            {
                new() { Label = "Apple", Value = "apple" },
                new() { Label = "Banana", Value = "banana" },
                new() { Label = "Cherry", Value = "cherry" }
            }
        };
    }
}
