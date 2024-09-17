using System.Data;
using Core.Filters;
using Core.Interfaces;
using Core.Models;
using Dapper;

namespace DataAccess.Repositories;

public class TrainRepository(IDbConnection dbConnection) : ITrainRepository
{
    public async Task<List<Train>> GetAll()
    {
        const string sql = """
                           SELECT
                               t.Id,
                               t.Name,
                               t.Capacity,
                               tt.Id,
                               tt.Name
                           FROM Trains t
                           JOIN TrainTypes tt ON t.TypeId = tt.Id
                           """;
        return (await dbConnection.QueryAsync<Train, TrainType, Train>(sql, (train, trainType) =>
        {
            train.TrainType = trainType;
            return train;
        }, splitOn: "Id")).ToList();
    }

    public async Task<Train?> GetById(int id)
    {
        var sql = $"""
                   SELECT
                       t.Id,
                       t.Name,
                       t.Capacity,
                       tt.Id,
                       tt.Name
                   FROM Trains t
                   JOIN TrainTypes tt ON t.TypeId = tt.Id
                   WHERE t.Id = {id}
                   """;
        return (await dbConnection.QueryAsync<Train, TrainType, Train>(sql, (train, trainType) =>
        {
            train.TrainType = trainType;
            return train;
        }, splitOn: "Id")).FirstOrDefault();
    }

    public async Task<bool> Exists(int id)
    {
        const string sql = "SELECT Id FROM Trains WHERE Id = @Id";
        return await dbConnection.ExecuteScalarAsync<bool>(sql, new { id });
    }

    public async Task<List<int>> ExistAll(int[] ids)
    {
        const string sql = "SELECT Id FROM Trains WHERE Id = ANY(${ids})";
        var foundIds = await dbConnection.QueryAsync<int>(sql, new { ids });

        return foundIds.ToList();
    }

    public async Task<int> Create(Train entity)
    {
        var sql = $"""
                   INSERT INTO Trains (Name, Capacity, TypeId)
                   VALUES ({entity.Name}, {entity.Capacity}, {entity.TrainType.Id});
                   SELECT SCOPE_IDENTITY();
                   """;
        return await dbConnection.QuerySingleAsync<int>(sql);
    }

    public async Task<bool> Update(Train entity)
    {
        var sql = $"""
                   UPDATE Trains
                   SET Name = {entity.Name}, Capacity = {entity.Capacity}, TypeId = {entity.TrainType.Id}
                   WHERE Id = {entity.Id};
                   SELECT SCOPE_IDENTITY();
                   """;
        return await dbConnection.ExecuteAsync(sql, entity) > 0;
    }

    public async Task<bool> Delete(int id)
    {
        const string sql = "DELETE FROM Trains WHERE Id = @Id";
        return await dbConnection.ExecuteAsync(sql, new { id }) > 0;
    }

    public async Task<List<Train>> Filter(TrainFilter filter)
    {
        var sql = """
                  SELECT
                      t.Id,
                      t.Name,
                      t.Capacity,
                      tt.Id,
                      tt.Name
                  FROM Trains t
                  JOIN TrainTypes tt ON t.TypeId = tt.Id
                  WHERE 1 = 1
                  """;

        if (!string.IsNullOrEmpty(filter.Name))
        {
            sql += $" AND t.Name LIKE '%{filter.Name}%'";
        }

        if (filter.TrainTypeId.HasValue)
        {
            sql += $" AND t.TypeId = {filter.TrainTypeId}";
        }

        return (await dbConnection.QueryAsync<Train, TrainType, Train>(sql, (train, trainType) =>
        {
            train.TrainType = trainType;
            return train;
        }, splitOn: "Id")).ToList();
    }

    public async Task<List<Train>> GetByType(int typeId)
    {
        var sql = $"""
                   SELECT
                       t.Id,
                       t.Name,
                       t.Capacity,
                       tt.Id,
                       tt.Name
                   FROM Trains t
                   JOIN TrainTypes tt ON t.TypeId = tt.Id
                   WHERE t.TypeId = {typeId}
                   """;
        return (await dbConnection.QueryAsync<Train, TrainType, Train>(sql, (train, trainType) =>
        {
            train.TrainType = trainType;
            return train;
        }, splitOn: "Id")).ToList();
    }

    public async Task<TrainType?> GetTrainType(int trainId)
    {
        var sql = $"""
                   SELECT
                       tt.Id,
                       tt.Name
                   FROM Trains t
                   JOIN TrainTypes tt ON t.TypeId = tt.Id
                   WHERE t.Id = {trainId}
                   """;
        return (await dbConnection.QueryAsync<TrainType>(sql)).FirstOrDefault();
    }

    public async Task<List<TrainType>> GetTrainTypes()
    {
        const string sql = "SELECT Id, Name FROM TrainTypes";
        return (await dbConnection.QueryAsync<TrainType>(sql)).ToList();
    }

    public async Task<bool> TrainTypeExists(int typeId)
    {
        const string sql = "SELECT Id FROM TrainTypes WHERE Id = @Id";
        return await dbConnection.ExecuteScalarAsync<bool>(sql, new { typeId });
    }

    public async Task<int> CreateTrainType(TrainType trainType)
    {
        var sql = $"""
                   INSERT INTO TrainTypes (Name)
                   VALUES ({trainType.Name});
                   SELECT SCOPE_IDENTITY();
                   """;
        return await dbConnection.QuerySingleAsync<int>(sql, trainType);
    }

    public async Task<bool> UpdateTrainType(int id, TrainType trainType)
    {
        var sql = $"""
                   UPDATE TrainTypes
                   SET Name = {trainType.Name}
                   WHERE Id = {id};
                   SELECT SCOPE_IDENTITY();
                   """;
        return await dbConnection.ExecuteAsync(sql, trainType) > 0;
    }

    public async Task<bool> UpdateTrainType(TrainType trainType)
    {
        var sql = $"""
                   UPDATE TrainTypes
                   SET Name = {trainType.Name}
                   WHERE Id = {trainType.Id};
                   SELECT SCOPE_IDENTITY();
                   """;
        return await dbConnection.ExecuteAsync(sql, trainType) > 0;
    }

    public async Task<bool> DeleteTrainType(int typeId)
    {
        const string sql = "DELETE FROM TrainTypes WHERE Id = @Id";
        return await dbConnection.ExecuteAsync(sql, new { typeId }) > 0;
    }
}
