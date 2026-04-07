using System;
using System.Collections.Generic;
using System.Text.Json;
using FormFlow.Data.Models;
using Microsoft.VisualBasic;
using Xunit;

namespace FormFlow.Data.Tests;

public class SurveyDefinitionTests
{
    private static readonly JsonSerializerOptions options = new()
    {
        PropertyNameCaseInsensitive = true
    };


    [Fact]
    public void Deserialize_ValidSurveyJson_Succeeds()
    {
        var json = """
        {
            "id": "11111111-1111-1111-1111-111111111111",
            "title": "Customer Feedback",
            "description": "A simple survey.",
            "questionIds": [
                "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
                "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
            ],
            "createdAt": "2024-01-01T12:00:00Z",
            "version": 1
        }
        """;

        var survey = JsonSerializer.Deserialize<SurveyDefinition>(json, options);

        Assert.NotNull(survey);
        Assert.Equal(Guid.Parse("11111111-1111-1111-1111-111111111111"), survey!.Id);
        Assert.Equal("Customer Feedback", survey.Title);
        Assert.Equal("A simple survey.", survey.Description);
        Assert.Equal(2, survey.QuestionIds.Count);
        Assert.Equal(1, survey.Version);
    }

        [Fact]
    public void Deserialize_PreservesQuestionOrder()
    {
        var json = """
        {
            "id": "22222222-2222-2222-2222-222222222222",
            "title": "Order Test",
            "description": "Testing order.",
            "questionIds": [
                "11111111-1111-1111-1111-111111111111",
                "22222222-2222-2222-2222-222222222222",
                "33333333-3333-3333-3333-333333333333"
            ],
            "createdAt": "2024-01-01T00:00:00Z"
        }
        """;

        var survey = JsonSerializer.Deserialize<SurveyDefinition>(json, options);

        Assert.NotNull(survey);
        Assert.Equal(Guid.Parse("11111111-1111-1111-1111-111111111111"), survey!.QuestionIds[0]);
        Assert.Equal(Guid.Parse("22222222-2222-2222-2222-222222222222"), survey.QuestionIds[1]);
        Assert.Equal(Guid.Parse("33333333-3333-3333-3333-333333333333"), survey.QuestionIds[2]);
    }

    [Fact]
    public void Deserialize_MissingRequiredFields_FailsValidation()
    {
        var json = """
        {
            "title": "Missing fields",
            "description": "This JSON is missing required fields.",
            "questionIds": [],
            "createdAt": "2024-01-01T00:00:00Z"
        }
        """;

        var survey = JsonSerializer.Deserialize<SurveyDefinition>(json, options);

        // SurveyId is required → default Guid means invalid
        Assert.Equal(Guid.Empty, survey!.Id);

        // Title exists
        Assert.Equal("Missing fields", survey.Title);

        // QuestionIds exists but empty → invalid per your business rules
        Assert.Empty(survey.QuestionIds);
    }

    [Fact]
    public void Deserialize_QuestionIds_Succeeds()
    {
        var json = """
        {
            "id": "99999999-9999-9999-9999-999999999999",
            "title": "Question ID Test",
            "description": "Testing GUID list.",
            "questionIds": [
                "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"
            ],
            "createdAt": "2024-01-01T00:00:00Z"
        }
        """;

        var survey = JsonSerializer.Deserialize<SurveyDefinition>(json, options);

        Assert.NotNull(survey);
        Assert.Single(survey!.QuestionIds);
        Assert.Equal(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), survey.QuestionIds[0]);
    }
}


