using FormFlow.Backend.Models;
using LiteDB;

namespace FormFlow.Backend.Data;

public interface ILiteDbContext
{
    ILiteCollection<FormResponse> FormResponses { get; }
}

public class LiteDbContext : ILiteDbContext
{
    private readonly ILiteDatabase _database;

    public ILiteCollection<FormResponse> FormResponses { get; }

    public LiteDbContext(ILiteDatabase database)
    {
        _database = database;
        FormResponses = database.GetCollection<FormResponse>("responses");
        FormResponses.EnsureIndex(r => r.FormId);
        FormResponses.EnsureIndex(r => r.SubmittedAtUtc);
    }
}
