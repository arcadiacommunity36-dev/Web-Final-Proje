using System.ComponentModel.DataAnnotations;

namespace GameQuest.ViewModels;

public class ApplicationCreateViewModel
{
    [StringLength(500)]
    [Display(Name = "Mesaj")]
    public string? Message { get; set; }
}

