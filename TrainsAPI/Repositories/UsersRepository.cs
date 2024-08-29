using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;

namespace TrainsAPI.Repositories;

public class UsersRepository(IConfiguration configuration) : IUsersRepository
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")!;

    public async Task<IdentityUser?> GetByEmail(string normalizedEmail)
    {
        await using var connection = new SqlConnection(_connectionString);
        const string sql = "SELECT * FROM Users WHERE NormalizedEmail = @normalizedEmail";
        return await connection
            .QuerySingleOrDefaultAsync<IdentityUser>(sql, new { normalizedEmail });
    }

    public async Task<string> Create(IdentityUser user)
    {
        await using var connection = new SqlConnection(_connectionString);
        user.Id = Guid.NewGuid().ToString();
        const string sql = """
                           INSERT INTO Users(id, email, NormalizedEmail, UserName, NormalizedUserName, 
                           PasswordHash)
                           VALUES (@Id, @email, @normalizedEmail, @userName, @normalizedUserName, @passwordHash);
                           """;
        await connection.ExecuteAsync(sql,
            new
            {
                user.Id,
                user.Email,
                user.NormalizedEmail,
                user.UserName,
                user.NormalizedUserName,
                user.PasswordHash
            });

        return user.Id;
    }

    public async Task<IList<Claim>> GetClaims(IdentityUser user)
    {
        await using var connection = new SqlConnection(_connectionString);
        const string sql = """
                           SELECT ClaimType as [Type], ClaimValue as [Value]
                           FROM UsersClaims
                           WHERE UserId = @id;
                           """;
        var claims = await connection.QueryAsync<Claim>(sql,
            new { user.Id });
        return claims.ToList();
    }

    public async Task AssignClaims(IdentityUser user, IEnumerable<Claim> claims)
    {
        const string sql = """
                           INSERT INTO UsersClaims (UserId, ClaimType, ClaimValue) 
                            VALUES (@Id, @Type, @Value);
                           """;
        var parameters = claims.Select(x => new { user.Id, x.Type, x.Value });

        await using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(sql, parameters);
    }

    public async Task RemoveClaims(IdentityUser user, IEnumerable<Claim> claims)
    {
        const string sql = "DELETE UsersClaims WHERE UserId = @Id AND ClaimType = @Type";
        var parameters = claims.Select(x => new { user.Id, x.Type });

        await using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(sql, parameters);
    }
}