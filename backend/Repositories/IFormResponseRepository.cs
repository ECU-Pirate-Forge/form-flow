using FormFlow.Backend.Models;

namespace FormFlow.Backend.Repositories;

public interface IFormResponseRepository
{
    Task<FormResponse> SaveAsync(FormResponse response, CancellationToken cancellationToken = default);
    Task<FormResponse?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
}