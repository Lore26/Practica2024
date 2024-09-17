namespace Core.Services;

using Filters;
using Interfaces;
using Models;

public class StationService(IStationRepository stationRepository) : IService<Station>
{
    public async Task<IEnumerable<Station>> GetAllAsync() => await stationRepository.GetAll();

    public async Task<Station?> GetByIdAsync(int id) => await stationRepository.GetById(id);

    public async Task<bool> ExistsAsync(int id) => await stationRepository.Exists(id);

    public async Task<List<int>> ExistAllAsync(int[] ids) => await stationRepository.ExistAll(ids);

    public async Task<int> CreateAsync(Station entity) => await stationRepository.Create(entity);

    public async Task<bool> UpdateAsync(Station entity) => await stationRepository.Update(entity);

    public async Task<bool> DeleteAsync(int id) => await stationRepository.Delete(id);

    public async Task<IEnumerable<Station>> FilterAsync(object filter) => await stationRepository.Filter((StationFilter)filter);
}
