using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManagementApp.Data;
using TaskManagementApp.Models;

namespace TaskManagementApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly TaskManagerDbContext _context;

        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 TaskManagerDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(ApplicationUser model, string password)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");

                var historyLog = new HistoryLog
                {
                    UserName = user.UserName,
                    Action = "Rejestracja",
                    ActionDate = DateTime.Now
                };
                _context.HistoryLogs.Add(historyLog);
                await _context.SaveChangesAsync();

                await _signInManager.SignInAsync(user, false);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError("", e.Description);
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(email, password, false, false);
                if (result.Succeeded)
                {
                    var historyLog = new HistoryLog
                    {
                        UserName = email,
                        Action = "Logowanie",
                        ActionDate = DateTime.Now
                    };
                    _context.HistoryLogs.Add(historyLog);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Błędne dane logowania.");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View("~/Views/Account/AccessDenied.cshtml");
        }

    }
}
