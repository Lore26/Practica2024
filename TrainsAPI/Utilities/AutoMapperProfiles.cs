using AutoMapper;
using TrainsAPI.DTOs;
using TrainsAPI.Entities;
using Route = TrainsAPI.Entities.Route;

namespace TrainsAPI.Utilities;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<TrainType, TrainTypeDTO>();
        CreateMap<CreateTrainTypeDTO, TrainType>();

        CreateMap<Station, StationDTO>();
        CreateMap<CreateStationDTO, Station>();
        CreateMap<Route, RouteDTO>()
            .ForMember(x => x.Stations, entity =>
                entity.MapFrom(p => p.RouteStations.Select(
                    gm => new TrainTypeDTO
                    {
                        Id = gm.RouteId,
                        Name = gm.Station.Name
                    })));
        CreateMap<CreateRouteDTO, Route>();

        CreateMap<AddStationToRouteDTO, RouteStation>();
        CreateMap<TrainDTO, Train>()
            .ForMember(x => x.TrainType, entity =>
                entity.MapFrom(p => new TrainType
                {
                    Id = p.TrainTypeId,
                    Name = p.TrainTypeName
                }));
        CreateMap<CreateTrainDTO, Train>();
    }
}
