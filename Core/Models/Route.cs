namespace Core.Models;

public class Route
{
    public int Id { get; init; }
    public int TrainId { get; init; }
    public int DepartureStationId { get; init; }
    public int ArrivalStationId { get; init; }

    public List<RouteStation> RouteStations { get; set; } = null!;
}
