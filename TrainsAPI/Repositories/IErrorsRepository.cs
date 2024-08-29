using TrainsAPI.Entities;

namespace TrainsAPI.Repositories;

public interface IErrorsRepository
{
    Task<Guid> Create(Error error);
}
