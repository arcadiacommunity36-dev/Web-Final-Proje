using GameQuest.Models;
using GameQuest.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GameQuest.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly GameQuestDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileController(GameQuestDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await GetOrCreateDomainUserAsync();

        var profile = await _db.UserProfiles.FirstAsync(p => p.UserId == user.Id);
        var gameProfiles = await _db.UserGameProfiles
            .AsNoTracking()
            .Include(p => p.Game)
            .Where(p => p.UserId == user.Id && p.IsActive)
            .OrderByDescending(p => p.UpdatedAt)
            .ToListAsync();

        return View(new ProfileIndexViewModel
        {
            User = user,
            Profile = profile,
            GameProfiles = gameProfiles
        });
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var user = await GetOrCreateDomainUserAsync();
        var profile = await _db.UserProfiles.FirstAsync(p => p.UserId == user.Id);

        return View(new ProfileEditViewModel
        {
            Bio = profile.Bio,
            AvatarUrl = profile.AvatarUrl,
            Country = profile.Country,
            TimeZone = profile.TimeZone
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProfileEditViewModel input)
    {
        if (!ModelState.IsValid)
        {
            return View(input);
        }

        var user = await GetOrCreateDomainUserAsync();
        var profile = await _db.UserProfiles.FirstAsync(p => p.UserId == user.Id);

        profile.Bio = input.Bio?.Trim();
        profile.AvatarUrl = input.AvatarUrl?.Trim();
        profile.Country = input.Country?.Trim();
        profile.TimeZone = input.TimeZone?.Trim();

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> AddGame()
    {
        var games = await _db.Games
            .AsNoTracking()
            .OrderBy(g => g.Name)
            .ToListAsync();

        ViewBag.Games = new SelectList(games, "Id", "Name");
        return View(new UserGameProfileEditViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddGame(UserGameProfileEditViewModel input)
    {
        var games = await _db.Games
            .AsNoTracking()
            .OrderBy(g => g.Name)
            .ToListAsync();
        ViewBag.Games = new SelectList(games, "Id", "Name", input.GameId);

        if (!ModelState.IsValid)
        {
            return View(input);
        }

        var user = await GetOrCreateDomainUserAsync();

        var existing = await _db.UserGameProfiles
            .FirstOrDefaultAsync(p => p.UserId == user.Id && p.GameId == input.GameId);

        if (existing is null)
        {
            _db.UserGameProfiles.Add(new UserGameProfile
            {
                UserId = user.Id,
                GameId = input.GameId,
                Rank = input.Rank?.Trim(),
                PreferredRole = input.PreferredRole?.Trim(),
                ExperienceLevel = input.ExperienceLevel,
                HoursPlayed = input.HoursPlayed,
                IsActive = true,
                UpdatedAt = DateTime.UtcNow
            });
        }
        else
        {
            existing.Rank = input.Rank?.Trim();
            existing.PreferredRole = input.PreferredRole?.Trim();
            existing.ExperienceLevel = input.ExperienceLevel;
            existing.HoursPlayed = input.HoursPlayed;
            existing.IsActive = true;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> EditGame(int id)
    {
        var user = await GetOrCreateDomainUserAsync();
        var profile = await _db.UserGameProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == user.Id);

        if (profile is null)
        {
            return NotFound();
        }

        var games = await _db.Games
            .AsNoTracking()
            .OrderBy(g => g.Name)
            .ToListAsync();

        ViewBag.Games = new SelectList(games, "Id", "Name", profile.GameId);

        return View(new UserGameProfileEditViewModel
        {
            GameId = profile.GameId,
            Rank = profile.Rank,
            PreferredRole = profile.PreferredRole,
            ExperienceLevel = profile.ExperienceLevel,
            HoursPlayed = profile.HoursPlayed
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditGame(int id, UserGameProfileEditViewModel input)
    {
        var games = await _db.Games
            .AsNoTracking()
            .OrderBy(g => g.Name)
            .ToListAsync();
        ViewBag.Games = new SelectList(games, "Id", "Name", input.GameId);

        if (!ModelState.IsValid)
        {
            return View(input);
        }

        var user = await GetOrCreateDomainUserAsync();
        var profile = await _db.UserGameProfiles.FirstOrDefaultAsync(p => p.Id == id && p.UserId == user.Id);
        if (profile is null)
        {
            return NotFound();
        }

        profile.GameId = input.GameId;
        profile.Rank = input.Rank?.Trim();
        profile.PreferredRole = input.PreferredRole?.Trim();
        profile.ExperienceLevel = input.ExperienceLevel;
        profile.HoursPlayed = input.HoursPlayed;
        profile.IsActive = true;
        profile.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task<User> GetOrCreateDomainUserAsync()
    {
        var identityId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(identityId))
        {
            throw new InvalidOperationException("Giriş yapılmış kullanıcı bulunamadı.");
        }

        var user = await _db.Users.FirstOrDefaultAsync(u => u.IdentityUserId == identityId);
        if (user is null)
        {
            var identityUser = await _userManager.GetUserAsync(User);
            var email = identityUser?.Email ?? User.Identity?.Name ?? "user";
            var username = email.Contains('@') ? email.Split('@', 2)[0] : email;

            user = new User
            {
                IdentityUserId = identityId,
                Username = username,
                Email = email,
                IsActive = true
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        var hasProfile = await _db.UserProfiles.AnyAsync(p => p.UserId == user.Id);
        if (!hasProfile)
        {
            _db.UserProfiles.Add(new UserProfile
            {
                UserId = user.Id,
                IsActive = true
            });
            await _db.SaveChangesAsync();
        }

        return user;
    }
}

