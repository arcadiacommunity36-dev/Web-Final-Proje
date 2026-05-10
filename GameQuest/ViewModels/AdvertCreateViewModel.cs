using System.ComponentModel.DataAnnotations;

namespace GameQuest.ViewModels;

public class AdvertCreateViewModel
{
    [Required]
    public int GameId { get; set; }

    [Required]
    [StringLength(150)]
    public string Title { get; set; } = "";

    [Required]
    public string Description { get; set; } = "";

    [Display(Name = "İlan türü")]
    public byte AdvertType { get; set; }

    [StringLength(50)]
    [Display(Name = "Gereken Rank")]
    public string? RequiredRank { get; set; }

    [StringLength(50)]
    [Display(Name = "Gereken Rol")]
    public string? RequiredRole { get; set; }
}

