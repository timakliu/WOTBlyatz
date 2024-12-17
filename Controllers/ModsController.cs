using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WOTBlyatz.Models;

namespace WOTBlyatz.Controllers
{
    public class ModsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public ModsController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        private readonly List<Mod> mods = new() {
            new Mod {
                Id = 1,
                Name = "Мод 1",
                Description = "Этот мод добавляет новые возможности и улучшения в игровой процесс, делая его более увлекательным и разнообразным.",
                ImageUrl = "/images/mod1.jpg",
                DownloadUrl = "/downloads/mod1.zip",
                IsSubscriptionRequired = false

            },
            new Mod {
                Id = 2,
                Name = "Мод 2",
                Description = "Уникальный мод, который предоставляет игрокам доступ к новым инструментам, персонажам и локациям.",
                ImageUrl = "/images/mod2.jpg",
                DownloadUrl = "/downloads/mod1.zip",
                IsSubscriptionRequired = true
            },
            new Mod {
                Id = 3,
                Name = "Мод 3",
                Description = "Обновите свою игру с этим потрясающим модом, добавляющим реалистичные графические эффекты и новые звуковые элементы.",
                ImageUrl = "/images/mod3.jpg",
                DownloadUrl = "/downloads/mod1.zip",
                IsSubscriptionRequired = true
            },
            new Mod {
                Id = 4,
                Name = "Мод 4",
                Description = "Мод, который меняет механику игры, добавляя дополнительные уровни сложности и уникальные испытания для игроков.",
                ImageUrl = "/images/mod4.jpg",
                DownloadUrl = "/downloads/mod1.zip"
            }
        };

        [Authorize]
        public IActionResult Index() => View(mods);
        //public IActionResult Details(int id)
        //{
        //    var mod = mods.FirstOrDefault(m => m.Id == id);
        //    return mod == null ? NotFound() : View(mod);
        //}

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
                //var user = HttpContext.User; // Текущий пользователь
                //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Получаем ID пользователя
                //var appUser = _userManager.Users.FirstOrDefault(u => u.Id == userId);

                //if (appUser == null || appUser.DateOfSubscription < DateTime.UtcNow)
                //{
                //    TempData["Error"] = "Для скачивания этого мода требуется активная подписка.";
                //    return RedirectToAction("Details", new { id });
                //}

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

    }
}
