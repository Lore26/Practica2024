using Dapper;
using Microsoft.Data.SqlClient;
using TrainsAPI.Entities;

namespace TrainsAPI.Repositories;

public class TrainsRepository(IConfiguration configuration) : ITrainsRepository
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")!;

    public async Task<int> Create(Train train)
    {
        await using var connection = new SqlConnection(_connectionString);
        const string sql = """
                           INSERT INTO Trains (Name, Capacity, TypeId)
                             VALUES (@Name, @Capacity, @TrainTypeId);
                             SELECT SCOPE_IDENTITY();
                           """;
        var id = await connection.QuerySingleAsync<int>(sql,
            new { train.Name, train.Capacity, train.TrainType.Id });

        train.Id = id;
        return id;
    }

    public async Task<List<Train>> GetAll()
    {
        await using var connection = new SqlConnection(_connectionString);
        const string sql = "SELECT Id, Name, Capacity, TypeId FROM Trains";
        var comments = await connection.QueryAsync<Train>(sql);
        return comments.ToList();
    }

    public async Task<Train?> GetById(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        const string sql = """
                           SELECT Id, Name, Capacity, TypeId
                             FROM Trains
                             WHERE Id = @id;
                           """;
        var comment = await connection.QueryFirstOrDefaultAsync<Train>(sql, new { id });
        return comment;
    }

    public async Task<bool> Exists(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        const string sql = "SELECT COUNT(1) FROM TrainTypes WHERE Id = @id";
        var exists = await connection.QuerySingleAsync<bool>(sql, new { id });

        return exists;
    }

    public async Task Update(Train train)
    {
        await using var connection = new SqlConnection(_connectionString);
        const string sql = """
                           UPDATE Trains
                             SET Name = @Name,
                                 Capacity = @Capacity,
                                 TypeId = @TrainTypeId
                             WHERE Id = @Id;
                           """;
        await connection.ExecuteAsync(sql, new { train.Name, train.Capacity, TrainTypeId = train.TrainType.Id, train.Id });
    }

    public async Task Delete(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync("DELETE FROM Trains WHERE Id = @id", new { id });
    }
}
