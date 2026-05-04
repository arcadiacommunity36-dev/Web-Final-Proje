using System;
using System.Collections.Generic;

namespace GameQuest.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? DiscordProfile { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Advert> Adverts { get; set; } = new List<Advert>();

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();
}
