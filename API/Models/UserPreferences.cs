using System;

namespace API.Models;

public class UserPreferences
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public AppUsers User { get; set; } = null!;
    
    // Matching Preferences
    public string InterestedIn { get; set; } = "All"; // "Male", "Female", "All"
    public int MinAge { get; set; } = 18;
    public int MaxAge { get; set; } = 100;
    
    // Advanced Preferences
    public List<string> Hobbies { get; set; } = new();
    public bool ShowVerifiedOnly { get; set; } = false;
    public bool HideProfileFromSearch { get; set; } = false;
    
    // Special Features
    public bool BoostActive { get; set; } = false;
    public DateTime? BoostExpires { get; set; }
    public int DailySwipesRemaining { get; set; } = 100;
    public DateTime SwipesResetDate { get; set; } = DateTime.UtcNow.AddDays(1);
}
