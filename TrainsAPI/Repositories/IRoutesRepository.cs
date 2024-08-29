using TrainsAPI.DTOs;
using TrainsAPI.Entities;
using Route = TrainsAPI.Entities.Route;

namespace TrainsAPI.Repositories;

public interface IRoutesRepository
{
    Task Assign(int id, List<RouteStation> stations);
    Task<int> Create(Route route);
    Task Delete(int id);
    Task<bool> Exists(int id);
    Task<List<Route>> GetAll(PaginationDTO paginationDTO);
    Task<Route?> GetById(int id);
    Task Update(Route route);
    Task<List<Route>> Filter(RouteFilterDTO routeFilterDTO);
}
