using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using TrainsAPI.DTOs;
using TrainsAPI.Entities;
using Route = TrainsAPI.Entities.Route;

namespace TrainsAPI.Repositories;

public class RoutesRepository(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    : IRoutesRepository
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async Task<int> Create(Route route)
    {
        await using var connection = new SqlConnection(_connectionString);
        const string sql = """
                           INSERT INTO Routes(TrainId, DepartureStationId, ArrivalStationId)
                           	VALUES (@TrainId, @DepartureStationId, @ArrivalStationId);
                           	SELECT SCOPE_IDENTITY();
                           """;
        var id = await connection.QuerySingleAsync<int>(sql,
            new { route.TrainId, route.DepartureStationId, route.ArrivalStationId });
        route.Id = id;
        return id;
    }

    public async Task<List<Route>> GetAll(PaginationDTO paginationDTO)
    {
        await using var connection = new SqlConnection(_connectionString);
        const string sql = """
                           SELECT Id, TrainId, DepartureStationId, ArrivalStationId
                            FROM Routes
                            ORDER BY Id
                            OFFSET ((@page - 1) * @recordsPerPage) ROWS FETCH NEXT @recordsPerPage ROWS ONLY
                           """;
        var routes = await connection.QueryAsync<Route>(sql, new { paginationDTO.Page, paginationDTO.RecordsPerPage });

        var routesCount = await connection.QuerySingleAsync<int>("SELECT COUNT(*) FROM Routes");

        _httpContext.Response.Headers.Append("totalAmountOfRecords", routesCount.ToString());

        return routes.ToList();
    }

    public async Task<Route?> GetById(int id)
    {
        const string sql = """
                           SELECT Id, TrainId, DepartureStationId, ArrivalStationId
                           	FROM Routes
                           	WHERE Id = @Id;

                           SELECT Id, Name, ArrivalTime, [Order]
                            FROM Stations
                            INNER JOIN RouteStations ON Stations.Id = StationId
                            WHERE RouteId = @Id
                            ORDER BY [Order];
                           """;
        await using var connection = new SqlConnection(_connectionString);
        await using var multi = await connection.QueryMultipleAsync(sql, new { id });
        var route = await multi.ReadFirstAsync<Route>();
        var stations = await multi.ReadAsync<RouteStationDTO>();

        foreach (var station in stations)
        {
            route.RouteStations.Add(new RouteStation
            {
                RouteId = route.Id,
                Route = route,
                StationId = station.Id,
                Station = new Station
                {
                    Id = station.Id,
                    Name = station.Name
                },
                ArrivalTime = station.ArrivalTime,
                Order = station.Order
            });
        }

        return route;
    }

    public async Task<bool> Exists(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        return await connection.ExecuteScalarAsync<bool>("SELECT Id FROM Routes WHERE Id = @Id", new { id });
    }

    public async Task Update(Route route)
    {
        await using var connection = new SqlConnection(_connectionString);
        const string sql = """
                           UPDATE Routes
                           	SET TrainId = @TrainId, DepartureStationId = @DepartureStationId, ArrivalStationId = @ArrivalStationId
                           	WHERE Id = @Id
                           """;
        await connection.ExecuteAsync(sql, new
            {
                route.Id,
                route.TrainId,
                route.DepartureStationId,
                route.ArrivalStationId
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task Delete(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync("DELETE FROM Routes WHERE Id = @Id", new { id });
    }

    public async Task Assign(int id, List<RouteStation> stations)
    {
        for (var i = 0; i < stations.Count; i++)
        {
            stations[i].Order = i;
        }

        var dt = new DataTable();
        dt.Columns.Add("RouteId", typeof(int));
        dt.Columns.Add("StationId", typeof(string));
        dt.Columns.Add("Order", typeof(int));

        foreach (var routeStation in stations)
        {
            dt.Rows.Add(routeStation.RouteId, routeStation.StationId, routeStation.Order);
        }

        await using var connection = new SqlConnection(_connectionString);
        const string sql = """
                           DELETE RouteStations WHERE RouteId = @routeId;

                           INSERT INTO RouteStations (RouteId, StationId, [Order], ArrivalTime)
                           SELECT RouteId, StationId, [Order], ArrivalTime FROM @stations
                           """;
        await connection.ExecuteAsync(sql, new { routeId = id, stations = dt });
    }

    public async Task<List<Route>> Filter(RouteFilterDTO routeFilterDTO)
    {
        await using var connection = new SqlConnection(_connectionString);
        const string sql = """
                           SELECT DISTINCT r.Id,
                                           r.TrainId, 
                                           rs1.StationId AS first_station_id, 
                                           s1.Name AS first_station_name,
                                           rs1.ArrivalTime AS departure_time,
                                           rs2.StationId AS second_station_id,
                                           s2.Name AS second_station_name,
                                           rs2.ArrivalTime AS arrival_time
                           FROM Routes r
                           INNER JOIN RouteStations rs1 ON r.Id = rs1.RouteId
                           INNER JOIN RouteStations rs2 ON r.Id = rs2.RouteId
                           INNER JOIN Stations s1 ON rs1.StationId = s1.Id
                           INNER JOIN Stations s2 ON rs2.StationId = s2.Id
                           WHERE rs1.StationId = @FirstStationId
                             AND rs2.StationId = @SecondStationId
                             AND rs1.order < rs2.order -- Ensure the first station comes before the second station in the route
                             AND rs1.ArrivalTime > @DepartureAfterTime -- Ensure departure time is after the specified time
                           ORDER BY rs1.ArrivalTime;
                           """;
        var routes = await connection.QueryAsync<Route>(sql,
            new
            {
                routeFilterDTO.Page,
                routeFilterDTO.RecordsPerPage,
                routeFilterDTO.StartStationId,
                routeFilterDTO.EndStationId,
                routeFilterDTO.DepartureTime
            });

        var routesCount = await connection
            .QuerySingleAsync<int>("SELECT COUNT(*) FROM Routes",
                new
                {
                    routeFilterDTO.StartStationId,
                    routeFilterDTO.EndStationId,
                    routeFilterDTO.DepartureTime
                });

        _httpContext.Response.Headers.Append("totalAmountOfRecords",
            routesCount.ToString());

        return routes.ToList();
    }
}
