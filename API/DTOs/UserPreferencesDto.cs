using System;

namespace API.DTOs;

public class UserPreferencesDto
{
    public string InterestedIn { get; set; } = "All";
    public int MinAge { get; set; } = 18;
    public int MaxAge { get; set; } = 100;
    public List<string> Hobbies { get; set; } = new();
    public bool ShowVerifiedOnly { get; set; } = false;
    public bool HideProfileFromSearch { get; set; } = false;
}
