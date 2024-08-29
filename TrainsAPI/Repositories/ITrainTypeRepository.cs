using TrainsAPI.Entities;

namespace TrainsAPI.Repositories;

public interface ITrainTypeRepository
{
    Task<int> Create(TrainType trainType);
    Task<List<TrainType>> GetAll();
    Task<TrainType?> GetById(int id);
    Task<bool> Exists(int id);
    Task Update(TrainType trainType);
    Task Delete(int id);
    Task<List<int>> Exists(List<int> ids);
    Task<bool> Exists(int id, string name);
}
