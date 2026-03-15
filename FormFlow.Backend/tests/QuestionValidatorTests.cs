using Xunit;
using database.models;
using database.services;

namespace tests
{
    public class QuestionValidatorTests
    {
        private readonly QuestionValidator _validator = new QuestionValidator();

        [Fact]
        public void Validate_NullQuestion_ReturnsInvalid()
        {
            var result = _validator.Validate(null);

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
