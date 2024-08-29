using Microsoft.AspNetCore.Identity;

namespace TrainsAPI.Entities;

public class Ticket
{
    public int Id { get; set; }
    public int ScheduleId { get; set; }
    public DateTime PurchaseDate { get; set; }
    public int SeatId { get; set; }
    public int Price { get; set; }
    public string Status { get; set; } = null!;
    
    public string UserId { get; set; } = null!;
    public IdentityUser User { get; set; } = null!;
    
    public Schedule Schedule { get; set; } = null!;
    public Seat Seat { get; set; } = null!;
}
