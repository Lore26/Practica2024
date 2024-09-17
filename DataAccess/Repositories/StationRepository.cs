using System.Data;
using Core.Filters;
using Core.Interfaces;
using Core.Models;
using Dapper;

namespace DataAccess.Repositories;

public class StationRepository(IDbConnection dbConnection) : IStationRepository
{
    public async Task<List<Station>> GetAll()
    {
        var sql = "SELECT Id, Name FROM Stations";
        return (await dbConnection.QueryAsync<Station>(sql)).ToList();
    }

    public async Task<Station?> GetById(int id)
    {
        var sql = $"SELECT Id, Name FROM Stations WHERE Id = {id}";
        return (await dbConnection.QueryAsync<Station>(sql)).FirstOrDefault();
    }

    public async Task<bool> Exists(int id)
    {
        var sql = "SELECT Id FROM Stations WHERE Id = @Id";
        return await dbConnection.ExecuteScalarAsync<bool>(sql, new { id });
    }

    public async Task<List<int>> ExistAll(int[] ids)
    {
        const string sql = "SELECT Id FROM Stations WHERE Id = ANY(${ids})";
        var foundIds = await dbConnection.QueryAsync<int>(sql);

        return foundIds.ToList();
    }

    public async Task<int> Create(Station entity)
    {
        var sql = $"""
                   INSERT INTO Stations (Name, City, Country)
                    VALUES ({entity.Name}, {entity.City}, {entity.Country});
                   SELECT SCOPE_IDENTITY();
                   """;
        return await dbConnection.QuerySingleAsync<int>(sql, entity);
    }

    public async Task<bool> Update(Station entity)
    {
        var sql = $"""
                   UPDATE Stations
                   SET Name = {entity.Name}, City = {entity.City}, Country = {entity.Country}
                   WHERE Id = {entity.Id};
                   SELECT SCOPE_IDENTITY();
                   """;
        return await dbConnection.ExecuteAsync(sql, entity) > 0;
    }

    public async Task<bool> Delete(int id)
    {
        var sql = "DELETE FROM Stations WHERE Id = @Id";
        return await dbConnection.ExecuteAsync(sql, new { id }) > 0;
    }

    public async Task<List<Station>> Filter(StationFilter filter)
    {
        var sql = "SELECT Id, Name FROM Stations WHERE 1 = 1";
        if (!string.IsNullOrEmpty(filter.Name))
        {
            sql += $" AND Name LIKE '%{filter.Name}%'";
        }

        if (!string.IsNullOrEmpty(filter.City))
        {
            sql += $" AND City LIKE '%{filter.City}%'";
        }

        if (!string.IsNullOrEmpty(filter.Country))
        {
            sql += $" AND Country LIKE '%{filter.Country}%'";
        }

        return (await dbConnection.QueryAsync<Station>(sql)).ToList();
    }
}
