using System;
using Xunit;
using FormFlow.Blazor.Components;
using FormFlow.Blazor.Components.QuestionTypes;

namespace FormFlow.Blazor.Tests.Components
{
    public class QuestionComponentMapperTests
    {
        [Theory]
        [InlineData("text", typeof(TextQuestion))]
        [InlineData("dropdown", typeof(DropdownQuestion))]
        [InlineData("yes_no", typeof(YesNoQuestion))]
        [InlineData("number", typeof(NumberQuestion))]
        [InlineData("multiselect", typeof(MultiselectQuestion))]
        [InlineData("checkbox", typeof(CheckboxQuestion))]
        [InlineData("radio", typeof(RadioQuestion))]
        public void Resolve_Returns_Correct_Component_Type(string type, Type expected)
        {
            // Act
            var result = QuestionComponentMapper.Resolve(type);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("TEXT")]          // uppercase
        [InlineData("Text")]          // PascalCase
        [InlineData("TeXt")]          // mixed case
        public void Resolve_Is_Case_Insensitive(string type)
        {
            // Act
            var result = QuestionComponentMapper.Resolve(type);

            // Assert
            Assert.Equal(typeof(TextQuestion), result);
        }

        [Fact]
        public void Resolve_Returns_Null_For_Unknown_Type()
        {
            // Act
            var result = QuestionComponentMapper.Resolve("not_a_real_type");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Resolve_Returns_Null_When_Type_Is_Null()
        {
            // Act
            var result = QuestionComponentMapper.Resolve(null);

            // Assert
            Assert.Null(result);
        }
    }
}
