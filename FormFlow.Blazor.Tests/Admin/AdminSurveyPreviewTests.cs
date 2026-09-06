using System.Net;
using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using FormFlow.Blazor.Components.Pages.Admin;
using FormFlow.Blazor.Components;
using FormFlow.Data.Models;
using RichardSzalay.MockHttp;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor.Services;

namespace FormFlow.Blazor.Tests.Admin
{
    public class AdminSurveyPreviewTests : BunitContext
    {
        public AdminSurveyPreviewTests()
        {
            Services.AddMudServices();
            JSInterop.SetupVoid("mudElementRef.addOnBlurEvent", _ => true);
            JSInterop.SetupVoid("mudElementRef.addOnFocusEvent", _ => true);
            JSInterop.SetupVoid("mudElementRef.addKeyDownEvent", _ => true);
        }

        private readonly Guid surveyId = Guid.NewGuid();

        private SurveyDefinition FakeSurvey => new()
        {
            Id = surveyId,
            Title = "Customer Satisfaction Survey",
            Description = "A test survey",
            QuestionIds = [q1.Id, q2.Id],
            CreatedAt = DateTime.UtcNow
        };

        private readonly QuestionDefinition q1 = new()
        {
            Id = Guid.NewGuid(),
            Label = "How satisfied are you?",
            Key = "satisfaction",
            Type = "rating"
        };

        private readonly QuestionDefinition q2 = new()
        {
            Id = Guid.NewGuid(),
            Label = "Any comments?",
            Key = "comments",
            Type = "text"
        };

        private RichardSzalay.MockHttp.MockHttpMessageHandler SetupMockApi()
        {
            var mock = new RichardSzalay.MockHttp.MockHttpMessageHandler();

            mock.When(HttpMethod.Get, $"http://localhost/api/surveys/{surveyId}")
                .Respond(_ => new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(FakeSurvey)
                });

            mock.When(HttpMethod.Get, $"http://localhost/api/questions/{q1.Id}")
                .Respond(_ => new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(q1)
                });

            mock.When(HttpMethod.Get, $"http://localhost/api/questions/{q2.Id}")
                .Respond(_ => new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(q2)
                });

            return mock;
        }

        private void RegisterMockClient(RichardSzalay.MockHttp.MockHttpMessageHandler mock)
        {
            Services.AddHttpClient("AdminApi")
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost"))
                .ConfigurePrimaryHttpMessageHandler(() => mock);
        }

        [Fact]
        public void ShowsLoadingStateBeforeDataLoads()
        {
            var mock = new RichardSzalay.MockHttp.MockHttpMessageHandler();
            RegisterMockClient(mock);

            var cut = Render<AdminSurveyPreview>(parameters =>
                parameters.Add(p => p.Id, surveyId));

            cut.Markup.Should().Contain("mud-progress-circular");
        }

        [Fact]
        public void LoadsSurveyAndRendersTitle()
        {
            var mock = SetupMockApi();
            RegisterMockClient(mock);

            var cut = Render<AdminSurveyPreview>(parameters =>
                parameters.Add(p => p.Id, surveyId));

            cut.WaitForAssertion(() =>
            {
                cut.Markup.Should().Contain(FakeSurvey.Title);
            });
        }

        [Fact]
        public void RendersAllQuestionsInOrder()
        {
            var mock = SetupMockApi();
            RegisterMockClient(mock);

            var cut = Render<AdminSurveyPreview>(parameters =>
                parameters.Add(p => p.Id, surveyId));

            cut.WaitForState(() => cut.FindComponents<QuestionRenderer>().Count == 2);

            var renderers = cut.FindComponents<QuestionRenderer>();
            renderers.Should().HaveCount(2);
            renderers[0].Instance.Question.Should().BeEquivalentTo(q1);
            renderers[1].Instance.Question.Should().BeEquivalentTo(q2);
        }

        [Fact]
        public void HandlesMissingSurveyGracefully()
        {
            var mock = new RichardSzalay.MockHttp.MockHttpMessageHandler();

            mock.When(HttpMethod.Get, $"http://localhost/api/surveys/{surveyId}")
                .Respond(HttpStatusCode.NotFound);

            RegisterMockClient(mock);

            var cut = Render<AdminSurveyPreview>(parameters =>
                parameters.Add(p => p.Id, surveyId));

            cut.Markup.Should().Contain("mud-progress-circular");
        }
    }
}
