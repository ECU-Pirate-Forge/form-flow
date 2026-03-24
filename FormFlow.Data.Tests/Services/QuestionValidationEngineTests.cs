using Xunit;
using FormFlow.Data.Services;

public class QuestionValidationEngineTests
{
    private readonly QuestionValidationEngine _engine = new();

    [Fact]
    public void Validate_AllRulesPass_ReturnsTrue()
    {
        string rules = """
        [
            { "validationType": "MinLength", "minLength": 3 }
        ]
        """;

        bool result = _engine.Validate("Hello", rules, out var errors);

        Assert.True(result);
        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_SingleRuleFails_ReturnsFalse()
    {
        string rules = """
        [
            { "validationType": "MinLength", "minLength": 10 }
        ]
        """;

        bool result = _engine.Validate("short", rules, out var errors);

        Assert.False(result);
        Assert.Single(errors);
    }

    [Fact]
    public void Validate_MultipleRulesFail_ReturnsFalse()
    {
        string rules = """
        [
            { "validationType": "Required", "isRequired": true },
            { "validationType": "MinLength", "minLength": 5 }
        ]
        """;

        bool result = _engine.Validate("", rules, out var errors);

        Assert.False(result);
        Assert.Equal(2, errors.Count);
    }
}