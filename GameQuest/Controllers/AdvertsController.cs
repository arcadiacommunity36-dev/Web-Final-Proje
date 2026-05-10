using GameQuest.Models;
using GameQuest.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GameQuest.Controllers;

public class AdvertsController : Controller
{
    private readonly GameQuestDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdvertsController(GameQuestDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int? gameId, int? categoryId)
    {
        var categories = await _db.Categories
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();

        var gamesQuery = _db.Games.AsNoTracking().Include(g => g.Category).OrderBy(g => g.Name).AsQueryable();
        if (categoryId is not null)
        {
            gamesQuery = gamesQuery.Where(g => g.CategoryId == categoryId.Value);
        }
        var games = await gamesQuery.ToListAsync();

        var query = _db.Adverts
            .AsNoTracking()
            .Include(a => a.Game).ThenInclude(g => g.Category)
            .Include(a => a.User)
            .Where(a => a.IsActive);

        if (categoryId is not null)
        {
            query = query.Where(a => a.Game.CategoryId == categoryId.Value);
        }

        if (gameId is not null)
        {
            query = query.Where(a => a.GameId == gameId.Value);
        }

        var adverts = await query
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

        ViewBag.Categories = categories;
        ViewBag.Games = games;
        ViewBag.SelectedCategoryId = categoryId;
        ViewBag.SelectedGameId = gameId;

        return View(adverts);
    }

    public async Task<IActionResult> Details(int id)
    {
        var advert = await _db.Adverts
            .AsNoTracking()
            .Include(a => a.Game).ThenInclude(g => g.Category)
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (advert is null)
        {
            return NotFound();
        }

        int? currentDomainUserId = null;
        if (User?.Identity?.IsAuthenticated == true)
        {
            var domainUser = await GetOrCreateDomainUserAsync();
            currentDomainUserId = domainUser.Id;
        }

        ViewBag.IsOwner = currentDomainUserId is not null && advert.UserId == currentDomainUserId.Value;
        ViewBag.CanApply = currentDomainUserId is not null && advert.IsActive && advert.UserId != currentDomainUserId.Value;

        if (currentDomainUserId is not null)
        {
            ViewBag.AlreadyApplied = await _db.Applications
                .AsNoTracking()
                .AnyAsync(ap => ap.AdvertId == advert.Id && ap.ApplicantId == currentDomainUserId.Value);
        }
        else
        {
            ViewBag.AlreadyApplied = false;
        }

        return View(advert);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadGamesAsync();
        return View(new AdvertCreateViewModel());
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AdvertCreateViewModel input)
    {
        await LoadGamesAsync(input.GameId);
        if (!ModelState.IsValid)
        {
            return View(input);
        }

        var user = await GetOrCreateDomainUserAsync();

        _db.Adverts.Add(new Advert
        {
            UserId = user.Id,
            GameId = input.GameId,
            Title = input.Title.Trim(),
            Description = input.Description.Trim(),
            AdvertType = input.AdvertType,
            RequiredRank = input.RequiredRank?.Trim(),
            RequiredRole = input.RequiredRole?.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Apply(int id)
    {
        var advert = await _db.Adverts
            .AsNoTracking()
            .Include(a => a.Game)
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == id && a.IsActive);

        if (advert is null)
        {
            return NotFound();
        }

        var user = await GetOrCreateDomainUserAsync();
        if (advert.UserId == user.Id)
        {
            return Forbid();
        }

        var already = await _db.Applications.AnyAsync(a => a.AdvertId == advert.Id && a.ApplicantId == user.Id);
        if (already)
        {
            return RedirectToAction(nameof(Details), new { id });
        }

        ViewBag.Advert = advert;
        return View(new ApplicationCreateViewModel());
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Apply(int id, ApplicationCreateViewModel input)
    {
        var advert = await _db.Adverts
            .Include(a => a.Game)
            .FirstOrDefaultAsync(a => a.Id == id && a.IsActive);

        if (advert is null)
        {
            return NotFound();
        }

        var user = await GetOrCreateDomainUserAsync();
        if (advert.UserId == user.Id)
        {
            return Forbid();
        }

        var already = await _db.Applications.AnyAsync(a => a.AdvertId == advert.Id && a.ApplicantId == user.Id);
        if (already)
        {
            return RedirectToAction(nameof(Details), new { id });
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Advert = advert;
            return View(input);
        }

        _db.Applications.Add(new Application
        {
            AdvertId = advert.Id,
            ApplicantId = user.Id,
            Message = input.Message?.Trim(),
            Status = 0,
            AppliedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task LoadGamesAsync(int? selectedId = null)
    {
        var games = await _db.Games
            .AsNoTracking()
            .Include(g => g.Category)
            .OrderBy(g => g.Name)
            .ToListAsync();

        ViewBag.Games = new SelectList(games.Select(g => new
        {
            g.Id,
            Name = $"{g.Name} ({g.Category?.Name})"
        }), "Id", "Name", selectedId);
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

