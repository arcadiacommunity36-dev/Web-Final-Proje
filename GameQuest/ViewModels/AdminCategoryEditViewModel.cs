using System.ComponentModel.DataAnnotations;

namespace GameQuest.ViewModels;

public class AdminCategoryEditViewModel
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = "";

    [Display(Name = "Aktif mi?")]
    public bool IsActive { get; set; }
}

