using TrainsAPI.Utilities;

namespace TrainsAPI.DTOs;

public class RouteFilterDTO
{
    public int Page { get; set; }
    public int RecordsPerPage { get; set; }

    public PaginationDTO PaginationDTO => new() { Page = Page, RecordsPerPage = RecordsPerPage };

    public int StartStationId { get; set; }
    public int EndStationId { get; set; }
    public DateTime DepartureTime { get; set; }
    public int? TrainTypeId { get; set; }
    public bool IncludePast { get; set; }

    public static ValueTask<RouteFilterDTO> BindAsync(HttpContext context)
    {
        var page = context.ExtractValueOrDefault(nameof(Page),
            PaginationDTO.PageInitialValue);
        var recordsPerPage = context.ExtractValueOrDefault(nameof(RecordsPerPage),
            PaginationDTO.RecordsPerPageInitialValue);

        var startStation = context.ExtractValueOrDefault(nameof(StartStationId), 0);
        var endStation = context.ExtractValueOrDefault(nameof(EndStationId), 0);
        var departureTime = context.ExtractValueOrDefault(nameof(DepartureTime), DateTime.Now);
        var afterDepartureTime = context.ExtractValueOrDefault(nameof(IncludePast), false);
        var trainTypeId = context.ExtractValueOrDefault(nameof(TrainTypeId), 0);
        
        var response = new RouteFilterDTO
        {
            Page = page,
            RecordsPerPage = recordsPerPage,
            StartStationId = startStation,
            EndStationId = endStation,
            DepartureTime = departureTime,
            IncludePast = afterDepartureTime
        };

        return ValueTask.FromResult(response);
    }
}
