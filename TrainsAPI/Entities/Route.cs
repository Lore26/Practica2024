namespace TrainsAPI.Entities;

public class Route
{
    public int Id { get; set; }
    public int TrainId { get; set; }
    public int DepartureStationId { get; set; }
    public int ArrivalStationId { get; set; }
    
    public List<RouteStation> RouteStations { get; set; } = null!;
}
