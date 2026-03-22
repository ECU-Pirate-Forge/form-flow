extern alias Backend;

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

using BackendProgram = Backend::Program;

namespace FormFlow.Tests
{
    public class QuestionEndpointTests : IClassFixture<WebApplicationFactory<BackendProgram>>
    {
        private readonly WebApplicationFactory<BackendProgram> _factory;

        public QuestionEndpointTests(WebApplicationFactory<BackendProgram> factory)
        {
            _factory = factory;
        }

        [Fact(Skip = "Endpoint behavior not finalized yet")]
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

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            mockInserter.Verify(i => i.InsertQuestionFromJson(It.IsAny<string>()), Times.Once);
        }
    }
}
