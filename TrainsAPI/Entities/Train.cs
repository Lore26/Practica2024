namespace TrainsAPI.Entities;

public class Train
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int Capacity { get; set; }
    public TrainType TrainType { get; set; } = null!;
}
