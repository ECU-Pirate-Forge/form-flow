using System;

namespace FormFlow.Data.Validation.Models;

public class MaxValueValidationConfig : IValidationConfig
{
    public string ValidationType { get; set; } = ValidationTypes.MaxValue;
    public int MaxValue { get; set; }

    public string? Message { get; set; }
}