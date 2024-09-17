namespace Core.Models;

public class Train
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public int Capacity { get; set; }
    public TrainType TrainType { get; set; } = null!;
}
