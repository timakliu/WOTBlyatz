﻿using Microsoft.AspNetCore.Authorization;
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

        private readonly List<Mod> mods = new() {
            new Mod {
                Id = 1,
                Name = "Mod 1",
                Description = "Этот мод добавляет новые возможности и улучшения в игровой процесс, делая его более увлекательным и разнообразным.",
                ImageUrl = "/Images/mod1.jpg",
                DownloadUrl = "/downloads/mod1.zip",
                IsSubscriptionRequired = false

            },
            new Mod {
                Id = 2,
                Name = "Мод 2",
                Description = "Уникальный мод, который предоставляет игрокам доступ к новым инструментам, персонажам и локациям.",
                ImageUrl = "/Images/mod2.jpg",
                DownloadUrl = "/downloads/mod1.zip",
                IsSubscriptionRequired = true
            },
            new Mod {
                Id = 3,
                Name = "Мод 3",
                Description = "Обновите свою игру с этим потрясающим модом, добавляющим реалистичные графические эффекты и новые звуковые элементы.",
                ImageUrl = "/Images/mod3.jpg",
                DownloadUrl = "/downloads/mod1.zip",
                IsSubscriptionRequired = true
            },
            new Mod {
                Id = 4,
                Name = "Мод 4",
                Description = "Мод, который меняет механику игры, добавляя дополнительные уровни сложности и уникальные испытания для игроков.",
                ImageUrl = "/Images/mod4.jpg",
                DownloadUrl = "/downloads/mod1.zip"
            }
        };

        public IActionResult Index() => View(mods);

        public IActionResult Details(int id)
        {
            var mod = mods.FirstOrDefault(m => m.Id == id);
            if (mod == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);

            // Проверяем, есть ли подписка
            var hasActiveSubscription = user != null && user.DateOfSubscription > DateTime.UtcNow;

            // Передаём данные через ViewData
            ViewData["HasActiveSubscription"] = hasActiveSubscription;

            return View(mod);
        }

        [Authorize]
        public IActionResult Download(int id)
        {
            var mod = mods.FirstOrDefault(m => m.Id == id);
            if (mod == null)
                return NotFound();

            if (mod.IsSubscriptionRequired)
            {

                // Получение информации о текущем пользователе из контекста
                var subscriptionEndDateClaim = User.FindFirst("DateOfSubscription");
                if (subscriptionEndDateClaim == null ||
                    !DateTime.TryParse(subscriptionEndDateClaim.Value, out var subscriptionEndDate) ||
                    subscriptionEndDate < DateTime.UtcNow)
                {
                    TempData["Error"] = "Для скачивания этого мода требуется активная подписка.";
                    return RedirectToAction("Details", new { id });
                }
            }

            return File(mod.DownloadUrl, "application/octet-stream", $"{mod.Name}.zip");
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
