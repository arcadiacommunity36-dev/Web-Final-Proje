using System;
using System.Collections.Generic;

namespace GameQuest.Models;

public partial class Game
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public virtual ICollection<Advert> Adverts { get; set; } = new List<Advert>();

    public virtual Category Category { get; set; } = null!;
}
