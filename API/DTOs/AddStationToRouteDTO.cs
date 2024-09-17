namespace TrainsAPI.DTOs;

public class AddStationToRouteDTO
{
    public int RouteId { get; set; }
    public int StationId { get; set; }
    public DateTime ArrivalTime { get; set; }
    public int Order { get; set; }
}
