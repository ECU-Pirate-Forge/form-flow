using Bunit;
using FluentAssertions;
using FormFlow.Blazor.Components.Pages.Admin;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using System.Reflection;

namespace FormFlow.Blazor.Tests.Components;

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
