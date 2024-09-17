namespace Core.Interfaces;

using Filters;
using Models;

public interface IRouteRepository : IRepository<Route>
{
    Task Assign(int id, List<RouteStation> stations);
    Task<List<Route>> Filter(RouteFilter filter);
}
