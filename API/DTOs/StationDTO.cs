namespace TrainsAPI.DTOs;

public class StationDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Country { get; set; } = null!;
}
