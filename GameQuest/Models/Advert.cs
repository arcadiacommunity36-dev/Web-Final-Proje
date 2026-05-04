using System;
using System.Collections.Generic;

namespace GameQuest.Models;

public partial class Advert
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int GameId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public byte AdvertType { get; set; }

    public string? RequiredRank { get; set; }

    public string? RequiredRole { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual Game Game { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
