using appointments.Models;
using appointments.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using vacations.Models.Helper;

namespace appointments.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Vacation");
                }
                ModelState.AddModelError("", "Próba logowania nieudana");
            }
            return View();
        }

        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!User.IsInRole(RoleNames.Role_Admin))
                ModelState.Remove("RoleName");

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                    VacationDays = 26,
                    VacationDaysTaken = 0
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    if (model.RoleName == null)
                        model.RoleName = RoleNames.Role_AppWorker;

                    await _userManager.AddToRoleAsync(user, model.RoleName);
                    // await _signInManager.SignInAsync(user, false);
                    bool isLoggedin = (_httpContextAccessor.HttpContext.User != null) &&
                        (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated);

                    if (isLoggedin)
                        return RedirectToAction("Index", "Home");

                    return RedirectToAction("Login", "Account");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");

        }
    }
}
