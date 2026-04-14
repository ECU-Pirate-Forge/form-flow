using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using FormFlow.Data.Models;
using FluentAssertions;


namespace FormFlow.Backend.Tests.Endpoints
{
    public class QuestionPostEndpointTests : IClassFixture<BackendFactory>
    {
        private readonly HttpClient _client;

        public QuestionPostEndpointTests(BackendFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Post_ValidQuestion_Returns201Created()
        {
            // Arrange
            var validQuestion = new QuestionDefinition
            {
                Key = "email",
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
            createdQuestion!.Key.Should().Be("email");
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
