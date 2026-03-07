using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using FormFlow.Data.Models;
using FormFlow.Data.Services;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FormFlow.Tests
{
    public class QuestionEndpointTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public QuestionEndpointTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Post_InvalidQuestion_ReturnsBadRequest()
        {
            var mockInserter = new Mock<IQuestionInserter>();
            mockInserter
                .Setup(i => i.InsertQuestionFromJson(It.IsAny<string>()))
                .Returns(new QuestionInserter.InsertResult { Success = false, Message = "invalid" });

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton<IQuestionInserter>(mockInserter.Object);
                });
            }).CreateClient();

            // build invalid JSON (missing required fields)
            var invalidJson = "{ \"label\": \"foo\" }";
            var content = new StringContent(invalidJson, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/questions", content);
            var body = await response.Content.ReadAsStringAsync();
            // dump for debugging
            System.Console.WriteLine("response body=" + body);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            mockInserter.Verify(i => i.InsertQuestionFromJson(It.IsAny<string>()), Times.Once);
        }
    }
}
