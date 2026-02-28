using backend.Models;
using LiteDB;

namespace backend.Repositories;

public class LiteDbFormResponseRepository(IConfiguration configuration) : IFormResponseRepository
{
    private const string CollectionName = "responses";
    private readonly string _connectionString = configuration.GetConnectionString("LiteDb") ?? "Filename=formflow.db;Connection=shared";

    public FormResponse Save(FormResponse response)
    {
        if (string.IsNullOrWhiteSpace(response.Id))
        {
            response.Id = Guid.NewGuid().ToString();
        }

        using var database = new LiteDatabase(_connectionString);
        var collection = database.GetCollection<FormResponse>(CollectionName);
        collection.EnsureIndex(item => item.FormId);
        collection.Insert(response);
        return response;
    }
}