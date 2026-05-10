using System.ComponentModel.DataAnnotations;

namespace GameQuest.ViewModels;

public class UserGameProfileEditViewModel
{
    [Required]
    public int GameId { get; set; }

    [StringLength(50)]
    public string? Rank { get; set; }

    [StringLength(50)]
    public string? PreferredRole { get; set; }

    [Range(0, 10)]
    public int ExperienceLevel { get; set; }

    [Range(0, 100000)]
    public int? HoursPlayed { get; set; }
}

