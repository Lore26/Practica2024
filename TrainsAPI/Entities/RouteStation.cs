namespace TrainsAPI.Entities;

public class RouteStation
{
    public int RouteId { get; set; }
    public Route Route { get; set; } = null!;
    public int StationId { get; set; }
    public Station Station { get; set; } = null!;
    
    public DateTime ArrivalTime { get; set; }
    public int Order { get; set; }
}
