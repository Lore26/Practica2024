using TrainsAPI.Utilities;

namespace TrainsAPI.DTOs;

public class PaginationDTO
{
    public const int PageInitialValue = 1;
    public const int RecordsPerPageInitialValue = 10;
    public int Page { get; init; } = 1;
    private readonly int _recordsPerPage = 10;
    private const int RecordsPerPageMax = 50;

    public int RecordsPerPage
    {
        get => _recordsPerPage;
        init => _recordsPerPage = value > RecordsPerPageMax ? RecordsPerPageMax : value;
    }

    public static ValueTask<PaginationDTO> BindAsync(HttpContext context)
    {
        // nameof(Page) = "Page"
        var page = context.ExtractValueOrDefault(nameof(Page), PageInitialValue);
        var recordsPerPage = context.ExtractValueOrDefault(nameof(RecordsPerPage),
            RecordsPerPageInitialValue);

        var response = new PaginationDTO
        {
            Page = page,
            RecordsPerPage = recordsPerPage
        };

        return ValueTask.FromResult(response);
    }
}