namespace TrainsAPI.DTOs;

public class CreateRouteDTO
{
    public string Name { get; set; } = null!;
    public int DepartureStationId { get; set; }
    public int ArrivalStationId { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public int TrainId { get; set; }
}
