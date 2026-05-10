using System;
using System.Collections.Generic;

namespace GameQuest.Models;

public partial class UserGameProfile
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int GameId { get; set; }

    public string? Rank { get; set; }

    public string? PreferredRole { get; set; }

    public int ExperienceLevel { get; set; }

    public int? HoursPlayed { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
