using System.Net;
using System.Net.Http.Json;
using FormFlow.Data.Models;
using LiteDB;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;


namespace FormFlow.Backend.Tests.Endpoints
{
    public sealed class BackendFactory : WebApplicationFactory<Program>, IDisposable
    {
        public string DatabasePath { get; } = Path.Combine(
            Path.GetTempPath(),
            $"formflow-endpoint-tests-{Guid.NewGuid():N}.db");

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                var existingDatabaseRegistration = services.FirstOrDefault(d => d.ServiceType == typeof(ILiteDatabase));
                if (existingDatabaseRegistration is not null)
                {
                    services.Remove(existingDatabaseRegistration);
                }

                services.AddSingleton<ILiteDatabase>(_ =>
                    new LiteDatabase($"Filename={DatabasePath};Connection=shared"));
            });
        }

        public new void Dispose()
        {
            base.Dispose();

            if (File.Exists(DatabasePath))
            {
                File.Delete(DatabasePath);
            }
        }
    }

    public class QuestionByIdEndpointTests : IClassFixture<BackendFactory>
    {
        private static readonly Guid ExistingQuestionId = new("b5d8f0e1-1c5b-4f7b-8cfa-6cab5f7fd001");
        private readonly HttpClient _client;
        private readonly BackendFactory _factory;

        public QuestionByIdEndpointTests(BackendFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_QuestionById_Returns200AndRequestedQuestion()
        {
            EnsureKnownQuestionExists();

            var response = await _client.GetAsync($"/api/questions/{ExistingQuestionId}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var question = await response.Content.ReadFromJsonAsync<QuestionDefinition>();
            Assert.NotNull(question);
            Assert.Equal(ExistingQuestionId, question.Id);
            Assert.Equal("first_name", question.Key);
            Assert.Equal("First Name", question.Label);
        }

        [Fact]
        public async Task Get_QuestionById_Returns404WhenQuestionDoesNotExist()
        {
            var unknownId = Guid.NewGuid();

            var response = await _client.GetAsync($"/api/questions/{unknownId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("not-a-guid")]
        public async Task Get_QuestionById_Returns400ForInvalidId(string invalidId)
        {
            var requestPath = string.IsNullOrEmpty(invalidId)
                ? "/api/questions/%20"
                : $"/api/questions/{Uri.EscapeDataString(invalidId)}";

            var response = await _client.GetAsync(requestPath);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Get_QuestionById_WithEmptyDatabase_Returns404AndDoesNotThrow()
        {
            using (var database = new LiteDatabase($"Filename={_factory.DatabasePath};Connection=shared"))
            {
                database.DropCollection("questions");
            }

            var response = await _client.GetAsync($"/api/questions/{Guid.NewGuid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Get_QuestionById_ProducesJson()
        {
            EnsureKnownQuestionExists();

            var response = await _client.GetAsync($"/api/questions/{ExistingQuestionId}");

            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType?.ToString());
        }


        

        private void EnsureKnownQuestionExists()
        {
            using var database = new LiteDatabase($"Filename={_factory.DatabasePath};Connection=shared");
            var collection = database.GetCollection<QuestionDefinition>("questions");

            collection.Upsert(new QuestionDefinition
            {
                Id = ExistingQuestionId,
                Key = "first_name",
                Label = "First Name",
                Type = "text",
                Required = true,
                Placeholder = "Enter your first name"
            });
        }
    }
}