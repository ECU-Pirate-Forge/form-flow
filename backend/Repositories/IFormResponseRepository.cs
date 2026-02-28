using backend.Models;

namespace backend.Repositories;

public interface IFormResponseRepository
{
    FormResponse Save(FormResponse response);
}