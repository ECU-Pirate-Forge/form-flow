using System;

namespace FormFlow.Data.Validation.Models;

public class MinValueValidationConfig:IValidationConfig
{
    public string ValidationType { get; set; } = ValidationTypes.MinValue;
    public int MinValue { get; set; }
    public string? Message { get; set; }
}