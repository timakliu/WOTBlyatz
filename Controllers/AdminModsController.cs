using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WOTBlyatz.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YWOTBlyatz.Data;

[Authorize(Roles = "Admin")]
public class AdminModsController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminModsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Список модов
    public IActionResult Index()
    {
        var mods = _context.Mods.ToList();
        return View(mods);
    }

    // Страница создания мода
    public IActionResult Create()
    {
        return View();
    }

    // Обработка создания нового мода
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Mod mod)
    {
        if (ModelState.IsValid)
        {
            _context.Add(mod);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(mod);
    }

    // Страница редактирования мода
    public IActionResult Edit(int id)
    {
        var mod = _context.Mods.Find(id);
        if (mod == null)
        {
            return NotFound();
        }
        return View(mod);
    }

    // Обработка редактирования мода
    // Обработка редактирования мода
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Mod mod)
    {
        if (id != mod.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(mod);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Mods.Any(e => e.Id == mod.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Редирект на /admin/mods
            return RedirectToAction("Mods", "Admin");
        }
        return View(mod);
    }


    // Удаление мода
    public async Task<IActionResult> Delete(int id)
    {
        var mod = await _context.Mods.FindAsync(id);
        if (mod == null)
        {
            return NotFound();
        }

        _context.Mods.Remove(mod);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
