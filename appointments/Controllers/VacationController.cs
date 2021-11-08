using appointments.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using vacations.Models.Helper;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;
using appointments.Models.ViewModels;

namespace appointments.Controllers
{
    public class VacationController : Controller
    {
        private readonly IVacationService _vacationService;
        private readonly IHttpContextAccessor _context;
        public VacationController(IVacationService vacationService, IHttpContextAccessor context)
        {
            _vacationService = vacationService;
            _context = context;
        }

        [Authorize(Roles = "Admin, AppWorker")]
        public IActionResult Index()
        {
            if (User.IsInRole(RoleNames.Role_Admin))
            {
                ViewBag.WorkerList = _vacationService.GetWorkerList();
            }
            else
            {
                var currentUser = _vacationService.GetCurrentUser();
                
                ViewBag.CurrentUser = currentUser;
            }
            return View();
        }
    }
}
