using System;

namespace FormFlow.Data.Validation.Models;

public class RangeValidationConfig : IValidationConfig
{
    public string ValidationType { get; set; } = ValidationTypes.Range;
    public int MinValue { get; set; }
    public int MaxValue { get; set; }
    public string? Message { get; set; }
}