using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using TrainsAPI.Entities;

namespace TrainsAPI.Repositories;

public class ErrorsRepository(IConfiguration configuration) : IErrorsRepository
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")!;

    public async Task<Guid> Create(Error error)
    {
        await using var connection = new SqlConnection(_connectionString);
        error.Id = Guid.NewGuid();
        const string sql = """
                           INSERT INTO Errors(Id, ErrorMessage, StackTrace, Date)
                           VALUES (@Id, @errorMessage, @stackTrace, @date);
                           """;
        await connection.ExecuteAsync(sql, new
        {
            error.Id,
            error.ErrorMessage,
            error.StackTrace,
            error.Date
        }, commandType: CommandType.StoredProcedure);
        return error.Id;
    }
}
