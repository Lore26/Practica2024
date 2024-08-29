using TrainsAPI.Entities;

namespace TrainsAPI.Repositories;

public interface ITrainsRepository
{
    Task<int> Create(Train train);
    Task Delete(int id);
    Task<bool> Exists(int id);
    Task<List<Train>> GetAll();
    Task<Train?> GetById(int id);
    Task Update(Train comment);
}
