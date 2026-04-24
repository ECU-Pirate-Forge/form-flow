using System.Net;
using System.Net.Http.Json;
using FormFlow.Backend.Endpoints;
using FormFlow.Backend.Repositories;
using FormFlow.Data.Models;
using LiteDB;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Moq;

namespace FormFlow.Backend.Tests.Endpoints;

public class SurveyEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public SurveyEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Replace LiteDB with in-memory instance
                services.AddSingleton<ILiteDatabase>(_ =>
                    new LiteDatabase(new MemoryStream()));

                services.AddSingleton<ISurveyRepository, SurveyRepository>();
            });
        });
    }

    [Fact]
    public async Task GetAllSurveys_ReturnsEmptyList_WhenNoneExist()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/surveys");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var surveys = await response.Content.ReadFromJsonAsync<List<SurveyDefinition>>();

        Assert.NotNull(surveys);
        Assert.Empty(surveys);
    }

    [Fact]
    public async Task GetSurveyById_ReturnsSurvey_WhenExists()
    {
        // Arrange
        var repo = _factory.Services.GetRequiredService<ISurveyRepository>();

        var survey = new SurveyDefinition
        {
            Id = Guid.NewGuid(),
            Title = "Test Survey",
            Description = "A test survey",
            QuestionIds = new List<Guid> { Guid.NewGuid() },
            CreatedAt = DateTime.UtcNow
        };

        repo.Surveys.Insert(survey);

        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/surveys/{survey.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var returned = await response.Content.ReadFromJsonAsync<SurveyDefinition>();

        Assert.NotNull(returned);
        Assert.Equal(survey.Id, returned.Id);
        Assert.Equal(survey.Title, returned.Title);
        Assert.Equal(survey.Description, returned.Description);
        Assert.Equal(survey.QuestionIds, returned.QuestionIds);
    }

    [Fact]
    public async Task GetSurveyById_ReturnsBadRequest_WhenInvalidGuid()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/surveys/not-a-guid");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetSurveyById_ReturnsNotFound_WhenSurveyDoesNotExist()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync($"/api/surveys/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostSurvey_ReturnsCreated_AndPersistsSurvey()
    {
        // Arrange
        var mockRepo = new Mock<ISurveyRepository>();

        var mockCollection = new Mock<ILiteCollection<SurveyDefinition>>();

        mockRepo.Setup(r => r.Surveys).Returns(mockCollection.Object);
        mockRepo.Setup(r => r.Insert(It.IsAny<SurveyDefinition>()))
                .Returns((SurveyDefinition s) => s);

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Replace real repo with mock
                services.AddSingleton<ISurveyRepository>(mockRepo.Object);
            });
        }).CreateClient();

        var newSurvey = new NewSurvey
        {
            Title = "Test Survey",
            Description = "A unit test survey",
            QuestionIds = new List<Guid> { Guid.NewGuid() }
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/surveys", newSurvey);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<SurveyDefinition>();
        Assert.NotNull(created);

        // Backend-generated fields
        Assert.NotEqual(Guid.Empty, created.Id);
        Assert.True(created.CreatedAt > DateTime.UtcNow.AddMinutes(-1));

        // Mapped fields
        Assert.Equal(newSurvey.Title, created.Title);
        Assert.Equal(newSurvey.Description, created.Description);
        Assert.Equal(newSurvey.QuestionIds, created.QuestionIds);

        // Verify repository was called
        mockRepo.Verify(r => r.Insert(It.IsAny<SurveyDefinition>()), Times.Once);
    }
}
