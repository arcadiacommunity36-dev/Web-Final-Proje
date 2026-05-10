using GameQuest.Models;

namespace GameQuest.ViewModels;

public class ProfileIndexViewModel
{
    public required User User { get; init; }
    public required UserProfile Profile { get; init; }
    public required List<UserGameProfile> GameProfiles { get; init; }
}

