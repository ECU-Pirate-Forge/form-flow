using Bunit;
using Xunit;
using FormFlow.Blazor.Components.Shared;

namespace FormFlow.Blazor.Tests.Components;
public class TextQuestionTests : BunitContext {
        [Fact]
        public void TextQuestion_ShouldRenderCorrectLabelAndInput()
        {
            // 1. Arrange: Define the parameters we want to pass to the component
            string expectedLabel = "What is your name?";
            string expectedValue = "John Doe";

            // 2. Act: Render the component
            var cut = Render<TextQuestion>(parameters => parameters
                .Add(p => p.Label, expectedLabel)
                .Add(p => p.Value, expectedValue)
            );

            // 3. Assert: Verify "Label is displayed"
            // This looks for a <label> tag and checks if the text matches
            var labelHtml = cut.Find("label");
            labelHtml.MarkupMatches($"<label>{expectedLabel}</label>");

            // 4. Assert: Verify "<input type='text'> exists"
            // This finds the input and checks its attributes
            var inputHtml = cut.Find("input");
            Assert.Equal("text", inputHtml.GetAttribute("type"));
            
            // 5. Assert: Verify the value is correctly bound
            Assert.Equal(expectedValue, inputHtml.GetAttribute("value"));
        }
}