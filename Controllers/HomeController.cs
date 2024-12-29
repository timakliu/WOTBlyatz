using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using WOTBlyatz.Models;

namespace WOTBlyatz.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        private readonly List<Mod> mods = new()
    {
        new Mod
        {
            Id = 1,
            Name = "Mod 1",
            Description = "Этот мод добавляет новые возможности и улучшения в игровой процесс.",
            ImageUrl = "/Images/mod1.jpg",
            DownloadUrl = "/downloads/mod1.zip",
            IsSubscriptionRequired = false,
            DateAdded = DateTime.Now.AddDays(-10),
            Rating = 4.5,
            Categories = new List<ModCategory> { ModCategory.Textures, ModCategory.Models }
        },
        new Mod
        {
            Id = 2,
            Name = "Мод 2",
            Description = "Уникальный мод, который предоставляет доступ к новым инструментам.",
            ImageUrl = "/Images/mod2.jpg",
            DownloadUrl = "/downloads/mod2.zip",
            IsSubscriptionRequired = true,
            DateAdded = DateTime.Now.AddDays(-5),
            Rating = 4.8,
            Categories = new List<ModCategory> { ModCategory.Models }
        },
        new Mod
        {
            Id = 3,
            Name = "Мод 3",
            Description = "Добавляет графические эффекты и новые звуковые элементы.",
            ImageUrl = "/Images/mod3.jpg",
            DownloadUrl = "/downloads/mod3.zip",
            IsSubscriptionRequired = true,
            DateAdded = DateTime.Now.AddDays(-1),
            Rating = 4.2,
            Categories = new List<ModCategory> { ModCategory.Textures }
        },
         new Mod
        {
            Id = 3,
            Name = "aaa 2",
            Description = "Добавляет графические эффекты и новые звуковые элементы.",
            ImageUrl = "/Images/mod4.jpg",
            DownloadUrl = "/downloads/mod4.zip",
            IsSubscriptionRequired = true,
            DateAdded = DateTime.Now.AddDays(-51),
            Rating = 4.4,
            Categories = new List<ModCategory> { ModCategory.Textures, ModCategory.Banned, ModCategory.Models}
        }
    };

        public IActionResult Index(string categoryFilter = "All", string sortBy = "name")
        {
            var filteredMods = mods;

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
