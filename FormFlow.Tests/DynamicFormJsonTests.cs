using System.IO;
using System.Text.Json;
using Xunit;
using FluentAssertions;
using FormFlow.Data.Models;

namespace FormFlow.Tests
{
    public class DynamicFormJsonTests
    {
        [Fact]
        public void CanDeserializeQuestionsJson()
        {
            var json = File.ReadAllText("multiple-sample-questions.json");

            var questions = JsonSerializer.Deserialize<List<QuestionDefinition>>(json, new JsonSerializerOptions {PropertyNameCaseInsensitive = true});

            questions.Should().NotBeNull();
            questions.Should().HaveCountGreaterThan(0);

            questions[0].Id.Should().NotBeEmpty();
            questions[0].Key.Should().NotBeNullOrEmpty();
            questions[0].Label.Should().NotBeNullOrEmpty();
        }
    }
}