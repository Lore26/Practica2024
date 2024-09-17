namespace Core.Services;

using Filters;
using Interfaces;
using Models;

public class RouteService(IRouteRepository routeRepository) : IService<Route>
{
    public async Task<IEnumerable<Route>> GetAllAsync() => await routeRepository.GetAll();

    public async Task<Route?> GetByIdAsync(int id) => await routeRepository.GetById(id);

    public async Task<bool> ExistsAsync(int id) => await routeRepository.Exists(id);

    public async Task<List<int>> ExistAllAsync(int[] ids) => await routeRepository.ExistAll(ids);

    public async Task<int> CreateAsync(Route entity) => await routeRepository.Create(entity);

    public async Task<bool> UpdateAsync(Route entity) => await routeRepository.Update(entity);

    public async Task<bool> DeleteAsync(int id) => await routeRepository.Delete(id);

    public async Task<IEnumerable<Route>> FilterAsync(object filter) => await routeRepository.Filter((RouteFilter)filter);

    public async Task Assign(int id, List<RouteStation> stations) => await routeRepository.Assign(id, stations);
}
