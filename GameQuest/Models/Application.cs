using System;
using System.Collections.Generic;

namespace GameQuest.Models;

public partial class Application
{
    public int Id { get; set; }

    public int AdvertId { get; set; }

    public int ApplicantId { get; set; }

    public string? Message { get; set; }

    public byte Status { get; set; }

    public DateTime AppliedAt { get; set; }

    public virtual Advert Advert { get; set; } = null!;

    public virtual User Applicant { get; set; } = null!;
}
