using System.ComponentModel.DataAnnotations;

namespace GameQuest.ViewModels;

public class ProfileEditViewModel
{
    [StringLength(500)]
    public string? Bio { get; set; }

    [StringLength(255)]
    [Display(Name = "Avatar URL")]
    public string? AvatarUrl { get; set; }

    [StringLength(60)]
    public string? Country { get; set; }

    [StringLength(50)]
    public string? TimeZone { get; set; }
}

