using TrainsAPI.DTOs;
using TrainsAPI.Entities;

namespace TrainsAPI.Repositories;

public interface IStationsRepository
{
    Task<int> Create(Station station);
    Task Delete(int id);
    Task<bool> Exist(int id);
    Task<List<int>> Exists(List<int> ids);
    Task<List<Station>> GetAll(PaginationDTO pagination);
    Task<Station?> GetById(int id);
    Task<List<Station>> GetByName(string name);
    Task<List<Station>> GetByCity(string city);
    Task<List<Station>> GetByCountry(string country);
    Task Update(Station station);
}
