namespace TrainsAPI.Controllers;

using Core.Filters;
using Core.Models;
using Core.Services;
using DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Route = Core.Models.Route;

[Route("api/[controller]")]
[ApiController]
public class RoutesController(RouteService routesService, StationService stationService) : ControllerBase
{

    [HttpGet]
    public async Task<Ok<List<RouteDTO>>> Get()
    {
        var routes = await routesService.GetAllAsync();
        var routesDTO = routes.Select(r => new RouteDTO
        {
            Id = r.Id,
            ArrivalStationId = r.ArrivalStationId,
            DepartureStationId = r.ArrivalStationId,
            RouteStations = r.RouteStations.Select(rs => new RouteStationDTO
            {
                Id = rs.StationId,
                Name = rs.Station.Name,
                Order = rs.Order,
                ArrivalTime = rs.ArrivalTime
            }).ToList()
        }).ToList();
        return TypedResults.Ok(routesDTO);
    }

    [HttpGet("{id:int}")]
    public async Task<Results<Ok<RouteDTO>, NotFound>> GetById(int id)
    {
        var route = await routesService.GetByIdAsync(id);

        if (route is null)
        {
            return TypedResults.NotFound();
        }

        var routeDTO = new RouteDTO
        {
            Id = route.Id,
            ArrivalStationId = route.ArrivalStationId,
            DepartureStationId = route.ArrivalStationId,
            RouteStations = route.RouteStations.Select(rs => new RouteStationDTO
            {
                Id = rs.StationId,
                Name = rs.Station.Name,
                Order = rs.Order,
                ArrivalTime = rs.ArrivalTime
            }).ToList()
        };
        return TypedResults.Ok(routeDTO);
    }

    [HttpPost]
    public async Task<Created<RouteDTO>> Create([FromForm] CreateRouteDTO createRouteDto)
    {
        var route = new Route
        {
            TrainId = createRouteDto.TrainId,
            ArrivalStationId = createRouteDto.ArrivalStationId,
            DepartureStationId = createRouteDto.DepartureStationId
        };

        var id = await routesService.CreateAsync(route);
        var routeDTO = new RouteDTO
        {
            Id = route.Id,
            ArrivalStationId = route.ArrivalStationId,
            DepartureStationId = route.ArrivalStationId,
            RouteStations = route.RouteStations.Select(rs => new RouteStationDTO
            {
                Id = rs.StationId,
                Name = rs.Station.Name,
                Order = rs.Order,
                ArrivalTime = rs.ArrivalTime
            }).ToList()
        };
        return TypedResults.Created($"routes/{id}", routeDTO);
    }

    [HttpPost("{id:int}/assignStations")]
    public async Task<Results<NotFound, NoContent, BadRequest<string>>> AssignStations(int id, List<AddStationToRouteDTO> stationsDTO)
    {
        if (stationsDTO.Count == 0)
        {
            return TypedResults.BadRequest("The stations list is empty.");
        }

        var route = await routesService.GetByIdAsync(id);

        if (route is null)
        {
            return TypedResults.NotFound();
        }

        var stationsIds = stationsDTO.Select(s => s.StationId).Distinct().ToList();
        var existingStations = await stationService.ExistAllAsync(stationsIds.ToArray());

        if (existingStations.Count != stationsIds.Count)
        {
            var nonExistingStations = stationsIds.Except(existingStations);
            var nonExistingStationsCsv = string.Join(",", nonExistingStations);
            return TypedResults.BadRequest($"The stations of Id {nonExistingStationsCsv} do not exist");
        }

        var routeStations = stationsDTO.Select(s => new RouteStation
        {
            StationId = s.StationId,
            ArrivalTime = s.ArrivalTime,
            Order = s.Order,
            RouteId = id
        }).ToList();

        await routesService.Assign(id, routeStations);
        return TypedResults.NoContent();
    }

    [HttpPut("{id:int}")]
    public async Task<Results<NoContent, NotFound>> Update(int id, [FromForm] CreateRouteDTO createRouteDto)
    {
        var route = await routesService.GetByIdAsync(id);

        if (route is null)
        {
            return TypedResults.NotFound();
        }

        await routesService.UpdateAsync(new Route
        {
            TrainId = createRouteDto.TrainId,
            ArrivalStationId = createRouteDto.ArrivalStationId,
            DepartureStationId = createRouteDto.DepartureStationId,
            Id = id
        });
        return TypedResults.NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<Results<NoContent, NotFound>> Delete(int id)
    {
        var route = await routesService.GetByIdAsync(id);

        if (route is null)
        {
            return TypedResults.NotFound();
        }

        await routesService.DeleteAsync(id);
        return TypedResults.NoContent();
    }

    [HttpGet("filter")]
    public async Task<Ok<List<RouteDTO>>> Filter([FromQuery] RouteFilter routeFilterDTO)
    {
        var routes = await routesService.FilterAsync(routeFilterDTO);
        var routesDTO = routes.Select(r => new RouteDTO
        {
            Id = r.Id,
            ArrivalStationId = r.ArrivalStationId,
            DepartureStationId = r.ArrivalStationId,
            TrainId = r.TrainId,
            RouteStations = r.RouteStations.Select(rs => new RouteStationDTO
            {
                Id = rs.StationId,
                Name = rs.Station.Name,
                Order = rs.Order,
                ArrivalTime = rs.ArrivalTime
            }).ToList()
        }).ToList();
        return TypedResults.Ok(routesDTO);
    }
}
