using FormFlow.Backend.Models;
using FormFlow.Backend.Data;
using LiteDB;

namespace FormFlow.Backend.Repositories;

public class LiteDbFormResponseRepository : IFormResponseRepository
{
    private readonly ILiteCollection<FormResponse> collection;

    public LiteDbFormResponseRepository(ILiteDbContext context)
    {
        collection = context.FormResponses;
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