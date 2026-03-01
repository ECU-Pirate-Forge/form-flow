using System.ComponentModel.DataAnnotations;

namespace FormFlow.Backend.Models;

public class FormResponse
{
    public string? Id { get; set; }

    [Required]
    public string FormId { get; set; } = string.Empty;

    [Required]
    public Dictionary<string, string?> Answers { get; set; } = new();

    public string? SubmittedBy { get; set; }

    public DateTime SubmittedAtUtc { get; set; }
}