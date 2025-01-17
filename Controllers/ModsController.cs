using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WOTBlyatz.Models;
using YWOTBlyatz.Data;

namespace WOTBlyatz.Controllers
{
    public class ModsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly ApplicationDbContext _context;

        private readonly List<Mod> mods;
        public ModsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
            mods = _context.Mods.ToList();
        }


        private static readonly List<Comment> comments = new()
{
    // Комментарии для Mod 1
    new Comment
    {
        UserName = "User1",
        Text = "Очень полезный мод! Использую уже неделю.",
        Date = DateTime.UtcNow.AddDays(-3),
        ModId = 1
    },
    new Comment
    {
        UserName = "Gamer2",
        Text = "Хотелось бы больше функций, но и так неплохо.",
        Date = DateTime.UtcNow.AddDays(-2),
        ModId = 1
    },
    new Comment
    {
        UserName = "Player3",
        Text = "Рекомендую всем, кто хочет улучшить игровой процесс.",
        Date = DateTime.UtcNow.AddDays(-1),
        ModId = 1
    },

    // Комментарии для Mod 2
    new Comment
    {
        UserName = "ProGamer",
        Text = "Лучший мод для профессионалов!",
        Date = DateTime.UtcNow.AddDays(-4),
        ModId = 2
    },
    new Comment
    {
        UserName = "ModFan",
        Text = "Подписка того стоит. Классный мод!",
        Date = DateTime.UtcNow.AddDays(-2),
        ModId = 2
    },
    new Comment
    {
        UserName = "Critic",
        Text = "Не работает на старых версиях игры. Исправьте, пожалуйста.",
        Date = DateTime.UtcNow,
        ModId = 2
    },

    // Комментарии для Mod 3
    new Comment
    {
        UserName = "VisualFan",
        Text = "Графика стала намного лучше. Спасибо!",
        Date = DateTime.UtcNow.AddDays(-1),
        ModId = 3
    },
    new Comment
    {
        UserName = "SoundLover",
        Text = "Звуки стали более реалистичными. Хорошая работа!",
        Date = DateTime.UtcNow.AddDays(-3),
        ModId = 3
    },
    new Comment
    {
        UserName = "Tester",
        Text = "Иногда игра вылетает. Надеюсь, это скоро исправят.",
        Date = DateTime.UtcNow,
        ModId = 3
    },

    // Комментарии для Mod 4 (aaa 2)
    new Comment
    {
        UserName = "OldSchool",
        Text = "Очень классный мод, хотя немного устарел.",
        Date = DateTime.UtcNow.AddDays(-10),
        ModId = 4
    },
    new Comment
    {
        UserName = "Explorer",
        Text = "Мод супер, особенно графические эффекты!",
        Date = DateTime.UtcNow.AddDays(-5),
        ModId = 4
    },
    new Comment
    {
        UserName = "CasualPlayer",
        Text = "Играю с этим модом уже месяц. Никаких проблем.",
        Date = DateTime.UtcNow.AddDays(-2),
        ModId = 4
    }
};


        [Authorize]
        public IActionResult Index() => View(mods);


        [Authorize(Roles = "Admin")]
        public IActionResult Details(int id)
        {
            var mod = mods.FirstOrDefault(m => m.Id == id);
            if (mod == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);

            var hasActiveSubscription = user != null && user.DateOfSubscription > DateTime.UtcNow;

            var otherMods = mods.Where(m => m.Id != id).ToList();

            ViewData["HasActiveSubscription"] = hasActiveSubscription;
            ViewData["OtherMods"] = otherMods;
            ViewData["Comments"] = comments.Where(c => c.ModId == id).OrderByDescending(c => c.Date).ToList();

            return View(mod);
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddComment(int modId, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                TempData["Error"] = "Комментарий не может быть пустым.";
                return RedirectToAction("Details", new { id = modId });
            }

            var userName = User.Identity.Name;
            comments.Add(new Comment
            {
                UserName = userName,
                Text = text,
                Date = DateTime.UtcNow,
                ModId = modId
            });

            return RedirectToAction("Details", new { id = modId });
        }



        [Authorize]
        public IActionResult Download(int id)
        {
            var mod = mods.FirstOrDefault(m => m.Id == id);
            if (mod == null)
                return NotFound();

            if (mod.IsSubscriptionRequired)
            {
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
