using Bunit;
using FluentAssertions;
using FormFlow.Blazor.Components.Pages.Admin;
using FormFlow.Blazor.Services;
using FormFlow.Data.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using System.Reflection;

namespace FormFlow.Blazor.Tests.Admin;

file sealed class FakeQuestionService : IQuestionService
{
    public (bool Success, string? Error) NextResult { get; set; } = (true, null);
    public NewQuestion? LastPayload { get; private set; }
    public int CreateCallCount { get; private set; }

    public Task<List<QuestionDefinition>?> GetAllQuestionsAsync()
        => Task.FromResult<List<QuestionDefinition>?>(new());

    public Task<(bool Success, string? Error)> CreateQuestionAsync(NewQuestion newQuestion)
    {
        LastPayload = newQuestion;
        CreateCallCount++;
        return Task.FromResult(NextResult);
    }
}

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
    public async Task CreateButton_Is_Disabled_When_Form_Is_Empty()
    {
        await using var ctx = CreateContext();
        var cut = ctx.Render<AdminCreateQuestion>();

        var button = cut.FindComponents<MudButton>()
            .First(b => b.Markup.Contains("Create Question"));

        button.Instance.Disabled.Should().BeTrue();
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

    [Fact]
    public async Task SuccessAlert_Renders_When_SuccessMessage_IsSet()
    {
        await using var ctx = CreateContext();
        var cut = ctx.Render<AdminCreateQuestion>();

        await cut.InvokeAsync(() =>
        {
            typeof(AdminCreateQuestion)
                .GetField("_successMessage", BindingFlags.Instance | BindingFlags.NonPublic)!
                .SetValue(cut.Instance, "Question 'Age' created successfully.");
            _stateHasChanged!.Invoke(cut.Instance, null);
        });

        cut.WaitForAssertion(() =>
            cut.FindComponents<MudAlert>().Should().ContainSingle(a =>
                a.Instance.Severity == Severity.Success));
    }
    [Fact]
    public async Task ErrorAlert_Renders_When_ErrorMessage_IsSet()
    {
        await using var ctx = CreateContext();
        var cut = ctx.Render<AdminCreateQuestion>();

        await cut.InvokeAsync(() =>
        {
            typeof(AdminCreateQuestion)
                .GetField("_errorMessage", BindingFlags.Instance | BindingFlags.NonPublic)!
                .SetValue(cut.Instance, "409: A question with key 'age' already exists");
            _stateHasChanged!.Invoke(cut.Instance, null);
        });

        cut.WaitForAssertion(() =>
            cut.FindComponents<MudAlert>().Should().ContainSingle(a =>
                a.Instance.Severity == Severity.Error));
    }

    [Fact]
    public async Task SuccessAlert_DoesNotRender_When_SuccessMessage_IsNull()
    {
        await using var ctx = CreateContext();
        var cut = ctx.Render<AdminCreateQuestion>();

        cut.WaitForAssertion(() =>
            cut.FindComponents<MudAlert>()
                .Where(a => a.Instance.Severity == Severity.Success)
                .Should().BeEmpty());
    }

    [Fact]
    public async Task ErrorAlert_DoesNotRender_When_ErrorMessage_IsNull()
    {
        await using var ctx = CreateContext();
        var cut = ctx.Render<AdminCreateQuestion>();

        cut.WaitForAssertion(() =>
            cut.FindComponents<MudAlert>()
                .Where(a => a.Instance.Severity == Severity.Error)
                .Should().BeEmpty());
    }

    private static BunitContext CreateContext()
    {
        var ctx = new BunitContext();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        ctx.Services.AddMudServices();

        var fake = new FakeQuestionService();
        ctx.Services.AddSingleton<FakeQuestionService>(fake);
        ctx.Services.AddSingleton<IQuestionService>(fake);


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
