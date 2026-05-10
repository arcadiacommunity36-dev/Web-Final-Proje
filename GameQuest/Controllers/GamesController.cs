using GameQuest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameQuest.Controllers;

public class GamesController : Controller
{
    private readonly GameQuestDbContext _db;

    public GamesController(GameQuestDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(string? q, int? categoryId)
    {
        var categories = await _db.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();

        var query = _db.Games
            .AsNoTracking()
            .Include(g => g.Category)
            .AsQueryable();

        if (categoryId is not null)
        {
            query = query.Where(g => g.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            query = query.Where(g => g.Name.Contains(term));
        }

        var games = await query
            .OrderBy(g => g.Name)
            .ToListAsync();

        ViewBag.Categories = categories;
        ViewBag.Query = q;
        ViewBag.SelectedCategoryId = categoryId;

        return View(games);
    }

    public async Task<IActionResult> Details(int id)
    {
        var game = await _db.Games
            .AsNoTracking()
            .Include(g => g.Category)
            .Include(g => g.Adverts.Where(a => a.IsActive))
                .ThenInclude(a => a.User)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (game is null)
        {
            return NotFound();
        }

        return View(game);
    }
}

