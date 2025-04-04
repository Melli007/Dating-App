using System;

namespace API.DTOs;

public class NearbyUserDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string KnownAs { get; set; } = string.Empty;
    public int Age { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string? Introduction { get; set; }
    public string? Interests { get; set; }
    public double DistanceKm { get; set; }
    public DateTime LastActive { get; set; }
}
