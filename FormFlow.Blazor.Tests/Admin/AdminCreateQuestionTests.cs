using Bunit;
using FluentAssertions;
using FormFlow.Blazor.Components.Pages.Admin;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using System.Reflection;

namespace FormFlow.Blazor.Tests.Admin;

public class AdminCreateQuestionTests
{
    [Fact]
    public async Task Form_Renders_All_NewQuestion_Fields()
    {
        await using var ctx = CreateContext();
        var cut = ctx.Render<AdminCreateQuestion>();

        cut.Markup.Should().Contain("Create Question");
        cut.Markup.Should().Contain("Label");
        cut.Markup.Should().Contain("Key");
        cut.Markup.Should().Contain("Type");
        cut.Markup.Should().Contain("Required");
        cut.Markup.Should().Contain("Placeholder");
        cut.Markup.Should().Contain("Default Value");
        cut.Markup.Should().Contain("Help Text");

        var items = cut.FindComponents<MudSelectItem<string>>();
        items.Should().HaveCount(7);
    }

    [Fact]
    public async Task Validation_Triggers_On_Submit()
    {
        await using var ctx = CreateContext();
        var cut = ctx.Render<AdminCreateQuestion>();

        await InvokeCreateQuestionAsync(cut);

        cut.WaitForAssertion(() =>
        {
            cut.Markup.Should().Contain("Label is required.");
            cut.Markup.Should().Contain("Key is required.");
            cut.Markup.Should().Contain("Type is required.");
        });
    }

    [Theory]
    [InlineData("dropdown")]
    [InlineData("radio")]
    [InlineData("checkbox")]
    [InlineData("multiselect")]
    public async Task OptionsEditor_Appears_For_OptionBasedTypes(string type)
    {
        await using var ctx = CreateContext();
        var cut = ctx.Render<AdminCreateQuestion>();

        await SetTypeAsync(cut, type);

        cut.WaitForAssertion(() =>
            cut.Markup.Should().Contain("Add Option"));
    }

    [Theory]
    [InlineData("text")]
    [InlineData("number")]
    [InlineData("yes_no")]
    public async Task OptionsEditor_Hidden_For_NonOptionTypes(string type)
    {
        await using var ctx = CreateContext();
        var cut = ctx.Render<AdminCreateQuestion>();

        await SetTypeAsync(cut, type);

        cut.WaitForAssertion(() =>
            cut.Markup.Should().NotContain("Add Option"));
    }

    [Fact]
    public async Task AddOption_AddsOptionRow()
    {
        await using var ctx = CreateContext();
        var cut = ctx.Render<AdminCreateQuestion>();

        await SetTypeAsync(cut, "dropdown");

        cut.WaitForAssertion(() =>
            cut.Markup.Should().Contain("Add Option"));

        await InvokeAddOptionAsync(cut);

        cut.WaitForAssertion(() =>
            cut.FindAll("[data-option-row]").Should().HaveCount(1));
    }

    [Fact]
    public async Task RemoveOption_RemovesOptionRow()
    {
        await using var ctx = CreateContext();
        var cut = ctx.Render<AdminCreateQuestion>();

        await SetTypeAsync(cut, "radio");

        await InvokeAddOptionAsync(cut);
        await InvokeAddOptionAsync(cut);

        cut.WaitForAssertion(() =>
            cut.FindAll("[data-option-row]").Should().HaveCount(2));

        await InvokeRemoveOptionAsync(cut, 0);

        cut.WaitForAssertion(() =>
            cut.FindAll("[data-option-row]").Should().HaveCount(1));
    }

    [Fact]
    public async Task AddOption_MultipleRows_TrackedCorrectly()
    {
        await using var ctx = CreateContext();
        var cut = ctx.Render<AdminCreateQuestion>();

        await SetTypeAsync(cut, "multiselect");

        await InvokeAddOptionAsync(cut);
        await InvokeAddOptionAsync(cut);
        await InvokeAddOptionAsync(cut);

        cut.WaitForAssertion(() =>
            cut.FindAll("[data-option-row]").Should().HaveCount(3));
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

    private static readonly MethodInfo? _stateHasChanged =
        typeof(ComponentBase).GetMethod("StateHasChanged", BindingFlags.Instance | BindingFlags.NonPublic);

    private static async Task SetTypeAsync(IRenderedComponent<AdminCreateQuestion> cut, string type)
    {
        var typeField = typeof(AdminCreateQuestion)
            .GetField("newQuestion", BindingFlags.Instance | BindingFlags.NonPublic);

        typeField.Should().NotBeNull();

        await cut.InvokeAsync(() =>
        {
            var question = typeField!.GetValue(cut.Instance);
            question!.GetType().GetProperty("Type")!.SetValue(question, type);
            _stateHasChanged!.Invoke(cut.Instance, null);
        });
    }

    private static async Task InvokeAddOptionAsync(IRenderedComponent<AdminCreateQuestion> cut)
    {
        var method = typeof(AdminCreateQuestion).GetMethod(
            "AddOption",
            BindingFlags.Instance | BindingFlags.NonPublic);

        method.Should().NotBeNull();

        await cut.InvokeAsync(() =>
        {
            method!.Invoke(cut.Instance, null);
            _stateHasChanged!.Invoke(cut.Instance, null);
        });
    }

    private static async Task InvokeRemoveOptionAsync(IRenderedComponent<AdminCreateQuestion> cut, int index)
    {
        var method = typeof(AdminCreateQuestion).GetMethod(
            "RemoveOption",
            BindingFlags.Instance | BindingFlags.NonPublic);

        method.Should().NotBeNull();

        await cut.InvokeAsync(() =>
        {
            method!.Invoke(cut.Instance, new object[] { index });
            _stateHasChanged!.Invoke(cut.Instance, null);
        });
    }

    private static async Task InvokeCreateQuestionAsync(IRenderedComponent<AdminCreateQuestion> cut)
    {
        var method = typeof(AdminCreateQuestion).GetMethod(
            "CreateQuestion",
            BindingFlags.Instance | BindingFlags.NonPublic);

        method.Should().NotBeNull();

        await cut.InvokeAsync(async () =>
        {
            var task = method!.Invoke(cut.Instance, null) as Task;
            task.Should().NotBeNull();
            await task!;
        });
    }
}
