using System.ComponentModel.DataAnnotations;

namespace GameQuest.ViewModels;

public class AdminGameEditViewModel
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = "";

    [Display(Name = "Görsel URL")]
    [StringLength(255)]
    public string? ImageUrl { get; set; }

    [Required]
    [Display(Name = "Kategori")]
    public int CategoryId { get; set; }
}

