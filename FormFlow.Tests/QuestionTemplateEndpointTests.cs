//extern alias Backend;

using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using FormFlow.Data.Models;

//using BackendProgram = Backend::Program;

namespace FormFlow.Tests
{
    public class BackendFactory : WebApplicationFactory<FormFlow.Backend.Program>
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