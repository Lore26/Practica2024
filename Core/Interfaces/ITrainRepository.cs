namespace Core.Interfaces;

using Filters;
using Models;

public interface ITrainRepository : IRepository<Train>
{
    Task<List<Train>> Filter(TrainFilter filter);
    Task<List<Train>> GetByType(int typeId);
    Task<TrainType?> GetTrainType(int trainId);
    Task<List<TrainType>> GetTrainTypes();
    Task<bool> TrainTypeExists(int typeId);
    Task<int> CreateTrainType(TrainType trainType);
    Task<bool> UpdateTrainType(TrainType trainType);
    Task<bool> DeleteTrainType(int typeId);
}
