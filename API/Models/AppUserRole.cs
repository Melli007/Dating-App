using System;
using Microsoft.AspNetCore.Identity;

namespace API.Models;

public class AppUserRole : IdentityUserRole<int>
{
    public AppUsers User { get; set; } = null!;
    public AppRole Role { get; set; } = null!;
}
