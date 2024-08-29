using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using TrainsAPI.DTOs;
using TrainsAPI.Entities;
using TrainsAPI.Filters;
using TrainsAPI.Repositories;
using TrainsAPI.Services;

namespace TrainsAPI.Endpoints;

public static class TrainsEndpoints
{
    public static RouteGroupBuilder MapTrains(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetAll)
            .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("trains-get"));
        group.MapGet("/{Id:int}", GetById);
        group.MapPost("/", Create)
            .AddEndpointFilter<ValidationFilter<CreateTrainDTO>>()
            // .RequireAuthorization();
            .WithOpenApi();
        group.MapPut("/{Id:int}", Update)
            .AddEndpointFilter<ValidationFilter<CreateTrainDTO>>();
        // .RequireAuthorization();
        group.MapDelete("/{Id:int}", Delete);
        // .RequireAuthorization();
        return group;
    }

    private static async Task<Results<Ok<List<TrainDTO>>, NotFound>> GetAll(ITrainsRepository trainsRepository,
        IMapper mapper)
    {
        var trains = await trainsRepository.GetAll();
        var trainsDTO = mapper.Map<List<TrainDTO>>(trains);
        return TypedResults.Ok(trainsDTO);
    }

    private static async Task<Results<Ok<TrainDTO>, NotFound>> GetById(int id, ITrainsRepository trainsRepository,
        IMapper mapper)
    {
        var train = await trainsRepository.GetById(id);

        if (train is null)
        {
            return TypedResults.NotFound();
        }

        var trainDTO = mapper.Map<TrainDTO>(train);
        return TypedResults.Ok(trainDTO);
    }

    private static async Task<Results<Created<TrainDTO>, NotFound, BadRequest<string>>>
        Create([FromForm] CreateTrainDTO createTrainDTO, ITrainsRepository trainsRepository, IMapper mapper,
            IOutputCacheStore outputCacheStore)
    {
        var train = mapper.Map<Train>(createTrainDTO);
        var id = await trainsRepository.Create(train);
        await outputCacheStore.EvictByTagAsync("trains-get", default);
        var trainDTO = mapper.Map<TrainDTO>(train);
        return TypedResults.Created($"/train/{id}", trainDTO);
    }

    private static async Task<Results<NoContent, NotFound, ForbidHttpResult>>
        Update(int id, CreateTrainDTO createTrainDTO, IOutputCacheStore outputCacheStore,
            ITrainsRepository trainsRepository)
    {
        var trainFromDB = await trainsRepository.GetById(id);

        if (trainFromDB is null)
        {
            return TypedResults.NotFound();
        }

        trainFromDB.Capacity = createTrainDTO.Capacity;

        await trainsRepository.Update(trainFromDB);
        await outputCacheStore.EvictByTagAsync("trains-get", default);
        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound, ForbidHttpResult>>
        Delete(int id, ITrainsRepository trainsRepository, IOutputCacheStore outputCacheStore,
            IUsersService usersService)
    {
        var trainFromDB = await trainsRepository.GetById(id);

        if (trainFromDB is null)
        {
            return TypedResults.NotFound();
        }

        var user = await usersService.GetUser();

        if (user is null)
        {
            return TypedResults.NotFound();
        }

        await trainsRepository.Delete(id);
        await outputCacheStore.EvictByTagAsync("trains-get", default);
        return TypedResults.NoContent();
    }
}
