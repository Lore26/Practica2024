namespace TrainsAPI.DTOs;

public class RouteStationDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime ArrivalTime { get; set; }
    public int Order { get; set; }
}
