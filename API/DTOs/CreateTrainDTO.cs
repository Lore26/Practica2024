namespace TrainsAPI.DTOs;

public class CreateTrainDTO
{
    public string Name { get; set; } = null!;
    public int Capacity { get; set; }
    public int TrainTypeId { get; set; }
}
