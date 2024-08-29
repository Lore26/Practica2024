using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using TrainsAPI.Utilities;
using TrainsAPI.DTOs;
using TrainsAPI.Entities;
using TrainsAPI.Filters;
using TrainsAPI.Repositories;

namespace TrainsAPI.Endpoints;

public static class StationEndpoints
{
    public static RouteGroupBuilder MapStations(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetAll)
            .CacheOutput(c => c.Expire(TimeSpan.FromMinutes(1)).Tag("stations-get"))
            .AddPaginationParameters();

        group.MapGet("/{Id:int}", GetById);
        group.MapGet("getByName/{name}", GetByName);
        group.MapGet("getByCity/{city}", GetByCity);
        group.MapGet("getByCountry/{country}", GetByCountry);
        group.MapPost("/", Create)
            .DisableAntiforgery()
            .AddEndpointFilter<ValidationFilter<CreateStationDTO>>()
            // .RequireAuthorization("isadmin");
            .WithOpenApi();
        group.MapPut("/{Id:int}", Update)
            .DisableAntiforgery()
            .AddEndpointFilter<ValidationFilter<CreateStationDTO>>()
            .RequireAuthorization("isadmin");
        group.MapDelete("/{Id:int}", Delete).RequireAuthorization("isadmin");
        return group;
    }

    private static async Task<Ok<List<StationDTO>>> GetAll(IStationsRepository repository, IMapper mapper,
        PaginationDTO pagination)
    {
        var stations = await repository.GetAll(pagination);
        var stationDTO = mapper.Map<List<StationDTO>>(stations);
        return TypedResults.Ok(stationDTO);
    }

    private static async Task<Results<Ok<StationDTO>, NotFound>> GetById(int id, IStationsRepository repository,
        IMapper mapper)
    {
        var station = await repository.GetById(id);

        if (station is null)
        {
            return TypedResults.NotFound();
        }

        var stationDTO = mapper.Map<StationDTO>(station);
        return TypedResults.Ok(stationDTO);
    }

    private static async Task<Ok<List<StationDTO>>> GetByName(string name, IStationsRepository repository,
        IMapper mapper)
    {
        var stations = await repository.GetByName(name);
        var stationsDTO = mapper.Map<List<StationDTO>>(stations);
        return TypedResults.Ok(stationsDTO);
    }

    private static async Task<Ok<List<StationDTO>>> GetByCity(string city, IStationsRepository repository,
        IMapper mapper)
    {
        var stations = await repository.GetByCity(city);
        var stationsDTO = mapper.Map<List<StationDTO>>(stations);
        return TypedResults.Ok(stationsDTO);
    }

    private static async Task<Ok<List<StationDTO>>> GetByCountry(string country, IStationsRepository repository,
        IMapper mapper)
    {
        var stations = await repository.GetByCountry(country);
        var stationsDTO = mapper.Map<List<StationDTO>>(stations);
        return TypedResults.Ok(stationsDTO);
    }

    private static async Task<Created<StationDTO>> Create([FromForm] CreateStationDTO createStationDto,
        IStationsRepository repository, IOutputCacheStore outputCacheStore,
        IMapper mapper)
    {
        var station = mapper.Map<Station>(createStationDto);

        var id = await repository.Create(station);
        await outputCacheStore.EvictByTagAsync("stations-get", default);
        var stationDTO = mapper.Map<StationDTO>(station);
        return TypedResults.Created($"/stations/{id}", stationDTO);
    }

    private static async Task<Results<NoContent, NotFound>> Update(int id,
        [FromForm] CreateStationDTO createStationDto, IStationsRepository repository,
        IOutputCacheStore outputCacheStore,
        IMapper mapper)
    {
        var stationDB = await repository.GetById(id);

        if (stationDB is null)
        {
            return TypedResults.NotFound();
        }

        var stationForUpdate = mapper.Map<Station>(createStationDto);
        stationForUpdate.Id = id;

        await repository.Update(stationForUpdate);
        await outputCacheStore.EvictByTagAsync("stations-get", default);
        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> Delete(int id, IStationsRepository repository,
        IOutputCacheStore outputCacheStore)
    {
        var stationDb = await repository.GetById(id);

        if (stationDb is null)
        {
            return TypedResults.NotFound();
        }

        await repository.Delete(id);
        await outputCacheStore.EvictByTagAsync("stations-get", default);
        return TypedResults.NoContent();
    }
}
