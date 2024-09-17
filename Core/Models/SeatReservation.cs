namespace Core.Models;

public class SeatReservation
{
    public int Id { get; set; }
    public int SeatId { get; set; }
    public int TicketId { get; set; }
    
    public Seat Seat { get; set; } = null!;
    public Ticket Ticket { get; set; } = null!;
}
