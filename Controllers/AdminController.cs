using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YWOTBlyatz.Data;

namespace WOTBlyatz.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();

        }

        public IActionResult Mods()
        {
            var mods = _context.Mods.ToList();
            return View(mods);
        }

        public IActionResult Users()
        {
            return View();
        }
    }
}
