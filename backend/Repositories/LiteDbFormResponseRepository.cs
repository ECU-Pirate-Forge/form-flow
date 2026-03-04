using FormFlow.Backend.Models;
using LiteDB;

namespace FormFlow.Backend.Repositories;

public class LiteDbFormResponseRepository : IFormResponseRepository
{
    private readonly ILiteCollection<FormResponse> collection;

    public LiteDbFormResponseRepository(ILiteDatabase database)
    {
        collection = database.GetCollection<FormResponse>("responses");
        collection.EnsureIndex(response => response.FormId);
        collection.EnsureIndex(response => response.SubmittedAtUtc);
    }

    public Task<FormResponse> SaveAsync(FormResponse response, CancellationToken cancellationToken = default)
    {
        response.Id = Guid.NewGuid().ToString("N");
        response.SubmittedAtUtc = DateTime.UtcNow;

        collection.Insert(response);

        return Task.FromResult(response);
    }

    public Task<FormResponse?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        FormResponse? response = collection.FindById(id);
        return Task.FromResult<FormResponse?>(response);
    }
}