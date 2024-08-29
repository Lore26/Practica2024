using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using TrainsAPI.Repositories;

namespace TrainsAPI.DTOs;

public class CreateTrainTypeRequestDTO
{
    public IOutputCacheStore OutputCacheStore { get; set; } = null!;
    public ITrainTypeRepository TrainTypeRepository { get; set; } = null!;
    public IMapper Mapper { get; set; } = null!;
}
