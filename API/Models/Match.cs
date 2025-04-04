using System;

namespace API.Models;

public class Match
{
    public int Id { get; set; }
    public int User1Id { get; set; }
    public AppUsers User1 { get; set; } = null!;
    public int User2Id { get; set; }
    public AppUsers User2 { get; set; } = null!;
    public DateTime MatchDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public bool User1Seen { get; set; } = false;
    public bool User2Seen { get; set; } = false;
}
