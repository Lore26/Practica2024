using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.OutputCaching;
using TrainsAPI.DTOs;
using TrainsAPI.Entities;
using TrainsAPI.Filters;
using TrainsAPI.Repositories;

namespace TrainsAPI.Endpoints;

public static class TrainTypesEndpoints
{
    public static RouteGroupBuilder MapTypes(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetTypes)
            .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("train-types-get"));
        group.MapGet("/{Id:int}", GetById);
        group.MapPost("/", Create)
            .AddEndpointFilter<ValidationFilter<CreateTrainTypeDTO>>()
            // .RequireAuthorization("isadmin")
            .WithOpenApi();
        group.MapPut("/{Id:int}", Update)
            .AddEndpointFilter<ValidationFilter<CreateTrainTypeDTO>>()
            .RequireAuthorization("isadmin")
            .WithOpenApi();
        group.MapDelete("/{Id:int}", Delete).RequireAuthorization("isadmin");
        return group;
    }

    private static async Task<Ok<List<TrainTypeDTO>>> GetTypes(ITrainTypeRepository repository,
        IMapper mapper)
    {
        var type = typeof(TrainTypesEndpoints);

        var trainTypes = await repository.GetAll();
        var trainTypesDTO = mapper.Map<List<TrainTypeDTO>>(trainTypes);
        return TypedResults.Ok(trainTypesDTO);
    }

    private static async Task<Results<Ok<TrainTypeDTO>, NotFound>> GetById(
        [AsParameters] GetTrainTypeByIdRequestDTO model)
    {
        var trainType = await model.Repository.GetById(model.Id);

        if (trainType is null)
        {
            return TypedResults.NotFound();
        }

        var trainTypeDTO = model.Mapper.Map<TrainTypeDTO>(trainType);

        return TypedResults.Ok(trainTypeDTO);
    }

    private static async Task<Results<Created<TrainTypeDTO>, ValidationProblem>> Create(
        CreateTrainTypeDTO createTrainTypeDTO, [AsParameters] CreateTrainTypeRequestDTO model)
    {
        var traintype = model.Mapper.Map<TrainType>(createTrainTypeDTO);
        var id = await model.TrainTypeRepository.Create(traintype);
        await model.OutputCacheStore.EvictByTagAsync("traintypes-get", default);
        var trainTypeDTO = model.Mapper.Map<TrainTypeDTO>(traintype);
        return TypedResults.Created($"/traintypes/{id}", trainTypeDTO);
    }

    private static async Task<Results<NotFound, NoContent>> Update(int id,
        CreateTrainTypeDTO createTrainTypeDTO,
        ITrainTypeRepository repository,
        IOutputCacheStore outputCacheStore, IMapper mapper)
    {
        var exists = await repository.Exists(id);

        if (!exists)
        {
            return TypedResults.NotFound();
        }

        var trainType = mapper.Map<TrainType>(createTrainTypeDTO);
        trainType.Id = id;

        await repository.Update(trainType);
        await outputCacheStore.EvictByTagAsync("traintypes-get", default);
        return TypedResults.NoContent();
    }

    private static async Task<Results<NotFound, NoContent>> Delete(int id, ITrainTypeRepository repository,
        IOutputCacheStore outputCacheStore)
    {
        var exists = await repository.Exists(id);

        if (!exists)
        {
            return TypedResults.NotFound();
        }

        await repository.Delete(id);
        await outputCacheStore.EvictByTagAsync("traintypes-get", default);
        return TypedResults.NoContent();
    }
}
