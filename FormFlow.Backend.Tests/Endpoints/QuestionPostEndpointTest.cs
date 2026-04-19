using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using FormFlow.Data.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;

namespace FormFlow.Backend.Tests.Endpoints
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IDisposable
    {
        public string tempDbPath { get; } = Path.Combine(
            Path.GetTempPath(),
            $"formflow-endpoint-tests-{Guid.NewGuid():N}.db");
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var existingDatabaseRegistration = services.FirstOrDefault(d => d.ServiceType == typeof(ILiteDatabase));
                if (existingDatabaseRegistration is not null)
                {
                    services.Remove(existingDatabaseRegistration);
                }

                services.AddSingleton<ILiteDatabase>(_ =>
                    new LiteDatabase($"Filename={tempDbPath};Connection=shared"));
            });
        }
        public new void Dispose()
        {
            base.Dispose();

            if (File.Exists(tempDbPath))
            {
                File.Delete(tempDbPath);
            }
        }
    }


    public class QuestionPostEndpointTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public QuestionPostEndpointTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Post_ValidQuestion_Returns201Created()
        {
            // Arrange
            var uniqueKey = $"email-{Guid.NewGuid():N}";
            var validQuestion = new QuestionDefinition
            {
                //Id = Guid.NewGuid(),
                Key = uniqueKey,
                Label = "Email Address",
                Type = "text",
                Required = true
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/questions", validQuestion);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var createdQuestion = await response.Content.ReadFromJsonAsync<QuestionDefinition>();
            createdQuestion.Should().NotBeNull();
            createdQuestion!.Key.Should().Be(uniqueKey);
            createdQuestion.Label.Should().Be("Email Address");
            createdQuestion.Type.Should().Be("text");
            createdQuestion.Id.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public async Task Post_InvalidQuestion_MissingKey_Returns400BadRequest()
        {
            // Arrange
            var invalidQuestion = new QuestionDefinition
            {
                Id = Guid.NewGuid(),
                Key = "", // Empty key - invalid
                Label = "Email Address",
                Type = "text",
                Required = true
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/questions", invalidQuestion);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Question key is required");
        }
    }
}
