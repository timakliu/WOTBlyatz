using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WOTBlyatz.Models;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }


    [HttpGet]
    public IActionResult LoginWithGoogle(string returnUrl = null)
    {
        var redirectUrl = Url.Action("GoogleResponse", "Account", new { returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
        return Challenge(properties, "Google");
    }

    [HttpGet]
    public async Task<IActionResult> GoogleResponse(string returnUrl = null)
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return RedirectToAction(nameof(Login));
        }

        // Проверка существующего пользователя
        var user = await _userManager.FindByEmailAsync(info.Principal.FindFirstValue(ClaimTypes.Email));
        if (user == null)
        {
            // Регистрация нового пользователя
            user = new ApplicationUser
            {
                UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                DateOfRegistration = DateTime.UtcNow
            };
            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return View("Error", result.Errors);
            }
        }

        // Вход через Google
        await _signInManager.SignInAsync(user, isPersistent: false);
        return LocalRedirect(returnUrl ?? "/");
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        // Проверка существования пользователя с таким же именем
        var existingUserByUsername = await _userManager.FindByNameAsync(model.Username);
        if (existingUserByUsername != null)
        {
            ModelState.AddModelError("Username", "Пользователь с таким именем уже существует");
        }

        // Проверка существования пользователя с таким же email
        var existingUserByEmail = await _userManager.FindByEmailAsync(model.Email);
        if (existingUserByEmail != null)
        {
            ModelState.AddModelError("Email", "Пользователь с таким email уже существует");
        }

        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                DateOfRegistration = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            // Добавление ошибок от Identity
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Login(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (ModelState.IsValid)
        {
            // Используем Email для входа
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim("DateOfSubscription", user.DateOfSubscription.ToString("O")) // ISO 8601 формат
                    };
                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                    }
                    var claimsIdentity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
                    await _signInManager.SignOutAsync(); // Завершаем предыдущую сессию, если есть
                    await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, new ClaimsPrincipal(claimsIdentity));

                    // Безопасное перенаправление
                    return LocalRedirect(returnUrl ?? Url.Content("~/"));
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return View("Lockout");
                }
            }

            ModelState.AddModelError(string.Empty, "Неверный логин или пароль.");
        }

        return View(model);
    }


    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out.");
        return RedirectToAction("Index", "Home");
    }
}
