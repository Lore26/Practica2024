using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using TrainsAPI.DTOs;
using TrainsAPI.Entities;

namespace TrainsAPI.Repositories;

public class StationsRepository(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    : IStationsRepository
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async Task<int> Create(Station station)
    {
        await using var connection = new SqlConnection(_connectionString);
        const string sql = """
                           INSERT INTO Stations(Name, City, Country)
                           	VALUES (@Name, @City, @Country);
                           	SELECT SCOPE_IDENTITY();
                           """;
        var id = await connection.QuerySingleAsync<int>(sql, new { station.Name, station.City, station.Country });

        station.Id = id;
        return id;
    }

    public async Task<List<Station>> GetAll(PaginationDTO pagination)
    {
        await using var connection = new SqlConnection(_connectionString);
        const string sql = """
                           SELECT Id, Name, City, Country
                            FROM Stations
                            ORDER BY Country
                            OFFSET ((@page - 1) * @recordsPerPage) ROWS FETCH NEXT @recordsPerPage ROWS ONLY
                           """;
        var stations = await connection.QueryAsync<Station>(sql, new { pagination.Page, pagination.RecordsPerPage });

        var stationCount = await connection.QuerySingleAsync<int>("SELECT COUNT(*) FROM Stations");

        _httpContext.Response.Headers.Append("totalAmountOfRecords", stationCount.ToString());

        return stations.ToList();
    }

    public async Task<Station?> GetById(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        const string sql = """
                           SELECT Id, Name, City, Country
                           	FROM Stations
                           	WHERE Id = @Id;
                           """;
        return await connection.QueryFirstOrDefaultAsync<Station>(sql, new { id });
    }

    public async Task<bool> Exist(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        const string sql = """
                           IF EXISTS (SELECT 1 FROM Stations WHERE Id= @Id)
                           	Select 1
                           ELSE
                           	Select 0
                           """;
        return await connection.QuerySingleAsync<bool>(sql, new { id });
    }

    public async Task<List<int>> Exists(List<int> ids)
    {
        var dt = new DataTable();
        dt.Columns.Add("Id", typeof(int));

        foreach (var id in ids)
        {
            dt.Rows.Add(id);
        }

        await using var connection = new SqlConnection(_connectionString);
        const string sql = """
                           SELECT Id
                           	FROM Stations
                           	WHERE Id IN (SELECT Id FROM @stationIds);
                           """;
        var idsOfExistingStations = await connection.QueryAsync<int>(sql, new { stationIds = dt });
        return idsOfExistingStations.ToList();
    }

    public async Task Update(Station station)
    {
        await using var connection = new SqlConnection(_connectionString);
        const string sql = """
                           UPDATE Stations
                           	SET Name = @Name, City = @City, Country = @Country
                           	WHERE Id = @Id;
                           """;
        await connection.ExecuteAsync(sql, new
        {
            station.Id,
            station.Name,
            station.City,
            station.Country
        });
    }

    public async Task Delete(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        const string sql = """
                           DELETE FROM Stations
                           	WHERE Id = @Id;
                           """;
        await connection.ExecuteAsync(sql, new { id });
    }

    public async Task<List<Station>> GetByName(string name)
    {
        await using var connection = new SqlConnection(_connectionString);
        const string sql = """
                           SELECT *
                           FROM Stations
                           WHERE Name like '%' + @Name + '%';
                           """;
        var stations = await connection.QueryAsync<Station>(sql, new { name });
        return stations.ToList();
    }

    public async Task<List<Station>> GetByCity(string city)
    {
        await using var connection = new SqlConnection(_connectionString);
        const string sql = """
                           SELECT *
                           FROM Stations
                           WHERE City like '%' + @City + '%';
                           """;
        var stations = await connection.QueryAsync<Station>(sql, new { city });
        return stations.ToList();
    }

    public async Task<List<Station>> GetByCountry(string country)
    {
        await using var connection = new SqlConnection(_connectionString);
        const string sql = """
                           SELECT *
                           FROM Stations
                           WHERE Country like '%' + @Country + '%';
                           """;
        var stations = await connection.QueryAsync<Station>(sql, new { country });
        return stations.ToList();
    }
}
