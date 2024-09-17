namespace Core.Models;

public class RouteStation
{
    public int RouteId { get; set; }
    public Route Route { get; set; } = null!;
    public int StationId { get; init; }
    public Station Station { get; set; } = null!;

    public DateTime ArrivalTime { get; init; }
    public int Order { get; set; }
}
