using System.Text.Json;
using System.Text.Json.Nodes;
using FormFlow.Data.Validation.Models;

namespace FormFlow.Data.Services;

public class QuestionValidationEngine
{
    public bool Validate(string? response, string? validationConfigs, out List<string> errorMessages)
    {
        errorMessages = new List<string>();

        if (string.IsNullOrWhiteSpace(validationConfigs))
            return true;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // Deserialize into list<object> just like the POC
        var ruleObjects = JsonSerializer.Deserialize<List<object>>(validationConfigs, options);

        if (ruleObjects == null || ruleObjects.Count == 0)
            return true;

        foreach (var ruleObj in ruleObjects)
        {
            JsonObject json = JsonNode.Parse(ruleObj?.ToString()?.Trim('"'))?.AsObject() ?? new JsonObject();

            json.TryGetPropertyValue("validationType", out var typeNode);
            string? type = typeNode?.GetValue<string>();

            switch (type)
            {
                case "MinLength":
                    var min = JsonSerializer.Deserialize<MinLengthValidationConfig>(json.ToJsonString(), options);
                    if (min != null && (response?.Length ?? 0) < min.MinLength)
                        errorMessages.Add(min.Message ?? $"Minimum length is {min.MinLength}.");
                    break;

                case "MaxLength":
                    var max = JsonSerializer.Deserialize<MaxLengthValidationConfig>(json.ToJsonString(), options);
                    if (max != null && (response?.Length ?? 0) > max.MaxLength)
                        errorMessages.Add(max.Message ?? $"Maximum length is {max.MaxLength}.");
                    break;

                case "MinValue":
                    if (int.TryParse(response, out int minVal))
                    {
                        var minValCfg = JsonSerializer.Deserialize<MinValueValidationConfig>(json.ToJsonString(), options);
                        if (minValCfg != null && minVal < minValCfg.MinValue)
                            errorMessages.Add(minValCfg.Message ?? $"Value must be ≥ {minValCfg.MinValue}.");
                    }
                    else errorMessages.Add("Response must be a number.");
                    break;

                case "MaxValue":
                    if (int.TryParse(response, out int maxVal))
                    {
                        var maxValCfg = JsonSerializer.Deserialize<MaxValueValidationConfig>(json.ToJsonString(), options);
                        if (maxValCfg != null && maxVal > maxValCfg.MaxValue)
                            errorMessages.Add(maxValCfg.Message ?? $"Value must be ≤ {maxValCfg.MaxValue}.");
                    }
                    else errorMessages.Add("Response must be a number.");
                    break;

                case "Range":
                    if (int.TryParse(response, out int rangeVal))
                    {
                        var rangeCfg = JsonSerializer.Deserialize<RangeValidationConfig>(json.ToJsonString(), options);
                        if (rangeCfg != null && (rangeVal < rangeCfg.MinValue || rangeVal > rangeCfg.MaxValue))
                            errorMessages.Add(rangeCfg.Message ?? $"Value must be between {rangeCfg.MinValue} and {rangeCfg.MaxValue}.");
                    }
                    else errorMessages.Add("Response must be a number.");
                    break;

                default:
                    errorMessages.Add("Unknown validation type.");
                    break;
            }
        }

        return errorMessages.Count == 0;
    }
}