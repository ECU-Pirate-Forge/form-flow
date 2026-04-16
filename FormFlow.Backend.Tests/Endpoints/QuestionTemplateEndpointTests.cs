using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Xunit;
using FormFlow.Data.Models;
using Microsoft.Extensions.DependencyInjection;
using LiteDB;


namespace FormFlow.Backend.Tests.Endpoints
{
    public class BackendFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove the default LiteDB registration
                var descriptor = services.FirstOrDefault(
                    d => d.ServiceType == typeof(ILiteDatabase));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Use an in-memory database for testing
                services.AddSingleton<ILiteDatabase>(sp =>
                {
                    var dbPath = Path.Combine(Path.GetTempPath(), $"formflow-test-{Guid.NewGuid():N}.db");
                    return new LiteDatabase($"Filename={dbPath};Connection=shared");
                });
            });
        }
    }

    public class QuestionTemplateEndpointTests : IClassFixture<BackendFactory>
    {
        private readonly HttpClient _client;

        public QuestionTemplateEndpointTests(BackendFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_TemplateQuestion_Returns200AndValidQuestion()
        {
            var response = await _client.GetAsync("/api/questions/template");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var question = await response.Content.ReadFromJsonAsync<QuestionDefinition>();
            Assert.NotNull(question);

            Assert.False(string.IsNullOrWhiteSpace(question.Label));
            Assert.False(string.IsNullOrWhiteSpace(question.Type));

            //ValidationConfigs must be a non-empty JSON string
            Assert.False(string.IsNullOrWhiteSpace(question.ValidationConfigs));
        }

        [Fact]
        public async Task Get_TemplateQuestion_ProducesJson()
        {
            var response = await _client.GetAsync("/api/questions/template");

            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType?.ToString());
        }
    }
}