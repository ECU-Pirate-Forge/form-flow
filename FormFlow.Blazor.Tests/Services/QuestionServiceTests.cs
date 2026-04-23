using System.Net;
using System.Net.Http.Json;
using FormFlow.Blazor.Services;
using FormFlow.Data.Models;
using Moq;
using Moq.Protected;
using System.Text.Json;

namespace FormFlow.Tests.Services;

public class QuestionServiceTests
{
    private readonly Mock<HttpMessageHandler> _handlerMock;
    private readonly HttpClient _httpClient;
    private readonly QuestionService _service;
    private const string BaseUrl = "https://localhost:7209/";

    public QuestionServiceTests()
    {
        _handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        _httpClient = new HttpClient(_handlerMock.Object)
        {
            BaseAddress = new Uri(BaseUrl)
        };
        _service = new QuestionService(_httpClient);
    }

    [Fact]
    public async Task GetAllQuestionsAsync_Success_ReturnsData()
    {
        // Arrange: Simulate a successful 200 OK with two questions
        var mockData = new List<QuestionDefinition>
        {
            new()
            {
                Key = "day",
                Label = "Day",
                Type = "text",
                Required = true,
                Placeholder = "Enter the day"
            },
            new()
            {
                Key = "month",
                Label = "Month",
                Type = "dropdown",
                Required = true,
                Placeholder = "Enter the month",
                Options = new List<Option>
                {
                    new() { Value = "January", Label = "January" },
                    new() { Value = "February", Label = "February" }
                }
            }
        };

        SetupMockResponse(HttpStatusCode.OK, mockData);

        // Act
        var result = await _service.GetAllQuestionsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal("day", result.First().Key);
    }

    [Fact]
    public async Task GetAllQuestionsAsync_ApiError_ReturnsEmptyList()
    {
        // Arrange: Simulate a 500 Internal Server Error
        SetupMockResponse(HttpStatusCode.InternalServerError, "Error message");

        // Act
        var result = await _service.GetAllQuestionsAsync();

        // Assert
        // Our service logic currently returns an empty list on failure
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllQuestionsAsync_NotFound_ReturnsEmptyList()
    {
        // Arrange: Simulate a 404
        SetupMockResponse(HttpStatusCode.NotFound, null);

        // Act
        var result = await _service.GetAllQuestionsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateQuestionAsync_Conflict_Handles409()
    {
        // Note: You'll implement this logic in PBI 3
        // Arrange: Simulate a 409 Conflict (Duplicate Key)
        SetupMockResponse(HttpStatusCode.Conflict, "Key already exists");

        // Act & Assert
        // Depending on your implementation, you might check for a custom exception 
        // or a specific return object that indicates the conflict.
        var response = await _httpClient.PostAsJsonAsync("api/questions",
            new QuestionDefinition
            {
                Key = "TestKey",
                Label = "Test Label",
                Type = "text",
                Required = true,
                Placeholder = "Test Placeholder"
            });
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    // Helper method to keep tests clean
    private void SetupMockResponse(HttpStatusCode code, object? content)
    {
        var response = new HttpResponseMessage
        {
            StatusCode = code,
            Content = new StringContent(JsonSerializer.Serialize(content))
        };

        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(response);
    }
}