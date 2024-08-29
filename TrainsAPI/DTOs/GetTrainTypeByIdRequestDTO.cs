using AutoMapper;
using TrainsAPI.Repositories;

namespace TrainsAPI.DTOs;

public class GetTrainTypeByIdRequestDTO
{
    public ITrainTypeRepository Repository { get; set; } = null!;
    public int Id { get; set; }
    public IMapper Mapper { get; set; } = null!;
}
