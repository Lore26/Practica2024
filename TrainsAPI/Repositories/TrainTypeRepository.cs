using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using TrainsAPI.Entities;

namespace TrainsAPI.Repositories;

public class TrainTypeRepository(IConfiguration configuration) : ITrainTypeRepository
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")!;

    public async Task<int> Create(TrainType trainType)
    {
        await using var connection = new SqlConnection(_connectionString);
        const string sql = """
                           INSERT INTO TrainTypes (Name)
                            VALUES (@Name);
                            SELECT SCOPE_IDENTITY();
                           """;
        var id = await connection.QuerySingleAsync<int>(sql, new { trainType.Name });
        trainType.Id = id;
        return id;
    }

    public async Task Delete(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync("DELETE FROM TrainTypes WHERE Id = @id", new { id },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> Exists(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        var exists =
            await connection.QuerySingleAsync<bool>("SELECT COUNT(1) FROM TrainTypes WHERE Id = @id", new { id });
        return exists;
    }

    public async Task<bool> Exists(int id, string name)
    {
        await using var connection = new SqlConnection(_connectionString);
        const string sql = "SELECT COUNT(1) FROM TrainTypes WHERE Id = @id AND Name = @name";
        var exists = await connection.QuerySingleAsync<bool>(sql, new { id, name });
        return exists;
    }

    public async Task<List<int>> Exists(List<int> ids)
    {
        var dt = new DataTable();
        dt.Columns.Add("Id", typeof(int));

        foreach (var typeId in ids)
        {
            dt.Rows.Add(typeId);
        }

        await using var connection = new SqlConnection(_connectionString);
        const string sql = """
                           SELECT Id
                           FROM TrainTypes
                           WHERE Id in (SELECT Id FROM @typesIds);;
                           """;
        var idsOfTypesThatExist = await connection.QueryAsync<int>(sql, new { typesIds = dt });

        return idsOfTypesThatExist.ToList();
    }

    public async Task<List<TrainType>> GetAll()
    {
        await using var connection = new SqlConnection(_connectionString);
        const string sql = """
                           SELECT Id, Name
                           From TrainTypes
                           ORDER BY Name
                           """;
        var genres = await connection.QueryAsync<TrainType>(sql);
        return genres.ToList();
    }

    public async Task<TrainType?> GetById(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        const string sql = """
                           SELECT Id, Name
                           FROM TrainTypes
                           WHERE Id = @id
                           """;
        return await connection.QueryFirstOrDefaultAsync<TrainType>(sql, new { id });
    }

    public async Task Update(TrainType trainType)
    {
        await using var connection = new SqlConnection(_connectionString);
        const string sql = """
                           UPDATE TrainTypes
                            SET Name = @Name
                            WHERE Id = @Id
                           """;
        await connection.ExecuteAsync(sql, new { trainType.Id, trainType.Name });
    }
}
