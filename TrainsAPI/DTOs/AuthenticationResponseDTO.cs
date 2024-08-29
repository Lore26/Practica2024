namespace TrainsAPI.DTOs;

public class AuthenticationResponseDTO
{
    public string Token { get; set; } = null!;
    public DateTime Expiration { get; set; }
}
