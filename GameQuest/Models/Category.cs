using System;
using System.Collections.Generic;

namespace GameQuest.Models;

public partial class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
}
