using System;
using API.Models;

namespace API.DTOs;

public class SwipeDto
{
    public int TargetId { get; set; }
    public SwipeType SwipeType { get; set; }
}
