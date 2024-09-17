namespace TrainsAPI.DTOs;

public class RouteDTO
{
    public int Id { get; set; }
    public int TrainId { get; set; }
    public int DepartureStationId { get; set; }
    public int ArrivalStationId { get; set; }
    public List<RouteStationDTO> RouteStations { get; set; } = [];
}
