using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using TrainsAPI.Utilities;
using TrainsAPI.DTOs;
using TrainsAPI.Entities;
using TrainsAPI.Filters;
using TrainsAPI.Repositories;
using Route = TrainsAPI.Entities.Route;

namespace TrainsAPI.Endpoints;

public static class RoutesEndpoints
{
    public static RouteGroupBuilder MapRoutes(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetAll)
            .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("routes-get"))
            .AddPaginationParameters();
        group.MapGet("/{Id:int}", GetById);

        group.MapGet("/filter", FilterRoutes).AddRoutesFilterParameters();

        group.MapPost("/", Create).DisableAntiforgery()
            .AddEndpointFilter<ValidationFilter<CreateRouteDTO>>()
            // .RequireAuthorization("isadmin")
            .WithOpenApi();
        group.MapPut("/{Id:int}", Update).DisableAntiforgery()
            .AddEndpointFilter<ValidationFilter<CreateRouteDTO>>()
            // .RequireAuthorization("isadmin")
            .WithOpenApi();
        group.MapDelete("/{Id:int}", Delete).RequireAuthorization("isadmin");
        group.MapPost("/{Id:int}/assignStations", AssignStations);
        // .RequireAuthorization("isadmin");
        return group;
    }

    private static async Task<Ok<List<RouteDTO>>> GetAll(IRoutesRepository repository,
        IMapper mapper, PaginationDTO pagination)
    {
        var routes = await repository.GetAll(pagination);
        var routesDTO = mapper.Map<List<RouteDTO>>(routes);
        return TypedResults.Ok(routesDTO);
    }

    private static async Task<Results<Ok<RouteDTO>, NotFound>> GetById(int id,
        IRoutesRepository repository, IMapper mapper)
    {
        var route = await repository.GetById(id);

        if (route is null)
        {
            return TypedResults.NotFound();
        }

        var routeDTO = mapper.Map<RouteDTO>(route);
        return TypedResults.Ok(routeDTO);
    }

    private static async Task<Created<RouteDTO>> Create([FromForm] CreateRouteDTO createRouteDto,
        IOutputCacheStore outputCacheStore, IMapper mapper, IRoutesRepository repository)
    {
        var route = mapper.Map<Route>(createRouteDto);

        var id = await repository.Create(route);
        await outputCacheStore.EvictByTagAsync("routes-get", default);
        var routeDTO = mapper.Map<RouteDTO>(route);
        return TypedResults.Created($"routes/{id}", routeDTO);
    }

    private static async Task<Results<NoContent, NotFound>> Update(int id,
        [FromForm] CreateRouteDTO createRouteDto, IRoutesRepository repository,
        IOutputCacheStore outputCacheStore, IMapper mapper)
    {
        var routeDb = await repository.GetById(id);

        if (routeDb is null)
        {
            return TypedResults.NotFound();
        }

        var routeForUpdate = mapper.Map<Route>(createRouteDto);
        routeForUpdate.Id = id;

        await repository.Update(routeForUpdate);
        await outputCacheStore.EvictByTagAsync("routes-get", default);
        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> Delete(int id, IRoutesRepository repository,
        IOutputCacheStore outputCacheStore)
    {
        var routeDb = await repository.GetById(id);

        if (routeDb is null)
        {
            return TypedResults.NotFound();
        }

        await repository.Delete(id);
        await outputCacheStore.EvictByTagAsync("routes-get", default);
        return TypedResults.NoContent();
    }

    private static async Task<Results<NotFound, NoContent, BadRequest<string>>> AssignStations
    (int id, List<AddStationToRouteDTO> stationsDTO, IRoutesRepository routesRepository,
        IStationsRepository stationsRepository, IMapper mapper)
    {
        if (!await routesRepository.Exists(id))
        {
            return TypedResults.NotFound();
        }

        var existingStations = new List<int>();
        var stationsIds = stationsDTO.Select(s => s.RouteStationId).ToList();

        if (stationsDTO.Count != 0)
        {
            existingStations = await stationsRepository.Exists(stationsIds);
        }

        if (existingStations.Count != stationsDTO.Count)
        {
            var nonExistingStations = stationsIds.Except(existingStations);
            var nonExistingStationsCsv = string.Join(",", nonExistingStations);
            return TypedResults.BadRequest($"The stations of Id {nonExistingStationsCsv} do not exists");
        }

        var stations = mapper.Map<List<RouteStation>>(stationsDTO);
        await routesRepository.Assign(id, stations);
        return TypedResults.NoContent();
    }

    private static async Task<Ok<List<RouteDTO>>> FilterRoutes(RouteFilterDTO routeFilterDTO,
        IRoutesRepository routesRepository, IMapper mapper)
    {
        var routes = await routesRepository.Filter(routeFilterDTO);
        var routesDTO = mapper.Map<List<RouteDTO>>(routes);
        return TypedResults.Ok(routesDTO);
    }
}