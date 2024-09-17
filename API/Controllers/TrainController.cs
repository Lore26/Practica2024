namespace TrainsAPI.Controllers;

using Core.Filters;
using Core.Models;
using Core.Services;
using DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class TrainController(TrainService trainService) : ControllerBase
{
    [HttpGet]
    public async Task<Ok<List<TrainDTO>>> Get()
    {
        var trains = await trainService.GetAllAsync();
        var trainsDTO = trains.Select(t => new TrainDTO
        {
            Id = t.Id,
            Name = t.Name,
            Capacity = t.Capacity,
            TrainTypeId = t.TrainType.Id,
            TrainTypeName = t.TrainType.Name
        }).ToList();
        return TypedResults.Ok(trainsDTO);
    }

    [HttpGet("{id:int}")]
    public async Task<Results<Ok<TrainDTO>, NotFound>> GetById(int id)
    {
        var train = await trainService.GetByIdAsync(id);

        if (train is null)
        {
            return TypedResults.NotFound();
        }

        var trainDTO = new TrainDTO
        {
            Id = train.Id,
            Name = train.Name,
            Capacity = train.Capacity,
            TrainTypeId = train.TrainType.Id,
            TrainTypeName = train.TrainType.Name
        };
        return TypedResults.Ok(trainDTO);
    }

    [HttpPost]
    public async Task<Results<Created<TrainDTO>, BadRequest>> Post(CreateTrainDTO trainDTO)
    {
        var train = new Train
        {
            Name = trainDTO.Name,
            Capacity = trainDTO.Capacity,
            TrainType = new TrainType { Id = trainDTO.TrainTypeId }
        };

        var createdTrainId = await trainService.CreateAsync(train);
        var createdTrain = await trainService.GetByIdAsync(createdTrainId);
        if (createdTrain == null)
        {
            return TypedResults.BadRequest();
        }

        var createdTrainDTO = new TrainDTO
        {
            Id = createdTrain.Id,
            Name = createdTrain.Name,
            Capacity = createdTrain.Capacity,
            TrainTypeId = createdTrain.TrainType.Id,
            TrainTypeName = createdTrain.TrainType.Name
        };
        return TypedResults.Created($"trains/{createdTrainDTO.Id}", createdTrainDTO);
    }

    [HttpPut("{id:int}")]
    public async Task<Results<Ok<TrainDTO>, NotFound>> Put(int id, CreateTrainDTO trainDTO)
    {
        var train = new Train
        {
            Id = id,
            Name = trainDTO.Name,
            Capacity = trainDTO.Capacity,
            TrainType = new TrainType { Id = trainDTO.TrainTypeId }
        };

        var updated = await trainService.UpdateAsync(train);
        if (!updated)
        {
            return TypedResults.NotFound();
        }

        var updatedTrain = await trainService.GetByIdAsync(id);
        if (updatedTrain == null)
        {
            return TypedResults.NotFound();
        }

        var updatedTrainDTO = new TrainDTO
        {
            Id = updatedTrain.Id,
            Name = updatedTrain.Name,
            Capacity = updatedTrain.Capacity,
            TrainTypeId = updatedTrain.TrainType.Id,
            TrainTypeName = updatedTrain.TrainType.Name
        };
        return TypedResults.Ok(updatedTrainDTO);
    }

    [HttpDelete("{id:int}")]
    public async Task<Results<NoContent, NotFound>> Delete(int id)
    {
        var deleted = await trainService.DeleteAsync(id);
        if (!deleted)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.NoContent();
    }

    [HttpGet("types")]
    public async Task<Ok<List<TrainTypeDTO>>> GetTrainTypes()
    {
        var trainTypes = await trainService.GetAllTrainTypesAsync();
        var trainTypesDTO = trainTypes.Select(tt => new TrainTypeDTO
        {
            Id = tt.Id,
            Name = tt.Name
        }).ToList();
        return TypedResults.Ok(trainTypesDTO);
    }

    [HttpGet("filter")]
    public async Task<Ok<List<TrainDTO>>> Filter([FromQuery] TrainFilter trainFilterDTO)
    {
        var trains = await trainService.FilterAsync(trainFilterDTO);
        var trainsDTO = trains.Select(t => new TrainDTO
        {
            Id = t.Id,
            Name = t.Name,
            Capacity = t.Capacity,
            TrainTypeId = t.TrainType.Id,
            TrainTypeName = t.TrainType.Name
        }).ToList();
        return TypedResults.Ok(trainsDTO);
    }

    [HttpGet("type/{id:int}")]
    public async Task<Ok<List<TrainDTO>>> GetByType(int id)
    {
        var trains = await trainService.GetByTypeAsync(id);
        var trainsDTO = trains.Select(t => new TrainDTO
        {
            Id = t.Id,
            Name = t.Name,
            Capacity = t.Capacity,
            TrainTypeId = t.TrainType.Id,
            TrainTypeName = t.TrainType.Name
        }).ToList();
        return TypedResults.Ok(trainsDTO);
    }
}
