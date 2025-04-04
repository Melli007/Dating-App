using System;

namespace API.DTOs;

public class UserLocationDto
{
    
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double MaxDistance { get; set; } = 50;
    public bool LocationEnabled { get; set; } = true;
}
