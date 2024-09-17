namespace DataAccess.Repositories;

using System.Data;
using System.Runtime.InteropServices.ComTypes;
using Core.Filters;
using Core.Interfaces;
using Core.Models;
using Dapper;

public class RouteRepository(IDbConnection dbConnection) : IRouteRepository
{
    public async Task<List<Route>> GetAll()
    {
        const string sql = "SELECT Id, TrainId, DepartureStationId, ArrivalStationId FROM Routes";
        return (await dbConnection.QueryAsync<Route>(sql)).ToList();
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
        var route = await dbConnection.QueryMultipleAsync(sql, new { id });
        var routeEntity = route.ReadFirstOrDefault<Route>();
        if (routeEntity == null)
        {
            return null;
        }

        routeEntity.RouteStations = route.Read<RouteStation>().ToList();
        return routeEntity;
    }

    public async Task<bool> Exists(int id)
    {
        const string sql = "SELECT Id FROM Routes WHERE Id = @Id";
        return await dbConnection.ExecuteScalarAsync<bool>(sql, new { id });
    }

    public async Task<List<int>> ExistAll(int[] ids)
    {
        const string sql = "SELECT Id FROM Routes WHERE Id = ANY(${ids})";
        var foundIds = await dbConnection.QueryAsync<int>(sql);

        return foundIds.ToList();
    }


    public async Task<int> Create(Route entity)
    {
        var sql = $"""
                   INSERT INTO Routes (TrainId, DepartureStationId, ArrivalStationId)
                    VALUES ({entity.TrainId}, {entity.DepartureStationId}, {entity.ArrivalStationId});
                   SELECT SCOPE_IDENTITY();
                   """;
        return await dbConnection.QuerySingleAsync<int>(sql, entity);
    }

    public async Task<bool> Update(Route entity)
    {
        var sql = $"""
                   UPDATE Routes
                   SET TrainId = {entity.TrainId}, DepartureStationId = {entity.DepartureStationId}, ArrivalStationId = {entity.ArrivalStationId}
                   WHERE Id = {entity.Id};
                   SELECT SCOPE_IDENTITY();
                   """;
        return await dbConnection.ExecuteAsync(sql, entity) > 0;
    }

    public async Task<bool> Delete(int id)
    {
        const string sql = "DELETE FROM Routes WHERE Id = @Id";
        return await dbConnection.ExecuteAsync(sql, new { id }) > 0;
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

        const string sql = """
                           DELETE RouteStations WHERE RouteId = @routeId;

                           INSERT INTO RouteStations (RouteId, StationId, [Order], ArrivalTime)
                           SELECT RouteId, StationId, [Order], ArrivalTime FROM @stations
                           """;
        await dbConnection.ExecuteAsync(sql, new { routeId = id, stations = dt });
    }

    public async Task<List<Route>> Filter(RouteFilter filter)
    {
        var sql = """
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
                  WHERE 1 = 1
                  """;

        if (filter.FirstStationId.HasValue)
        {
            sql += $" AND rs1.StationId = {filter.FirstStationId}";
        }

        if (filter.SecondStationId.HasValue)
        {
            sql += $" AND rs2.StationId = {filter.SecondStationId}";
        }

        if (filter.TrainTypeId.HasValue)
        {
            sql += $" AND r.TrainId = {filter.TrainTypeId}";
        }

        if (filter.DepartureTime.HasValue)
        {
            sql += $" AND rs1.ArrivalTime >= {filter.DepartureTime}";
        }

        sql += " AND rd1.[Order] < rd2.[Order]";
        sql += " ORDER BY rs1.ArrivalTime";

        var routes = await dbConnection.QueryAsync<Route>(sql);

        return routes.ToList();
    }
}
