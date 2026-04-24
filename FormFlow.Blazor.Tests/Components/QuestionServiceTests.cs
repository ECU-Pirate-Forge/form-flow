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
    private const string BaseUrl = "https://test.local/";

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
        var mockData = BuildMockQuestions();

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
    public async Task CreateQuestionAsync_Success_ReturnsSuccessTrue()
    {
        SetupMockResponse(HttpStatusCode.Created, null);

        var (success, error) = await _service.CreateQuestionAsync(BuildNewQuestion());

        Assert.True(success);
        Assert.Null(error);
    }
    [Fact]
    public async Task CreateQuestionAsync_Conflict_ReturnsSuccessFalseWithMessage()
    {
        SetupMockResponse(HttpStatusCode.Conflict, "A question with key 'age' already exists");

        var (success, error) = await _service.CreateQuestionAsync(BuildNewQuestion());

        Assert.False(success);
        Assert.NotNull(error);
        Assert.Contains("409", error);
    }
    [Fact]
    public async Task CreateQuestionAsync_ServerError_ReturnsSuccessFalseWithMessage()
    {
        SetupMockResponse(HttpStatusCode.InternalServerError, "Internal Server Error");

        var (success, error) = await _service.CreateQuestionAsync(BuildNewQuestion());

        Assert.False(success);
        Assert.NotNull(error);
        Assert.Contains("500", error);
    }

    [Fact]
    public async Task CreateQuestionAsync_MapsNewQuestion_ToCorrectPayload()
    {
        // Capture what actually gets sent over the wire
        HttpRequestMessage? captured = null;

        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .Callback<HttpRequestMessage, CancellationToken>((req, _) => captured = req)
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent("")
            });

        var newQuestion = new NewQuestion
        {
            Label = "Age",
            Key = "age",
            Type = "number",
            Required = true,
            Placeholder = "Enter your age",
            HelpText = "Must be 18 or older"
        };

        await _service.CreateQuestionAsync(newQuestion);

        Assert.NotNull(captured);
        Assert.Equal(HttpMethod.Post, captured!.Method);
        Assert.Contains("api/questions", captured.RequestUri!.ToString());

        var body = await captured.Content!.ReadFromJsonAsync<QuestionDefinition>();
        Assert.NotNull(body);
        Assert.Equal("Age", body!.Label);
        Assert.Equal("age", body.Key);
        Assert.Equal("number", body.Type);
        Assert.True(body.Required);
        Assert.Equal("Enter your age", body.Placeholder);
        Assert.Equal("Must be 18 or older", body.HelpText);
        Assert.NotEqual(Guid.Empty, body.Id); // ToDefinition() assigns a new Guid
    }
    [Fact]
    public async Task CreateQuestionAsync_WithOptions_MapsOptionsCorrectly()
    {
        HttpRequestMessage? captured = null;

        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .Callback<HttpRequestMessage, CancellationToken>((req, _) => captured = req)
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent("")
            });

        var newQuestion = new NewQuestion
        {
            Label = "Favourite Color",
            Key = "fav_color",
            Type = "dropdown",
            Options = new List<Option>
            {
                new() { Label = "Red",  Value = "red" },
                new() { Label = "Blue", Value = "blue" }
            }
        };

        await _service.CreateQuestionAsync(newQuestion);

        var body = await captured!.Content!.ReadFromJsonAsync<QuestionDefinition>();
        Assert.NotNull(body);
        Assert.Equal(2, body!.Options.Count);
        Assert.Equal("red", body.Options[0].Value);
        Assert.Equal("blue", body.Options[1].Value);
    }

    // Helpers
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
    private static NewQuestion BuildNewQuestion() => new()
    {
        Label = "Age",
        Key = "age",
        Type = "number",
        Required = true
    };
    private static List<QuestionDefinition> BuildMockQuestions() => new()
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
                new() { Value = "January",  Label = "January" },
                new() { Value = "February", Label = "February" }

            }
        }
    };

}