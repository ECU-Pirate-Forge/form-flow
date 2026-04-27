using System.Net;
using System.Text;
using System.Text.Json;
using Bunit;
using FormFlow.Blazor.Components.Pages.Admin;
using FormFlow.Data.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Services;


namespace FormFlow.Blazor.Tests.Admin
{
    public class AdminSurveysListTests
    {
        private static BunitContext CreateContext()
        {
            var ctx = new BunitContext();
            ctx.JSInterop.Mode = JSRuntimeMode.Loose;
            ctx.Services.AddMudServices();

            ctx.JSInterop.SetupVoid("mudPopover.initialize", _ => true);
            ctx.JSInterop.SetupVoid("mudElementRef.addOnBlurEvent", _ => true);
            ctx.JSInterop.SetupVoid("mudKeyInterceptor.connect", _ => true);

            // Register the mock handler as a service
            ctx.Services.AddSingleton<MockHttpMessageHandler>();

            // Register HttpClientFactory using the mock handler
            ctx.Services.AddHttpClient("AdminApi", client =>
            {
                client.BaseAddress = new Uri("http://localhost/");
            })
            .AddHttpMessageHandler<MockHttpMessageHandler>();

            ctx.Render<MudThemeProvider>();
            ctx.Render<MudPopoverProvider>();
            ctx.Render<MudDialogProvider>();
            ctx.Render<MudSnackbarProvider>();

            return ctx;
        }

        // -------------------------------------------------------
        // 1. Loads surveys and displays them
        // -------------------------------------------------------
        [Fact]
        public async Task SurveyList_LoadsAndDisplaysSurveys()
        {
            await using var ctx = CreateContext();

            // Arrange
            var mockHandler = ctx.Services.GetRequiredService<MockHttpMessageHandler>();
            mockHandler.SetJsonResponse("api/surveys", new List<SurveyDefinition>
            {
                new() { Id = Guid.NewGuid(), Title = "Survey A", Description = "Desc A", QuestionIds = [ Guid.NewGuid() ], CreatedAt = DateTime.UtcNow },
                new() { Id = Guid.NewGuid(), Title = "Survey B", Description = "Desc B", QuestionIds = [ Guid.NewGuid(), Guid.NewGuid() ], CreatedAt = DateTime.UtcNow }
            });

            // Act
            var cut = ctx.Render<AdminSurveysList>();

            // Assert
            cut.WaitForAssertion(() => Assert.Contains("Survey A", cut.Markup));
            Assert.Contains("Survey B", cut.Markup);
        }

        // -------------------------------------------------------
        // 2. Shows empty state when no surveys exist
        // -------------------------------------------------------
        [Fact]
        public async Task SurveyList_ShowsEmptyMessage_WhenNoSurveys()
        {
            await using var ctx = CreateContext();

            var mockHandler = ctx.Services.GetRequiredService<MockHttpMessageHandler>();
            mockHandler.SetJsonResponse("api/surveys", new List<SurveyDefinition>());

            var cut = ctx.Render<AdminSurveysList>();

            cut.WaitForAssertion(() => Assert.Contains("No surveys found", cut.Markup));
        }

        // -------------------------------------------------------
        // 3. Preview button navigates correctly
        // -------------------------------------------------------
        [Fact]
        public async Task SurveyList_PreviewButton_NavigatesToPreviewPage()
        {
            await using var ctx = CreateContext();

            var mockHandler = ctx.Services.GetRequiredService<MockHttpMessageHandler>();
            var id = Guid.NewGuid();

            mockHandler.SetJsonResponse("api/surveys", new List<SurveyDefinition>
            {
                new() { Id = id, Title = "Survey A", Description = "Desc", QuestionIds = [], CreatedAt = DateTime.UtcNow }
            });

            var nav = ctx.Services.GetRequiredService<NavigationManager>();

            var cut = ctx.Render<AdminSurveysList>();

            cut.WaitForAssertion(() => Assert.Contains("Survey A", cut.Markup));

            // Click the Preview action for the rendered survey row.
            var previewButton = cut.FindAll("button").First(b => b.TextContent.Contains("Preview", StringComparison.OrdinalIgnoreCase));
            previewButton.Click();

            Assert.Equal($"admin/surveys/{id}/preview", nav.Uri.Replace(nav.BaseUri, ""));
        }
    }

    // ===================================================================
    // Mock HTTP Handler — MUST inherit from DelegatingHandler
    // MUST have a public parameterless constructor
    // ===================================================================
    public class MockHttpMessageHandler : DelegatingHandler
    {
        private readonly Dictionary<string, HttpResponseMessage> _responses = new();

        // REQUIRED: parameterless constructor
        public MockHttpMessageHandler() : base()
        {
        }

        public void SetJsonResponse(string urlContains, object responseObject)
        {
            var json = JsonSerializer.Serialize(responseObject);
            var message = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            _responses[urlContains] = message;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            foreach (var entry in _responses)
            {
                if (request.RequestUri!.ToString().Contains(entry.Key))
                    return Task.FromResult(entry.Value);
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }
    }
}
