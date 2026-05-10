using GameQuest.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameQuest.Controllers;

[Authorize]
public class ApplicationsController : Controller
{
    private readonly GameQuestDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public ApplicationsController(GameQuestDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    // Kullanıcının yaptığı başvurular
    public async Task<IActionResult> Mine()
    {
        var user = await GetOrCreateDomainUserAsync();

        var apps = await _db.Applications
            .AsNoTracking()
            .Include(a => a.Advert).ThenInclude(ad => ad.Game).ThenInclude(g => g.Category)
            .Include(a => a.Advert).ThenInclude(ad => ad.User)
            .Where(a => a.ApplicantId == user.Id)
            .OrderByDescending(a => a.AppliedAt)
            .ToListAsync();

        return View(apps);
    }

    // Kullanıcının ilanlarına gelen başvurular
    public async Task<IActionResult> Received()
    {
        var user = await GetOrCreateDomainUserAsync();

        var apps = await _db.Applications
            .AsNoTracking()
            .Include(a => a.Advert).ThenInclude(ad => ad.Game).ThenInclude(g => g.Category)
            .Include(a => a.Applicant)
            .Where(a => a.Advert.UserId == user.Id)
            .OrderByDescending(a => a.AppliedAt)
            .ToListAsync();

        return View(apps);
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

