using System;
using System.Collections.Generic;

namespace GameQuest.Models;

public partial class UserProfile
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? Bio { get; set; }

    public string? AvatarUrl { get; set; }

    public string? Country { get; set; }

    public string? TimeZone { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual User User { get; set; } = null!;
}
