namespace Core.Interfaces;

using Filters;
using Models;

public interface IStationRepository : IRepository<Station>
{
    Task<List<Station>> Filter(StationFilter filter);
}
