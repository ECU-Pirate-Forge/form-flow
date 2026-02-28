using System.ComponentModel.DataAnnotations;
using LiteDB;

namespace backend.Models;

public class FormResponse
{
    [BsonId]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [MinLength(1)]
    public string FormId { get; set; } = string.Empty;

    [Required]
    [MinLength(1)]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public DateTime SubmittedAtUtc { get; set; }

    [Required]
    [MinLength(1)]
    public Dictionary<string, string> Answers { get; set; } = new();
}