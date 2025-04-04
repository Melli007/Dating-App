using System;

namespace API.Models;

public enum SwipeType
{
    Like,
    Pass,
    SuperLike
}

public class SwipeAction
{
    public int Id { get; set; }
    public int SwiperId { get; set; }
    public AppUsers Swiper { get; set; } = null!;
    public int TargetId { get; set; }
    public AppUsers Target { get; set; } = null!;
    public SwipeType SwipeType { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
