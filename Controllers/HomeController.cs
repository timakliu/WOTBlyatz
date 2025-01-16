using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using WOTBlyatz.Models;
using YWOTBlyatz.Data;

namespace WOTBlyatz.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index(string categoryFilter = "All", string sortBy = "name")
        {
            var filteredMods = _context.Mods.ToList();

            // Фильтрация по категории
            if (categoryFilter != "All")
            {
                var category = Enum.TryParse(categoryFilter, out ModCategory parsedCategory) ? parsedCategory : (ModCategory?)null;
                if (category.HasValue)
                {
                    filteredMods = filteredMods.Where(m => m.Categories.Contains(category.Value)).ToList();
                }
            }

            // Сортировка
            switch (sortBy)
            {
                case "name":
                    filteredMods = filteredMods.OrderBy(m => m.Name).ToList();
                    break;
                case "date":
                    filteredMods = filteredMods.OrderByDescending(m => m.DateAdded).ToList();
                    break;
                case "rating":
                    filteredMods = filteredMods.OrderByDescending(m => m.Rating).ToList();
                    break;
                default:
                    filteredMods = filteredMods.OrderBy(m => m.Name).ToList();
                    break;
            }

            // Сохраняем параметры сортировки и категории в ViewData
            ViewData["Category"] = categoryFilter;
            ViewData["SortBy"] = sortBy;

            return View(filteredMods);
        }





        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
