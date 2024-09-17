namespace Core.Models;

public class Seat
{
    public int Id { get; set; }
    public int TrainId { get; set; }
    public int SeatNumber { get; set; }
    public bool IsReserved { get; set; }
    
    public Train Train { get; set; } = null!;
}
