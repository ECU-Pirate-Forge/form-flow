using System;
using System.Collections.Generic;
using System.Text.Json;
using FormFlow.Data.Models;
using Xunit;

namespace FormFlow.Tests.Models
{
    public class QuestionDefinitionTests
    {
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        // ---------------------------------------------------------
        // VALID JSON TESTS
        // ---------------------------------------------------------

        [Fact]
        public void Deserialize_ValidMinimalJson_Succeeds()
        {
            string json = """
            {
                "id": "11111111-1111-1111-1111-111111111111",
                "key": "firstName",
                "label": "First Name",
                "type": "text"
            }
            """;

            var result = JsonSerializer.Deserialize<QuestionDefinition>(json, _jsonOptions);

            Assert.NotNull(result);
            Assert.Equal("firstName", result.Key);
            Assert.Equal("First Name", result.Label);
            Assert.Equal("text", result.Type);
            Assert.True(result.Required); // default value
        }

        [Fact]
        public void Deserialize_WithOptions_Succeeds()
        {
            string json = """
            {
                "id": "22222222-2222-2222-2222-222222222222",
                "key": "favoriteColor",
                "label": "Favorite Color",
                "type": "dropdown",
                "options": [
                    { "value": "red", "label": "Red" },
                    { "value": "blue", "label": "Blue" }
                ]
            }
            """;

            var result = JsonSerializer.Deserialize<QuestionDefinition>(json, _jsonOptions);

            Assert.NotNull(result);
            Assert.Equal(2, result.Options.Count);
            Assert.Equal("red", result.Options[0].Value);
            Assert.Equal("Red", result.Options[0].Label);
        }

        [Fact]
        public void Deserialize_WithVisibleIf_Succeeds()
        {
            string json = """
            {
                "id": "33333333-3333-3333-3333-333333333333",
                "key": "petName",
                "label": "Pet Name",
                "type": "text",
                "visibleIf": {
                    "key": "hasPet",
                    "shouldEqual": true
                }
            }
            """;

            var result = JsonSerializer.Deserialize<QuestionDefinition>(json, _jsonOptions);

            Assert.NotNull(result);
            Assert.NotNull(result.VisibleIf);
            Assert.Equal("hasPet", result.VisibleIf!.Key);
            Assert.True(result.VisibleIf.ShouldEqual);
        }

        [Fact]
        public void Deserialize_WithValidationConfigsString_Succeeds()
        {
            string json = """
            {
                "id": "44444444-4444-4444-4444-444444444444",
                "key": "age",
                "label": "Age",
                "type": "number",
                "validationConfigs": "[{\"validationType\":\"MinValue\",\"minValue\":18}]"
            }
            """;

            var result = JsonSerializer.Deserialize<QuestionDefinition>(json, _jsonOptions);

            Assert.NotNull(result);
            Assert.Equal("[{\"validationType\":\"MinValue\",\"minValue\":18}]", result.ValidationConfigs);
        }

        [Fact]
        public void Serialize_And_Deserialize_RoundTrip_NoDataLoss()
        {
            var original = new QuestionDefinition
            {
                Id = Guid.NewGuid(),
                Key = "email",
                Label = "Email Address",
                Type = "text",
                Placeholder = "Enter your email",
                Required = true,
                HelpText = "We will not spam you.",
                ValidationConfigs = "[{\"validationType\":\"Required\"}]",
                Options = new List<Option>(),
                VisibleIf = new VisibleIf
                {
                    Key = "wantsNewsletter",
                    ShouldEqual = true
                }
            };

            string json = JsonSerializer.Serialize(original);
            var result = JsonSerializer.Deserialize<QuestionDefinition>(json);

            Assert.NotNull(result);
            Assert.Equal(original.Key, result.Key);
            Assert.Equal(original.Label, result.Label);
            Assert.Equal(original.Type, result.Type);
            Assert.Equal(original.Placeholder, result.Placeholder);
            Assert.Equal(original.HelpText, result.HelpText);
            Assert.Equal(original.ValidationConfigs, result.ValidationConfigs);
            Assert.NotNull(result.VisibleIf);
            Assert.Equal("wantsNewsletter", result.VisibleIf!.Key);
        }

        // ---------------------------------------------------------
        // INVALID JSON TESTS
        // ---------------------------------------------------------

        [Fact]
        public void Deserialize_MissingRequiredField_ThrowsException()
        {
            string json = """
            {
                "id": "55555555-5555-5555-5555-555555555555",
                "label": "Missing Key",
                "type": "text"
            }
            """;

            Assert.Throws<JsonException>(() =>
                JsonSerializer.Deserialize<QuestionDefinition>(json, _jsonOptions));
        }

        [Fact]
        public void Deserialize_InvalidVisibleIfType_ThrowsException()
        {
            string json = """
            {
                "id": "66666666-6666-6666-6666-666666666666",
                "key": "petName",
                "label": "Pet Name",
                "type": "text",
                "visibleIf": {
                    "key": "hasPet",
                    "equals": "yes"
                }
            }
            """;

            var result = JsonSerializer.Deserialize<QuestionDefinition>(json, _jsonOptions);

            Assert.NotNull(result);
            Assert.NotNull(result.VisibleIf);
            Assert.False(result.VisibleIf!.ShouldEqual);
        }

        [Fact]
        public void Deserialize_OptionsMissingRequiredFields_ThrowsException()
        {
            string json = """
            {
                "id": "77777777-7777-7777-7777-777777777777",
                "key": "color",
                "label": "Color",
                "type": "dropdown",
                "options": [
                    { "value": "red" }
                ]
            }
            """;

            Assert.Throws<JsonException>(() =>
                JsonSerializer.Deserialize<QuestionDefinition>(json, _jsonOptions));
        }
    }
}