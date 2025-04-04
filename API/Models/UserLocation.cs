using System;
using NetTopologySuite.Geometries;


namespace API.Models;

public class UserLocation
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public AppUsers User { get; set; } = null!;
    public Point Location { get; set; } = null!;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    public double MaxDistance { get; set; } = 50; // Default max distance in kilometers
    public bool LocationEnabled { get; set; } = true;
}