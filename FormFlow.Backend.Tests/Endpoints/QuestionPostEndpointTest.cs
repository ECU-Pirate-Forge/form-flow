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
        public async Task Post_ValidQuestion_Returns_Created()
        {
            var response = await _client.PostAsJsonAsync("/api/questions", BuildNewQuestion());

            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task Post_ValidQuestion_ResponseBody_Contains_Correct_Fields()
        {
            var input = BuildNewQuestion(label: "Fruit");

            var response = await _client.PostAsJsonAsync("/api/questions", input);
            var body = await response.Content.ReadFromJsonAsync<QuestionDefinition>();

            body.Should().NotBeNull();
            body!.Key.Should().Be(input.Key);
            body.Label.Should().Be("Fruit");
            body.Type.Should().Be(input.Type);
            body.Required.Should().Be(input.Required);
        }

        [Fact]
        public async Task Post_ValidQuestion_Server_Generates_Non_Empty_Id()
        {
            var response = await _client.PostAsJsonAsync("/api/questions", BuildNewQuestion());
            var body = await response.Content.ReadFromJsonAsync<QuestionDefinition>();

            body!.Id.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public async Task ValidQuestion_With_Options_Maps_Options_Correctly()
        {
            var input = BuildNewQuestion(
                type: "dropdown",
                options: new List<Option>
                {
                    new () { Label = "Knight", Value = "knight"},
                    new () { Label = "Bishop", Value = "bishop"}
                });

            var response = await _client.PostAsJsonAsync("/api/questions", input);
            var body = await response.Content.ReadFromJsonAsync<QuestionDefinition>();

            body!.Options.Should().HaveCount(2);
            body.Options[0].Value.Should().Be("knight");
            body.Options[1].Value.Should().Be("bishop");
        }

        [Fact]
        public async Task Post_Missing_Key_Returns_400_BadRequest()
        {
            var input = BuildNewQuestion();


            input.Key = "";

            var response = await _client.PostAsJsonAsync("/api/questions", input);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Question key is required");
        }

        [Fact]
        public async Task Post_Missing_Label_Returns_400_BadRequest()
        {
            var input = BuildNewQuestion();


            input.Label = "";

            var response = await _client.PostAsJsonAsync("/api/questions", input);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Question label is required");
        }

        [Fact]
        public async Task Post_Invalid_Type_Returns_400_BadRequest()
        {
            var input = BuildNewQuestion();


            input.Type = "imagination";

            var response = await _client.PostAsJsonAsync("/api/questions", input);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Question type must be one of: text, number, yes_no, dropdown, radio, checkbox, multiselect");
        }

        [Fact]
        public async Task Post_Option_BasedType_WithNo_Options_Returns_400_BadRequest()
        {
            var input = BuildNewQuestion(type: "dropdown");


            input.Options = new List<Option>();

            var response = await _client.PostAsJsonAsync("/api/questions", input);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("requires at least one option");
        }

        [Fact]
        public async Task Post_Duplicate_Key_Returns_400_BadRequest()
        {
            var input = BuildNewQuestion(key: $"dupe-{Guid.NewGuid():N}");

            await _client.PostAsJsonAsync("/api/questions", input);

            var response = await _client.PostAsJsonAsync("/api/questions", input);


            response.StatusCode.Should().Be(HttpStatusCode.Conflict);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain(input.Key);
        }

        private static NewQuestion BuildNewQuestion(
            string? key = null,
            string label = "Test Label",
            string type = "text",
            List<Option>? options = null) => new()
            {
                Key = key ?? $"test-key-{Guid.NewGuid():N}",
                Label = label,
                Type = type,
                Required = true,
                Options = options ?? new()
            };
    }
}
