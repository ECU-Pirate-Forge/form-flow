using Bunit;
using Bunit.JSInterop;
using FluentAssertions;
using FormFlow.Blazor.Components;
using FormFlow.Data.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MudBlazor.Services;

namespace FormFlow.Blazor.Tests;

public class QuestionRendererMultipleInstancesTests : TestContext
{
    public QuestionRendererMultipleInstancesTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddMudServices();
    }

    [Fact]
    public void MultipleInstancesInLoop_MaintainIndependentState()
    {
        var questions = new List<QuestionDefinition>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Key = "first",
                Label = "First",
                Type = "text",
                Required = false,
                Placeholder = "First placeholder",
                DefaultValue = "first default",
                Options = new List<Option>(),
                HelpText = ""
            },
            new()
            {
                Id = Guid.NewGuid(),
                Key = "second",
                Label = "Second",
                Type = "text",
                Required = false,
                Placeholder = "Second placeholder",
                DefaultValue = "second default",
                Options = new List<Option>(),
                HelpText = ""
            }
        };

        var cut = RenderComponent<QuestionRendererLoopHost>(parameters =>
            parameters.Add(p => p.Questions, questions));

        var inputs = cut.FindAll("input");
        inputs.Should().HaveCount(2);
        inputs[0].GetAttribute("value").Should().Be("first default");
        inputs[1].GetAttribute("value").Should().Be("second default");

        inputs[0].Change("changed first");

        inputs = cut.FindAll("input");
        inputs[0].GetAttribute("value").Should().Be("changed first");
        inputs[1].GetAttribute("value").Should().Be("second default");
    }

    private sealed class QuestionRendererLoopHost : ComponentBase
    {
        [Parameter]
        public IReadOnlyList<QuestionDefinition> Questions { get; set; } = [];

#pragma warning disable ASP0006
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            var seq = 0;
            foreach (var question in Questions)
            {
                builder.OpenComponent<QuestionRenderer>(seq++);
                builder.SetKey(question.Id);
                builder.AddAttribute(seq++, "Question", question);
                builder.CloseComponent();
            }
        }
#pragma warning restore ASP0006
    }
}
