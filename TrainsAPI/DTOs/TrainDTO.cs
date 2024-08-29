namespace TrainsAPI.DTOs;

public class TrainDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int Capacity { get; set; }
    public int TrainTypeId { get; set; }
    public string TrainTypeName { get; set; } = null!;
}
