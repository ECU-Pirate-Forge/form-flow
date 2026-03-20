using System;

namespace FormFlow.Data.Validation.Models;

public class MinLengthValidationConfig:IValidationConfig
{
    public string ValidationType { get; set; } = ValidationTypes.MinLength;
    public int MinLength { get; set; }

    public string? Message { get; set; } 

}