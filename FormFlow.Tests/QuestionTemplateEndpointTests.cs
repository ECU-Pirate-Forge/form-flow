using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using FormFlow.Data.Models;

namespace FormFlow.Tests
{
    public class BackendFactory : WebApplicationFactory<Program>
    {
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

            var question = await response.Content.ReadFromJsonAsync<Question>();
            Assert.NotNull(question);

            Assert.False(string.IsNullOrWhiteSpace(question.Label));
            Assert.False(string.IsNullOrWhiteSpace(question.Type));
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