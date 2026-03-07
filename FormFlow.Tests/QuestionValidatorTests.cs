using Xunit;
using FormFlow.Data.Models;
using FormFlow.Data.Services;

namespace tests
{
    public class QuestionValidatorTests
    {
        private readonly QuestionValidator _validator = new QuestionValidator();

        [Fact]
        public void Validate_NullQuestion_ReturnsInvalid()
        {
            Question? nullQuestion = default;
            var result = _validator.Validate(nullQuestion);

            Assert.False(result.Valid);
            Assert.Single(result.Errors);
            Assert.Equal("Question object cannot be null", result.Errors[0].Message);
        }

        [Fact]
        public void Validate_EmptyJsonString_Invalid()
        {
            // use ValidateJson helper
            var result = _validator.ValidateJson(string.Empty);
            Assert.False(result.Valid);
            Assert.Contains(result.Errors, e => e.Message.Contains("empty"));
        }
    }
}
