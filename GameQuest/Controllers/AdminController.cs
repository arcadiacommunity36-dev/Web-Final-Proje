using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GameQuest.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GameQuest.ViewModels;

namespace GameQuest.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly GameQuestDbContext _db;

    public AdminController(GameQuestDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.UsersCount = await _db.Users.CountAsync();
        ViewBag.GamesCount = await _db.Games.CountAsync();
        ViewBag.CategoriesCount = await _db.Categories.CountAsync();
        ViewBag.AdvertsCount = await _db.Adverts.CountAsync();
        ViewBag.ApplicationsCount = await _db.Applications.CountAsync();

        return View();
    }

    public async Task<IActionResult> Games()
    {
        var games = await _db.Games
            .AsNoTracking()
            .Include(g => g.Category)
            .OrderBy(g => g.Name)
            .ToListAsync();

        return View(games);
    }

    [HttpGet]
    public async Task<IActionResult> CreateGame()
    {
        await LoadCategoriesAsync();
        return View(new AdminGameEditViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateGame(AdminGameEditViewModel input)
    {
        await LoadCategoriesAsync(input.CategoryId);
        if (!ModelState.IsValid)
        {
            return View(input);
        }

        _db.Games.Add(new Game
        {
            Name = input.Name.Trim(),
            ImageUrl = string.IsNullOrWhiteSpace(input.ImageUrl) ? null : input.ImageUrl.Trim(),
            CategoryId = input.CategoryId
        });

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Games));
    }

    [HttpGet]
    public async Task<IActionResult> EditGame(int id)
    {
        var game = await _db.Games.FirstOrDefaultAsync(g => g.Id == id);
        if (game is null)
        {
            return NotFound();
        }

        await LoadCategoriesAsync(game.CategoryId);

        return View(new AdminGameEditViewModel
        {
            Name = game.Name,
            ImageUrl = game.ImageUrl,
            CategoryId = game.CategoryId
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditGame(int id, AdminGameEditViewModel input)
    {
        await LoadCategoriesAsync(input.CategoryId);
        if (!ModelState.IsValid)
        {
            return View(input);
        }

        var game = await _db.Games.FirstOrDefaultAsync(g => g.Id == id);
        if (game is null)
        {
            return NotFound();
        }

        game.Name = input.Name.Trim();
        game.ImageUrl = string.IsNullOrWhiteSpace(input.ImageUrl) ? null : input.ImageUrl.Trim();
        game.CategoryId = input.CategoryId;

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Games));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteGame(int id)
    {
        var game = await _db.Games.FirstOrDefaultAsync(g => g.Id == id);
        if (game is null)
        {
            return NotFound();
        }

        _db.Games.Remove(game);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Games));
    }

    public async Task<IActionResult> Categories()
    {
        var categories = await _db.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync();

        return View(categories);
    }

    [HttpGet]
    public IActionResult CreateCategory()
    {
        return View(new AdminCategoryEditViewModel { IsActive = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(AdminCategoryEditViewModel input)
    {
        if (!ModelState.IsValid)
        {
            return View(input);
        }

        _db.Categories.Add(new Category
        {
            Name = input.Name.Trim(),
            IsActive = input.IsActive
        });

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Categories));
    }

    [HttpGet]
    public async Task<IActionResult> EditCategory(int id)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category is null)
        {
            return NotFound();
        }

        return View(new AdminCategoryEditViewModel
        {
            Name = category.Name,
            IsActive = category.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCategory(int id, AdminCategoryEditViewModel input)
    {
        if (!ModelState.IsValid)
        {
            return View(input);
        }

        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category is null)
        {
            return NotFound();
        }

        category.Name = input.Name.Trim();
        category.IsActive = input.IsActive;

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Categories));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category is null)
        {
            return NotFound();
        }

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Categories));
    }

    private async Task LoadCategoriesAsync(int? selectedId = null)
    {
        var categories = await _db.Categories
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();

        ViewBag.Categories = new SelectList(categories, "Id", "Name", selectedId);
    }
}
