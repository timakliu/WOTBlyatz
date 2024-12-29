using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
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
                Description = "���� ��� ��������� ����� ����������� � ��������� � ������� �������, ����� ��� ����� ������������� � �������������.",
                ImageUrl = "/Images/mod1.jpg",
                DownloadUrl = "/downloads/mod1.zip",
                IsSubscriptionRequired = false

            },
            new Mod {
                Id = 2,
                Name = "��� 2",
                Description = "���������� ���, ������� ������������� ������� ������ � ����� ������������, ���������� � ��������.",
                ImageUrl = "/Images/mod2.jpg",
                DownloadUrl = "/downloads/mod1.zip",
                IsSubscriptionRequired = true
            },
            new Mod {
                Id = 3,
                Name = "��� 3",
                Description = "�������� ���� ���� � ���� ����������� �����, ����������� ������������ ����������� ������� � ����� �������� ��������.",
                ImageUrl = "/Images/mod3.jpg",
                DownloadUrl = "/downloads/mod1.zip",
                IsSubscriptionRequired = true
            },
            new Mod {
                Id = 4,
                Name = "��� 4",
                Description = "���, ������� ������ �������� ����, �������� �������������� ������ ��������� � ���������� ��������� ��� �������.",
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

            // ���������, ���� �� ��������
            var hasActiveSubscription = user != null && user.DateOfSubscription > DateTime.UtcNow;

            // ������� ������ ����� ViewData
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
                //var user = HttpContext.User; // ������� ������������
                //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // �������� ID ������������
                //var appUser = _userManager.Users.FirstOrDefault(u => u.Id == userId);

                //if (appUser == null || appUser.DateOfSubscription < DateTime.UtcNow)
                //{
                //    TempData["Error"] = "��� ���������� ����� ���� ��������� �������� ��������.";
                //    return RedirectToAction("Details", new { id });
                //}

                // ��������� ���������� � ������� ������������ �� ���������
                var subscriptionEndDateClaim = User.FindFirst("DateOfSubscription");
                if (subscriptionEndDateClaim == null ||
                    !DateTime.TryParse(subscriptionEndDateClaim.Value, out var subscriptionEndDate) ||
                    subscriptionEndDate < DateTime.UtcNow)
                {
                    TempData["Error"] = "��� ���������� ����� ���� ��������� �������� ��������.";
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
