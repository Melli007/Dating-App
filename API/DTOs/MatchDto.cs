using System;

namespace API.DTOs;

public class MatchDto
{
     public int MatchId { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string KnownAs { get; set; } = string.Empty;
    public int Age { get; set; }
    public string? PhotoUrl { get; set; }
    public string City { get; set; } = string.Empty;
    public DateTime MatchDate { get; set; }
    public DateTime LastActive { get; set; }
    public bool Seen { get; set; }
}
