namespace Core.Services;

using Filters;
using Interfaces;
using Models;

public class TrainService(ITrainRepository trainRepository) : IService<Train>
{
    public async Task<IEnumerable<Train>> GetAllAsync() => await trainRepository.GetAll();

    public async Task<Train?> GetByIdAsync(int id) => await trainRepository.GetById(id);

    public async Task<bool> ExistsAsync(int id) => await trainRepository.Exists(id);

    public async Task<List<int>> ExistAllAsync(int[] ids) => await trainRepository.ExistAll(ids);

    public async Task<int> CreateAsync(Train entity) => await trainRepository.Create(entity);

    public async Task<bool> UpdateAsync(Train entity) => await trainRepository.Update(entity);

    public async Task<bool> DeleteAsync(int id) => await trainRepository.Delete(id);

    public async Task<IEnumerable<Train>> FilterAsync(object filter) => await trainRepository.Filter((TrainFilter)filter);

    public async Task<IEnumerable<Train>> GetByTypeAsync(int typeId) => await trainRepository.GetByType(typeId);

    public async Task<TrainType?> GetTrainTypeAsync(int trainId) => await trainRepository.GetTrainType(trainId);

    public async Task<IEnumerable<TrainType>> GetAllTrainTypesAsync() => await trainRepository.GetTrainTypes();

    public async Task<bool> TrainTypeExistsAsync(int typeId) => await trainRepository.TrainTypeExists(typeId);

    public async Task<int> CreateTrainTypeAsync(TrainType trainType) => await trainRepository.CreateTrainType(trainType);

    public async Task<bool> UpdateTrainTypeAsync(TrainType trainType) => await trainRepository.UpdateTrainType(trainType);

    public async Task<bool> DeleteTrainTypeAsync(int typeId) => await trainRepository.DeleteTrainType(typeId);
}
