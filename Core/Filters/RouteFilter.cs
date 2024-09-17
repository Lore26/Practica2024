namespace Core.Filters;

public class RouteFilter
{
    public int? TrainTypeId { get; set; }
    public int? FirstStationId { get; set; }
    public int? SecondStationId { get; set; }
    public DateTime? DepartureTime { get; set; }
}
